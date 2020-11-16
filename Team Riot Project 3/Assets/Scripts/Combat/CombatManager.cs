
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.ComTypes;
using UnityEngine;
using v3 = UnityEngine.Vector3;
using v2 = UnityEngine.Vector2;
using UnityEngine.Animations;
using UnityEngine.UI;
using System.Runtime.InteropServices;

public class CombatManager : MonoBehaviour
{
    struct Entity
    {
        public v3 pos;
        public GameObject self;
        public GameObject current_tile;
        public int health;
        public Color e_color;
        
         
        public Entity(GameObject obj)
        {
            self = obj;
            pos = obj.transform.position;
            health = -1;
            current_tile = null;
            e_color = Color.black;
        }
        public Entity(GameObject obj, int h)
        {
            self = obj;
            pos = obj.transform.position;
            health = h;
            current_tile = null;
            e_color = Color.black;
        }
        public Entity(GameObject obj, int h, GameObject t)
        {
            self = obj;
            pos = obj.transform.position;
            health = h;
            current_tile = t;
            
            e_color = self.GetComponent<SpriteRenderer>().color;
        }
        public void SubtractHealth()
        {
            health--;
        }
        public void AddHealth()
        {
            health++;
        }
    }
    struct AttackProperties
    {
        public Color str;
        public Color weak;
        public Color attackColor;
        public AttackProperties(Color s, Color w)
        {
            str = s;
            weak = w;
            attackColor = Color.black;
        }
        
    }
    // Start is called before the first frame update
    GameObject cam;
    GameObject right;
    GameObject left;
    Camera mainCam;
    Transform original = null;
    AttackProperties atk_prop;
    int rows = 6;
    int cols = 4;
    int num_moves = 3;
    float tileSize = 1;
    public GameObject reftile;
    public GameObject Player;
    private int currentPlayerTileIndex;
    public GameObject floor;
    //public GameObject Enemy;
    public GameObject Enemy;
    float move_width;
    float move_height;
    GameObject[] tiles;
    //Menus
    public GameObject combatmenu;
    public GameObject elementmenu;
    public GameObject movemenu;
    public GameObject fleemenu;
    public GameObject attackmenu;
    public GameObject one;
    public GameObject two;
    bool moveselected = false;
    bool w = false;
    bool a = false;
    bool s = false;
    bool d = false;
    int num_enemies = 2;
    Color original_tile;
    bool playerTurn = false;
    int player_lvl = 0;
    float xp_nextlvl = 100.0f;
    v3 menu_pos;
    List<Entity> Enemies;
    public enum combatOptions
    {
        move, attack, flee, none
    }

    combatOptions currentOpt;

    void Start()
    {

        cam = GameObject.FindGameObjectWithTag("MainCamera");
        mainCam = cam.GetComponent<Camera>();
        tiles = GameObject.FindGameObjectsWithTag("tile");
        Enemies = new List<Entity>();
        GameObject starttile = tiles[0];
        currentPlayerTileIndex = 0;
        var spr_render = starttile.GetComponent<SpriteRenderer>().bounds.size;
        move_width = spr_render.x;
        move_height = spr_render.y;
        var n_p = starttile.transform.position;
        n_p.y = -2;
        Player.transform.position = n_p;

        original_tile = starttile.GetComponent<SpriteRenderer>().color;
        //ENEMY POS
        //Enemy.transform.position = tiles[14].transform.position;

        currentOpt = combatOptions.none;
        Debug.Log(currentOpt);
        CheckIntersectXY(Player.transform.position, starttile.transform.position);
        playerTurn = true;

        
        var enemies = GameObject.FindGameObjectsWithTag("enemy");
        for(int i = 0; i < num_enemies; i++)
        {
            var tile_range = (int)UnityEngine.Random.Range(0, tiles.Length-1);
            GameObject enemy = Instantiate(Enemy);
            v3 e_p = tiles[tile_range].transform.position;
            e_p.y = -2f;
            enemy.transform.position = e_p;
            //enemy.transform.position = e_p;
            var rand = (int)UnityEngine.Random.Range(0, 5);
            var renderer = enemy.GetComponent<SpriteRenderer>();
            switch (rand)
            {
                case 0:
                    renderer.color = Color.green;
                    Enemies.Add(new Entity(enemy, 3, tiles[tile_range]));
                    // renderer.color = new v3(.6f, .4f, .2f);
                    break;
                case 1:
                    renderer.color = Color.red;
                    break;
                case 2:
                    renderer.color = Color.blue;
                    break;
                case 3:
                    renderer.color = Color.Lerp(Color.yellow, Color.green, 0.75f);
                    break;
                case 4:
                    renderer.color = Color.grey;
                    break;
                default:
                    break;
            }
        }
        // Update is called once per frame

        if (playerTurn == true)
        {
            Debug.Log(playerTurn);
            elementmenu.SetActive(false);
            movemenu.SetActive(false);
            fleemenu.SetActive(false);
            attackmenu.SetActive(false);
        }

    }

    public void Attack(GameObject obj)
    {
        attackmenu.SetActive(false);
        movemenu.SetActive(true);
        moveselected = true;
        switch (obj.GetComponentInChildren<Text>().text)
        {
            case "Quake":
                
               // Debug.Log("Enemy health: " + Enemies[0].health);
                atk_prop.attackColor = Color.green;
                break;
            case "Ember":
               
               // Debug.Log("Enemy health: " + Enemies[0].health);
                atk_prop.attackColor = Color.red;
                break;
            case "Douse":
           
               // Debug.Log("Enemy health: " + Enemies[0].health);
                atk_prop.attackColor = Color.blue;
                break;
            case "Bind":
              
               // Debug.Log("Enemy health: " + Enemies[0].health);
                atk_prop.attackColor = Color.Lerp(Color.yellow, Color.green, 0.75f);
                break;
            case "Harden":
                
               // Debug.Log("Enemy health: " + Enemies[0].health);
                atk_prop.attackColor = Color.grey;
                break;
            default:
                break;
        }
        //Debug.Log(obj.name);

    }

    public void AttackByElement(GameObject obj)
    {
        attackmenu.SetActive(true);
        elementmenu.SetActive(false);
        switch (obj.name)
        {
            case "Earth":
                var weak = Color.red;
                var strong = Color.blue;
                atk_prop = new AttackProperties(weak, strong);
                one.GetComponentInChildren<Text>().text = "Quake";
                if(player_lvl < 1)
                {
                    two.SetActive(false);
                }

                break;
            case "Fire":
                weak = Color.blue;
                strong = Color.red;
                atk_prop = new AttackProperties(weak, strong);
                one.GetComponentInChildren<Text>().text = "Ember";
                if (player_lvl < 1)
                {
                    two.SetActive(false);
                }
                break;
            case "Water":
                weak = Color.green;
                strong = Color.red;
                atk_prop = new AttackProperties(weak, strong);
                one.GetComponentInChildren<Text>().text = "Douse";
                if (player_lvl < 1)
                {
                    two.SetActive(false);
                }
                break;
            case "Wood":
                weak = Color.grey;
                strong = Color.green;
                atk_prop = new AttackProperties(weak, strong);
                one.GetComponentInChildren<Text>().text = "Bind";
                if (player_lvl < 1)
                {
                    two.SetActive(false);
                }
                break;
            case "Metal":
                weak = Color.red;
                strong = Color.Lerp(Color.yellow, Color.green, 0.75f);
                atk_prop = new AttackProperties(weak, strong);
                one.GetComponentInChildren<Text>().text = "Harden";
                if (player_lvl < 1)
                {
                    two.SetActive(false);
                }
                break;
            default:
                break;
        }
        //Debug.Log(obj.name);

    }

    public void AttackOption()
    {
        Debug.Log("Attack");
        //Instantiate(elementmenu);
        elementmenu.SetActive(true);
        combatmenu.SetActive(!combatmenu.activeSelf);
        currentOpt = combatOptions.attack;
    }

    public void MoveOption()
    {
        Debug.Log("Move");
        movemenu.SetActive(true);
        combatmenu.SetActive(!combatmenu.activeSelf);
        currentOpt = combatOptions.move;
    }

    public void FleeOption()
    {
        Debug.Log("Flee");
        fleemenu.SetActive(true);
        combatmenu.SetActive(!combatmenu.activeSelf);
        currentOpt = combatOptions.flee;
    }

    public void Back()
    {
        Debug.Log(attackmenu.activeSelf);
        if (moveselected)
        {
            
            attackmenu.SetActive(true);
            movemenu.SetActive(false);
            moveselected = false;
            return;
        }
        
        if(attackmenu.activeSelf == true)
        {
            attackmenu.SetActive(false);
            elementmenu.SetActive(!elementmenu.activeSelf);
            return;
        }
        switch (currentOpt)
        {
            case combatOptions.move:
                movemenu.SetActive(false);
                combatmenu.SetActive(!combatmenu.activeSelf);
                currentOpt = combatOptions.none;
                break;
            case combatOptions.attack:
                elementmenu.SetActive(false);
                combatmenu.SetActive(!combatmenu.activeSelf);

                currentOpt = combatOptions.none;
                break;
            case combatOptions.flee:
                fleemenu.SetActive(false);
                combatmenu.SetActive(!combatmenu.activeSelf);
                currentOpt = combatOptions.none;
                break;
            case combatOptions.none:
                break;
            default:
                break;
        }
    }

    void Update()
    {
        var n_p = Player.transform.position;
        n_p.y = -2;
        Player.transform.position = n_p;
        int e_count = 0;
        foreach (var enemy in Enemies)
        {
            e_count++;
            if (playerTurn == false)
            {
                foreach (var tile in tiles)
                {
                    if (enemy.e_color == tile.GetComponent<SpriteRenderer>().color &&
                        moveselected)
                    {
                        enemy.AddHealth();
                    }
                    if (enemy.e_color == tile.GetComponent<SpriteRenderer>().color &&
                        moveselected)
                    {
                        enemy.SubtractHealth();
                    }
                }
            }
        }
        
        if (playerTurn == true)
        {
            switch (currentOpt)
            {
                case combatOptions.move:
                    if(original == null)
                    {
                        original = Player.transform;
                    }
                    PlayerMove();
                    break;
                case combatOptions.attack:
                    
                    if (moveselected)
                    {
                        AttackSpaceMove(atk_prop.attackColor);

                    }
                    else
                    {
                        
                        foreach (var item in tiles)
                        {
                            item.GetComponent<SpriteRenderer>().color = Color.white;
                        }
                    }
                    
                    
                    break;
                case combatOptions.flee:
                    break;
                case combatOptions.none:
                        
                    break;
                default:
                    break;
            }


        }
        else
        {
            EnemyMove();
        }

    }
    //GenFloor();
    //Destroy(reftile);


    void AttackSpaceMove(Color c)
    {
        GameObject t;
        if (currentPlayerTileIndex - tiles.Length / 4 >= 0)
        {
            currentPlayerTileIndex = currentPlayerTileIndex - tiles.Length / 4;
            t = tiles[currentPlayerTileIndex];
            original_tile = t.GetComponent<SpriteRenderer>().color;
            t.GetComponent<SpriteRenderer>().color = c;
            //Debug.Log(t.name);
            //Player.transform.position = tiles[currentPlayerTileIndex].transform.position;
            //playerTurn = false;
        }
        
        if ((currentPlayerTileIndex + 1) % 7 != 0)
        {
            currentPlayerTileIndex = currentPlayerTileIndex + 1;
            t = tiles[currentPlayerTileIndex];
            original_tile = t.GetComponent<SpriteRenderer>().color;
            t.GetComponent<SpriteRenderer>().color = c;
            //Debug.Log(t.name);
            //Player.transform.position = tiles[currentPlayerTileIndex].transform.position;
            //playerTurn = false;
        }
        
        if (currentPlayerTileIndex + tiles.Length / 4 <= tiles.Length)
        {
            currentPlayerTileIndex = currentPlayerTileIndex + tiles.Length / 4;
            t = tiles[currentPlayerTileIndex];
            original_tile = t.GetComponent<SpriteRenderer>().color;
            t.GetComponent<SpriteRenderer>().color = c;

            //Debug.Log(t.name);

            // Player.transform.position = tiles[currentPlayerTileIndex].transform.position;
            // playerTurn = false;
        }
        
        if ((currentPlayerTileIndex) % 7 != 0)
        {
            currentPlayerTileIndex = currentPlayerTileIndex - 1;
            t = tiles[currentPlayerTileIndex];
            original_tile = t.GetComponent<SpriteRenderer>().color;
            t.GetComponent<SpriteRenderer>().color = c;

            //Debug.Log(t.name);
            // Player.transform.position = tiles[currentPlayerTileIndex].transform.position;
            // playerTurn = false;
        }

       /* if (Input.GetKeyDown(KeyCode.W))
        {
            
            
        }
        else if (Input.GetKeyDown(KeyCode.A))
        {
            
        }
        else if (Input.GetKeyDown(KeyCode.S))
        {
            
        }
        else if (Input.GetKeyDown(KeyCode.D))
        {
            
        }*/
    }


    void PlayerMove()
    {

        
        if(num_moves <= 0)
        {
            currentOpt = combatOptions.none;
        }

        if (Input.GetKeyDown(KeyCode.W) && num_moves > 0)
        {
            if (currentPlayerTileIndex - tiles.Length / 4 >= 0)
            {
                currentPlayerTileIndex = currentPlayerTileIndex - tiles.Length / 4;
                Player.transform.position = tiles[currentPlayerTileIndex].transform.position;
                num_moves--;
                //playerTurn = false;
            }
        }
        else if (Input.GetKeyDown(KeyCode.A) && num_moves > 0)
        {
            if ((currentPlayerTileIndex + 1) % 7 != 0)
            {
                currentPlayerTileIndex = currentPlayerTileIndex + 1;
                Player.transform.position = tiles[currentPlayerTileIndex].transform.position;
                num_moves--;
                //playerTurn = false;
            }
        }
        else if (Input.GetKeyDown(KeyCode.S) && num_moves > 0)
        {
            if (currentPlayerTileIndex + tiles.Length / 4 <= tiles.Length)
            {
                currentPlayerTileIndex = currentPlayerTileIndex + tiles.Length / 4;
                Player.transform.position = tiles[currentPlayerTileIndex].transform.position;
                num_moves--;
                //playerTurn = false;
            }
        }
        else if (Input.GetKeyDown(KeyCode.D) && num_moves > 0)
        {
            if ((currentPlayerTileIndex) % 7 != 0)
            {
                currentPlayerTileIndex = currentPlayerTileIndex - 1;
                Player.transform.position = tiles[currentPlayerTileIndex].transform.position;
                num_moves--;
                //playerTurn = false;
            }
        }

    }





    void EnemyMove()
    {

    }


    bool CheckIntersectXY(v3 p1, v3 p2)

    {

        var x = p1.x / p2.x;
        var y = p1.y / p2.y;
        if (x > 0.5 || y > 0.5)
        {
            //Debug.Log(x);
            //Debug.Log(y);
            return true;
        }

        return false;
    }
}

// Update is called once per frame


