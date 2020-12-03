
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
        public int tileIndex;
        public string spaceby;

        public Entity(GameObject obj)
        {
            self = obj;
            pos = obj.transform.position;
            health = -1;
            current_tile = null;
            e_color = obj.GetComponent<SpriteRenderer>().color;
            tileIndex = 0;
            spaceby = null;
        }
        public Entity(GameObject obj, int h)
        {
            self = obj;
            pos = obj.transform.position;
            health = h;
            current_tile = null;
            e_color = obj.GetComponent<SpriteRenderer>().color;
            tileIndex = 0;
            spaceby = null;
        }
        //note added index since needed it for enemy movement
        public Entity(GameObject obj, int h, GameObject t,int index)
        {
            self = obj;
            pos = obj.transform.position;
            health = h;
            current_tile = t;
            tileIndex = index;
            spaceby = null;

            e_color = obj.GetComponent<SpriteRenderer>().color;
        }
        public Entity(GameObject obj, int index, string sp)
        {
            self = obj;
            pos = obj.transform.position;
            health = -1;
            current_tile = null;
            tileIndex = index;
            e_color = self.GetComponent<SpriteRenderer>().color;
           
            spaceby = sp;
        }
        public void SetSpaceBy(String str)
        {
            spaceby = str;
        }
        public String SpaceBy()
        {
            return spaceby;
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
        public AttackProperties(Color c)
        {
            str = Color.black;
            weak = Color.black;
            attackColor = c;
        }
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
    int num_moves = 3;//note must change value in loop as well
    float tileSize = 1;

    public HealthBar healthBar;
    public int maxHealth = 100;
    public int currentHealth;                             // health bar stuff

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
    public GameObject[] elements;
    bool moveselected = false;
    bool w = false;
    bool a = false;
    bool s = false;
    bool d = false;
    
    int num_enemies = 2;
    int originalidx = -1;
    Color original_tile;
    bool playerTurn = false;
    int player_lvl = 1;
    float xp_currentlvl = 0.0f;
    float xp_nextlvl = 100.0f;
    v3 menu_pos;
    List<Entity> Enemies;
    List<int> enemyHealth;
    List<string> attackBy;
    List<Entity> Tiles_e;

    Entity Player_e;
    public enum combatOptions
    {
        move, attack, flee, none
    }
    public enum ElementAttacks
    {
        Quake, Ember, Douse, Bind, Harden, none
    }
    public enum Dir
    {
        left, right, up, down, none
    }
    combatOptions currentOpt;//player
    ElementAttacks player_attacks;
    Dir player_dir;
    void Start()
    {
        currentHealth = maxHealth;
        healthBar.SetMaxHealth(maxHealth);               // health bar stuff

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
        Debug.Log((Color)starttile.GetComponent<SpriteRenderer>().color);
        original_tile = starttile.GetComponent<SpriteRenderer>().color;

        
        //ENEMY POS
        //Enemy.transform.position = tiles[14].transform.position;

        currentOpt = combatOptions.none;
        
        //Debug.Log(currentOpt);
        CheckIntersectXY(Player.transform.position, starttile.transform.position);
        playerTurn = true;
        Tiles_e = new List<Entity>();
        int c = 0;
        attackBy = new List<string>();
        foreach (var tile in tiles)
        {
            tile.name = c.ToString();
            Tiles_e.Add(new Entity(tile, c, ""));
            attackBy.Add("empty");
            c++;
        }
        Player_e = new Entity(Player, currentHealth ,starttile, 0);
        enemyHealth = new List<int>();
        
        var enemies = GameObject.FindGameObjectsWithTag("enemy");
        for(int i = 0; i < num_enemies; i++)
        {
            var tile_range = (int)UnityEngine.Random.Range(0, tiles.Length-1);
            GameObject enemy = Instantiate(Enemy);
            v3 e_p = tiles[tile_range].transform.position;
            e_p.y = -2f;
            enemy.transform.position = e_p;
            Enemies.Add(new Entity(enemy, 3, tiles[tile_range], tile_range));
            enemyHealth.Add(3);
            
            //enemy.transform.position = e_p;
            var rand = (int)UnityEngine.Random.Range(0, 5);
            var renderer = enemy.GetComponent<SpriteRenderer>();
            switch (rand)
            {
                case 0:
                    renderer.color = Color.green;
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
            //Debug.Log(playerTurn);
            elementmenu.SetActive(false);
            movemenu.SetActive(false);
            fleemenu.SetActive(false);
            attackmenu.SetActive(false);
            player_attacks = ElementAttacks.none;
            player_dir = Dir.down;
        }
        
        
        foreach (var element in elements)
        {
            //Debug.Log(element.name);
        }
    }



    public void Attack(GameObject obj)
    {
        attackmenu.SetActive(false);
        movemenu.SetActive(true);
        moveselected = true;

        ResetTileColor();

        switch (obj.GetComponentInChildren<Text>().text)
        {
            case "Quake":
                
               // Debug.Log("Enemy health: " + Enemies[0].health);
                atk_prop.attackColor = Color.green;
                player_attacks = ElementAttacks.Quake;
                break;
            case "Ember":
               
               // Debug.Log("Enemy health: " + Enemies[0].health);
                atk_prop.attackColor = Color.red;
                player_attacks = ElementAttacks.Ember;
                break;
            case "Douse":
           
               // Debug.Log("Enemy health: " + Enemies[0].health);
                atk_prop.attackColor = Color.blue;
                player_attacks = ElementAttacks.Douse;
                break;
            case "Bind":
              
               // Debug.Log("Enemy health: " + Enemies[0].health);
                atk_prop.attackColor = Color.Lerp(Color.yellow, Color.green, 0.75f);
                player_attacks = ElementAttacks.Bind;
                break;
            case "Harden":
                
               // Debug.Log("Enemy health: " + Enemies[0].health);
                atk_prop.attackColor = Color.grey;
                player_attacks = ElementAttacks.Harden;
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
                ResetTileColor();
                var weak = Color.red;
                var strong = Color.blue;
                atk_prop = new AttackProperties(weak, strong);
                atk_prop.attackColor = Color.green;
                one.GetComponentInChildren<Text>().text = "Quake";
                two.SetActive(false);

                break;
            case "Fire":
                ResetTileColor();
                weak = Color.blue;
                strong = Color.red;
                atk_prop = new AttackProperties(weak, strong);
                atk_prop.attackColor = Color.red;
                one.GetComponentInChildren<Text>().text = "Ember";
                two.SetActive(false);
                
                break;
            case "Water":
                ResetTileColor();
                weak = Color.green;
                strong = Color.red;
                atk_prop = new AttackProperties(weak, strong);
                atk_prop.attackColor = Color.blue;
                one.GetComponentInChildren<Text>().text = "Douse";
                two.SetActive(false);
                
                break;
            case "Wood":
                ResetTileColor();
                weak = Color.grey;
                strong = Color.green;
                atk_prop = new AttackProperties(weak, strong);
                atk_prop.attackColor = Color.Lerp(Color.yellow, Color.green, 0.75f);
                one.GetComponentInChildren<Text>().text = "Bind";
                two.SetActive(false);
                
                break;
            case "Metal":
                ResetTileColor();
                weak = Color.red;
                strong = Color.Lerp(Color.yellow, Color.green, 0.75f);
                atk_prop = new AttackProperties(weak, strong);
                atk_prop.attackColor = Color.grey;
                one.GetComponentInChildren<Text>().text = "Harden";
                two.SetActive(false);
                
                break;
            default:
                break;
        }
        //Debug.Log(obj.name);

    }

    public void AttackOption()
    {
        //Debug.Log("Attack");
        //Instantiate(elementmenu);
        elementmenu.SetActive(true);
        combatmenu.SetActive(!combatmenu.activeSelf);
        currentOpt = combatOptions.attack;
    }

    public void MoveOption()
    {
        //Debug.Log("Move");
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
    public void Enter()
    {
        if(currentOpt == combatOptions.move)
        {
            playerTurn = false;
            return;
        }
        if (currentOpt == combatOptions.attack)
        {
            CheckEnemyDMG();
            originalidx = -1;
            playerTurn = false;
            currentOpt = combatOptions.none;
            //movemenu.SetActive(false);
            return;
        }
        if (currentOpt == combatOptions.flee)
        {
            playerTurn = false;
            return;
        }
        if (currentOpt == combatOptions.none)
        {
            playerTurn = false;
            return;
        } 
        
        
    }
    public void Back()
    {
        //Debug.Log(attackmenu.activeSelf);
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

    private enum enemyTurnPhase 
    {
        move,attack
    }
    enemyTurnPhase enemyTurn;
    void Update()
    {
        var n_p = Player.transform.position;
        n_p.y = -2;
        Player.transform.position = n_p;
       
        if (player_lvl == 1)
        {
            elements[0].SetActive(true);
            elements[1].SetActive(false);
            elements[2].SetActive(false);
            elements[3].SetActive(false);
            elements[4].SetActive(false);
        }
        if (player_lvl == 2)
        {
            elements[0].SetActive(true);
            elements[1].SetActive(true);
            elements[2].SetActive(false);
            elements[3].SetActive(false);
            elements[4].SetActive(false);
        }
        if (player_lvl == 3)
        {
            elements[0].SetActive(true);
            elements[1].SetActive(true);
            elements[2].SetActive(false);
            elements[3].SetActive(false);
            elements[4].SetActive(false);
        }
        if (player_lvl == 4)
        {
            elements[0].SetActive(true);
            elements[1].SetActive(true);
            elements[2].SetActive(true);
            elements[3].SetActive(false);
            elements[4].SetActive(false);
        }
        if (player_lvl == 5)
        {
            elements[0].SetActive(true);
            elements[1].SetActive(true);
            elements[2].SetActive(true);
            elements[3].SetActive(true);
            elements[4].SetActive(false);
        }
        if (player_lvl == 6)
        {
            elements[0].SetActive(true);
            elements[1].SetActive(true);
            elements[2].SetActive(true);
            elements[3].SetActive(true);
            elements[4].SetActive(true);
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
                        if(originalidx == -1)
                        {
                            originalidx = currentPlayerTileIndex;
                        }
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
            if (enemyTurn==enemyTurnPhase.move)
            {
                EnemyMove();
                enemyTurn = enemyTurnPhase.attack;
            }
            else if (enemyTurn == enemyTurnPhase.attack) 
            {
                EnemyAttack();
                enemyTurn = enemyTurnPhase.move;
                playerTurn = true;
                currentOpt = combatOptions.none;
                num_moves = 3;
                combatmenu.SetActive(true);
                movemenu.SetActive(false);
            }
            
        }

        // health bar stuff
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            currentHealth -= 10;
            if (currentHealth <= 0)
            {
                currentHealth = 0;
            }
            healthBar.SetHealth(currentHealth);
        }

        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            currentHealth += 10;
            if (currentHealth >= 100)
            {
                currentHealth = 100;
            }
            healthBar.SetHealth(currentHealth);
        }

    }
    //GenFloor();
    //Destroy(reftile);


    void CheckEnemyDMG()
    {
        Debug.Log("Checking dmg");

        for (var i = 0; i < Enemies.Count; i++)
        {
            var enemy = Enemies[i];
            int u = 0;
            //Debug.Log(enemy.SpaceBy());

            foreach (var tile_e in Tiles_e)
            {
                var tile = tile_e.self;
                
                if (GameObject.ReferenceEquals(enemy.current_tile, tile_e.self))
                {

                    if (attackBy[u] == "Player")
                    {
                        
                        var health = enemy.health;
                        health--;
                        Enemies[i] = new Entity(enemy.self, health, tile_e.self, u);
                        enemyHealth[i]--;
                        Debug.Log("ENEMY TAKING DMG: " + i);
                        Debug.Log(enemyHealth[i]);
                        if (enemyHealth[i] <= 0)
                        {
                            Debug.Log("ENEMY KILLED");
                            Destroy(Enemies[i].self);
                            Enemies.RemoveAt(i);
                            
                            num_enemies--;
                            xp_currentlvl += 105.0f;
                            Debug.Log("XP Gain: " + xp_currentlvl);
                            if(xp_currentlvl >= xp_nextlvl)
                            {
                                player_lvl++;
                                xp_currentlvl = 0.0f;
                            }
                        }
                    }
                }
                u++;
            }
            //Debug.Log(enemy.SpaceBy());
            //if()



        }
    }

    void AttackSpaceMove(Color c)
    {
        GameObject t;
        int moveidx = currentPlayerTileIndex;
        moveidx = currentPlayerTileIndex;
        
       
        if (Input.GetKeyDown(KeyCode.S))
        {
            if (moveidx - 7 >= 0) //S
            {
                ResetTileColor();
                moveidx = currentPlayerTileIndex - 7;
                player_dir = Dir.down;
                //Tiles_e[moveidx] = new Entity(Tiles_e[moveidx].self, moveidx, "Player");

            }
            
        }
        if (Input.GetKeyDown(KeyCode.W))
        {
            if (moveidx + 7 <= tiles.Length) //N
            {
                ResetTileColor();
                moveidx = currentPlayerTileIndex + 7;
                player_dir = Dir.up;
                //Tiles_e[moveidx] = new Entity(Tiles_e[moveidx].self, moveidx, "Player");

            }
            
        }
        if (Input.GetKeyDown(KeyCode.D))
        {
            if (moveidx - 1 >= 0) //E
            {
                ResetTileColor();
                moveidx = currentPlayerTileIndex - 1;
                player_dir = Dir.right;
               // Tiles_e[moveidx] = new Entity(Tiles_e[moveidx].self, moveidx, "Player");

            }
            
        }
        if (Input.GetKeyDown(KeyCode.A))
        {
            if (moveidx + 1 <= tiles.Length) //W
            {
                ResetTileColor();
                moveidx = currentPlayerTileIndex + 1;
                player_dir = Dir.left;
                //Tiles_e[moveidx] = new Entity(Tiles_e[moveidx].self, moveidx, "Player");

            }
        }
        

        switch (player_dir)
        {
            case Dir.left:
                Attack(c, moveidx);

                break;
            case Dir.right:
                Attack(c, moveidx);
                break;
            case Dir.up:
                Attack(c, moveidx);
                break;
            case Dir.down:
                Attack(c, moveidx);
                break;
            case Dir.none:
                break;
            default:
                
                
                break;
        }


        

    }

    void Attack(Color c, int curr)
    {
        GameObject t;

        int moveidx = curr;
        //currentPlayerTileIndex = originalidx;
        var diff = moveidx - originalidx;

       
        switch (player_attacks)
            {   
            case ElementAttacks.Quake:
                
                //left
                if (currentPlayerTileIndex + 1 <= tiles.Length) //W
                {

                    t = tiles[currentPlayerTileIndex + 1];
                    Tiles_e[currentPlayerTileIndex + 1] = new Entity(t, currentPlayerTileIndex + 1, "Player");
                    Tiles_e[currentPlayerTileIndex + 1].self.GetComponent<SpriteRenderer>().color = c;
                    attackBy[currentPlayerTileIndex + 1] = "Player";

                }
                if (currentPlayerTileIndex - 1 >= 0)
                {

                    t = tiles[currentPlayerTileIndex - 1];
                    Tiles_e[currentPlayerTileIndex - 1] = new Entity(t, currentPlayerTileIndex - 1, "Player");
                    Tiles_e[currentPlayerTileIndex - 1].self.GetComponent<SpriteRenderer>().color = c;
                    attackBy[currentPlayerTileIndex - 1] = "Player";
                }
                //up 
                if (currentPlayerTileIndex + 7 <= tiles.Length) //W
                {

                    t = tiles[currentPlayerTileIndex + 7];
                    Tiles_e[currentPlayerTileIndex + 7] = new Entity(t, currentPlayerTileIndex + 7, "Player");
                    Tiles_e[currentPlayerTileIndex + 7].self.GetComponent<SpriteRenderer>().color = c;
                    attackBy[currentPlayerTileIndex + 7] = "Player";
                }
                if (currentPlayerTileIndex - 7 >= 0)
                {

                    t = tiles[currentPlayerTileIndex - 7];
                    Tiles_e[currentPlayerTileIndex - 7] = new Entity(t, currentPlayerTileIndex - 7, "Player");
                    Tiles_e[currentPlayerTileIndex - 7].self.GetComponent<SpriteRenderer>().color = c;
                    attackBy[currentPlayerTileIndex - 7] = "Player";
                }
                break;
            case ElementAttacks.Ember:

                
                //left
                if(diff == 0 && player_dir == Dir.left)
                {
                    if (moveidx + 1 <= tiles.Length) //W
                    {
                        
                        t = tiles[moveidx + 1];
                        Tiles_e[moveidx + 1] = new Entity(t, moveidx + 1, "Player");
                        Tiles_e[moveidx + 1].self.GetComponent<SpriteRenderer>().color = c;
                        
                    }
                    if (moveidx + 2 <= tiles.Length) //W
                    {

                        t = tiles[moveidx + 2];
                        Tiles_e[moveidx + 2] = new Entity(t, moveidx + 2, "Player");
                        Tiles_e[moveidx + 2].self.GetComponent<SpriteRenderer>().color = c;
                        moveidx = currentPlayerTileIndex;
                        
                    }
                    
                }
                if (diff == -1 && player_dir == Dir.right)
                {
                    if (moveidx>= 0) 
                    {

                        t = tiles[moveidx];
                        Tiles_e[moveidx] = new Entity(t, moveidx, "Player");
                        Tiles_e[moveidx].self.GetComponent<SpriteRenderer>().color = c;
                        
                    }
                    if (moveidx - 1 >= 0) 
                    {

                        t = tiles[moveidx - 1];
                        Tiles_e[moveidx - 1] = new Entity(t, moveidx - 1, "Player");
                        Tiles_e[moveidx - 1].self.GetComponent<SpriteRenderer>().color = c;
                  
                    }

                }
                //up 
                if (diff == 7 && player_dir == Dir.up)
                {
                    if (moveidx <= tiles.Length) //W
                    {

                        t = tiles[moveidx];
                        Tiles_e[moveidx] = new Entity(t, moveidx, "Player");
                        Tiles_e[moveidx].self.GetComponent<SpriteRenderer>().color = c;
                        

                    }
                    if (moveidx + 7 <= tiles.Length) //W
                    {

                        t = tiles[moveidx + 7];
                        Tiles_e[moveidx + 7] = new Entity(t, moveidx + 7, "Player");
                        Tiles_e[moveidx + 7].self.GetComponent<SpriteRenderer>().color = c;
                        
                    }

                }
                if (diff == -7 && player_dir == Dir.down)
                {
                    if (moveidx >= 0)
                    {

                        t = tiles[moveidx];
                        Tiles_e[moveidx] = new Entity(t, moveidx, "Player");
                        Tiles_e[moveidx].self.GetComponent<SpriteRenderer>().color = c;
                        
                    }
                    if (moveidx - 7 >= 0)
                    {

                        t = tiles[moveidx - 7];
                        Tiles_e[moveidx - 7] = new Entity(t, moveidx - 7, "Player");
                        Tiles_e[moveidx - 7].self.GetComponent<SpriteRenderer>().color = c;
                        
                    }

                }


                break;
            case ElementAttacks.Douse:
                //left
                if (diff == 0 && player_dir == Dir.left)
                {
                    if (moveidx + 1 <= tiles.Length) //W
                    {

                        t = tiles[moveidx + 1];
                        Tiles_e[moveidx + 1] = new Entity(t, moveidx + 1, "Player");
                        Tiles_e[moveidx + 1].self.GetComponent<SpriteRenderer>().color = c;
                       

                    }
                    if (moveidx + 2 <= tiles.Length) //W
                    {

                        t = tiles[moveidx + 2];
                        Tiles_e[moveidx + 2] = new Entity(t, moveidx + 2, "Player");
                        Tiles_e[moveidx + 2].self.GetComponent<SpriteRenderer>().color = c;
                       
                        moveidx += 2;
                        if (moveidx + 7 <= tiles.Length)
                        {

                            t = tiles[moveidx + 7];
                            Tiles_e[moveidx + 7] = new Entity(t, moveidx + 7, "Player");
                            Tiles_e[moveidx + 7].self.GetComponent<SpriteRenderer>().color = c;
                            

                        }
                        if (moveidx - 7 <= tiles.Length)
                        {

                            t = tiles[moveidx - 7];
                            Tiles_e[moveidx - 7] = new Entity(t, moveidx - 7, "Player");
                            Tiles_e[moveidx - 7].self.GetComponent<SpriteRenderer>().color = c;
                         

                        }
                    }
                    
                    


                }
                if (diff == -1 && player_dir == Dir.right)
                {
                    if (moveidx >= 0)
                    {

                        t = tiles[moveidx];
                        Tiles_e[moveidx] = new Entity(t, moveidx, "Player");
                        Tiles_e[moveidx].self.GetComponent<SpriteRenderer>().color = c;
                        
                    }
                    if (moveidx - 1 >= 0)
                    {

                        t = tiles[moveidx - 1];
                        Tiles_e[moveidx - 1] = new Entity(t, moveidx - 1, "Player");
                        Tiles_e[moveidx - 1].self.GetComponent<SpriteRenderer>().color = c;
                        
                        moveidx -= 1;
                        if (moveidx + 7 <= tiles.Length)
                        {

                            t = tiles[moveidx + 7];
                            Tiles_e[moveidx + 7] = new Entity(t, moveidx + 7, "Player");
                            Tiles_e[moveidx + 7].self.GetComponent<SpriteRenderer>().color = c;
                            

                        }
                        if (moveidx - 7 <= tiles.Length)
                        {

                            t = tiles[moveidx - 7];
                            Tiles_e[moveidx - 7] = new Entity(t, moveidx - 7, "Player");
                            Tiles_e[moveidx - 7].self.GetComponent<SpriteRenderer>().color = c;
                            

                        }
                    }

                }
                //up 
                if (diff == 7 && player_dir == Dir.up)
                {
                    if (moveidx <= tiles.Length) 
                    {

                        t = tiles[moveidx];
                        Tiles_e[moveidx] = new Entity(t, moveidx, "Player");
                        Tiles_e[moveidx].self.GetComponent<SpriteRenderer>().color = c;
                       

                    }
                    if (moveidx + 7 <= tiles.Length) 
                    {

                        t = tiles[moveidx + 7];
                        Tiles_e[moveidx + 7] = new Entity(t, moveidx + 7, "Player");
                        Tiles_e[moveidx + 7].self.GetComponent<SpriteRenderer>().color = c;
                        
                        moveidx += 7;
                        if (moveidx + 1 <= tiles.Length)
                        {

                            t = tiles[moveidx + 1];
                            Tiles_e[moveidx + 1] = new Entity(t, moveidx + 1, "Player");
                            Tiles_e[moveidx + 1].self.GetComponent<SpriteRenderer>().color = c;
                            

                        }
                        if (moveidx - 1 <= tiles.Length)
                        {

                            t = tiles[moveidx - 1];
                            Tiles_e[moveidx - 1] = new Entity(t, moveidx - 1, "Player");
                            Tiles_e[moveidx - 1].self.GetComponent<SpriteRenderer>().color = c;
                       

                        }
                    }

                }
                if (diff == -7 && player_dir == Dir.down)
                {
                    if (moveidx >= 0)
                    {

                        t = tiles[moveidx];
                        Tiles_e[moveidx] = new Entity(t, moveidx, "Player");
                        Tiles_e[moveidx].self.GetComponent<SpriteRenderer>().color = c;
                        
                    }
                    if (moveidx - 7 >= 0)
                    {

                        t = tiles[moveidx - 7];
                        Tiles_e[moveidx - 7] = new Entity(t, moveidx - 7, "Player");
                        Tiles_e[moveidx - 7].self.GetComponent<SpriteRenderer>().color = c;
                        
                        moveidx -= 7;
                        if (moveidx + 1 <= tiles.Length)
                        {

                            t = tiles[moveidx + 1];
                            Tiles_e[moveidx + 1] = new Entity(t, moveidx + 1, "Player");
                            Tiles_e[moveidx + 1].self.GetComponent<SpriteRenderer>().color = c;
                            

                        }
                        if (moveidx - 1 <= tiles.Length)
                        {

                            t = tiles[moveidx - 1];
                            Tiles_e[moveidx - 1] = new Entity(t, moveidx - 1, "Player");
                            Tiles_e[moveidx - 1].self.GetComponent<SpriteRenderer>().color = c;
                         

                        }
                    }

                }
                break;
            case ElementAttacks.Bind:
                break;
            case ElementAttacks.Harden:
                t = tiles[currentPlayerTileIndex];
                Tiles_e[currentPlayerTileIndex] = new Entity(t, currentPlayerTileIndex, "Player");
                Tiles_e[currentPlayerTileIndex].self.GetComponent<SpriteRenderer>().color = c;
             
                break;
            case ElementAttacks.none:
                break;
            default:
                break;
        }

        

    }

    void ResetTileColor()
    {
        foreach (var item in Tiles_e)
        {
            item.self.GetComponent<SpriteRenderer>().color = original_tile;
        }
    }

    void PlayerMove()
    {
        if (num_moves > 0)
        {
            if (Input.GetKeyDown(KeyCode.S))
            {
                if (currentPlayerTileIndex - tiles.Length / 4 >= 0)
                {
                    currentPlayerTileIndex = currentPlayerTileIndex - tiles.Length / 4;
                    Player.transform.position = tiles[currentPlayerTileIndex].transform.position;
                    num_moves--;
                }
            }
            else if (Input.GetKeyDown(KeyCode.A))
            {
                if ((currentPlayerTileIndex + 1) % 7 != 0)
                {
                    currentPlayerTileIndex = currentPlayerTileIndex + 1;
                    Player.transform.position = tiles[currentPlayerTileIndex].transform.position;
                    num_moves--;
                }
            }
            else if (Input.GetKeyDown(KeyCode.W))
            {
                if (currentPlayerTileIndex + tiles.Length / 4 <= tiles.Length)
                {
                    currentPlayerTileIndex = currentPlayerTileIndex + tiles.Length / 4;
                    Player.transform.position = tiles[currentPlayerTileIndex].transform.position;
                    num_moves--;
                }
            }
            else if (Input.GetKeyDown(KeyCode.D))
            {
                if ((currentPlayerTileIndex) % 7 != 0)
                {
                    currentPlayerTileIndex = currentPlayerTileIndex - 1;
                    Player.transform.position = tiles[currentPlayerTileIndex].transform.position;
                    num_moves--;
                }
            }
        }
        else { playerTurn = false; }

    }




    private enum EnemyMoveDir 
    {
        none,left,right,up,down,start
    }
    private int enemy_moves = 1;
    void EnemyMove()
    {
        for(int i = 0; i < num_enemies; i++) 
        {
            Entity enemy = Enemies[i];//can't use for each throws error
            EnemyMoveDir dir = EnemyMoveDir.start;
            int movesLeft = enemy_moves;

            while (movesLeft > 0 && dir != EnemyMoveDir.none)
            {
                //decide movement dir
                //does NOT use tile calculations rather uses transforms of player and that enemy
                if (enemy.self.transform.position.z<Player.transform.position.z)
                {
                    dir = EnemyMoveDir.up;
                }
                else if (enemy.self.transform.position.z > Player.transform.position.z)
                {
                    dir = EnemyMoveDir.down;
                }
                else if (enemy.self.transform.position.x > Player.transform.position.x)
                {
                    dir = EnemyMoveDir.left;
                }
                else if (enemy.self.transform.position.x < Player.transform.position.x)
                {
                    dir = EnemyMoveDir.right;
                }
                else 
                {
                    dir = EnemyMoveDir.none;//just in case lands on same spot
                }


                //go that direction
                if (dir == EnemyMoveDir.down)
                {
                    if (enemy.tileIndex - tiles.Length / 4 >= 0)
                    {
                        if (currentPlayerTileIndex != enemy.tileIndex - tiles.Length / 4)
                        {
                            enemy.tileIndex = enemy.tileIndex - tiles.Length / 4;
                            //Debug.Log(enemy.tileIndex);
                            enemy.self.transform.position = tiles[enemy.tileIndex].transform.position;
                            movesLeft--;
                        }
                        else { dir = EnemyMoveDir.none; }
                    }
                }
                else if (dir == EnemyMoveDir.left)
                {
                    if ((enemy.tileIndex + 1) % 7 != 0)
                    {
                        if (currentPlayerTileIndex != enemy.tileIndex + 1)
                        {
                            enemy.tileIndex = enemy.tileIndex + 1;
                            //Debug.Log(enemy.tileIndex);
                            enemy.self.transform.position = tiles[enemy.tileIndex].transform.position;
                            movesLeft--;
                        }
                        else { dir = EnemyMoveDir.none; }
                    }
                }
                else if (dir == EnemyMoveDir.up)
                {
                    if (currentPlayerTileIndex + tiles.Length / 4 <= tiles.Length)
                    {
                        if (currentPlayerTileIndex != enemy.tileIndex + tiles.Length / 4)
                        {
                            enemy.tileIndex = enemy.tileIndex + tiles.Length / 4;
                            //Debug.Log(enemy.tileIndex);
                            enemy.self.transform.position = tiles[enemy.tileIndex].transform.position;
                            movesLeft--;
                        }
                        else { dir = EnemyMoveDir.none; }
                    }
                }
                else if (dir == EnemyMoveDir.right)
                {
                    if ((currentPlayerTileIndex) % 7 != 0)
                    {
                        if (currentPlayerTileIndex != enemy.tileIndex - 1)
                        {
                            enemy.tileIndex = enemy.tileIndex - 1;
                            //Debug.Log(enemy.tileIndex);
                            enemy.self.transform.position = tiles[enemy.tileIndex].transform.position;
                            movesLeft--;
                        }
                        else { dir = EnemyMoveDir.none; }
                    }
                }
            }
            v3 e_p = enemy.self.transform.position;
            e_p.y = -2f;
            enemy.self.transform.position = e_p;
        }
    }


    void EnemyAttack()
    {

        // Debug.Log("Enemey Attacking");
        int eidx = 0;
        for (int u = 0; u < Enemies.Count; u++)
        {
            int tileidx = 0;
            for (int i = 0; i < tiles.Length; i++)
            {
                if (GameObject.ReferenceEquals(tiles[i], Enemies[u].current_tile))
                {
                    tileidx = i;

                }
            }
            GameObject t;
            if (tileidx - tiles.Length / 4 >= 0)
            {
                tileidx = tileidx - tiles.Length / 4;
                t = tiles[tileidx];
                //original_tile = t.GetComponent<SpriteRenderer>().color;
                Tiles_e[tileidx].self.GetComponent<SpriteRenderer>().color = Enemies[u].self.GetComponent<SpriteRenderer>().color;

                Tiles_e[tileidx] = new Entity(Tiles_e[tileidx].self, tileidx, "Enemy");
                if (GameObject.ReferenceEquals(Player_e.current_tile, Tiles_e[tileidx].self))
                {

                    if (Tiles_e[tileidx].SpaceBy() == "Enemy")
                    {
                        currentHealth = currentHealth - 10;
                        Player_e = new Entity(Player, currentHealth, Tiles_e[tileidx].self, tileidx);
                        Debug.Log("Player Taking DMG: " + currentHealth);
                    }
                }
            }
            if ((tileidx + 1) % 7 != 0)
            {
                tileidx = tileidx + 1;
                t = tiles[tileidx];
                //original_tile = t.GetComponent<SpriteRenderer>().color;
                Tiles_e[tileidx].self.GetComponent<SpriteRenderer>().color = Enemies[u].self.GetComponent<SpriteRenderer>().color;

                Tiles_e[tileidx] = new Entity(Tiles_e[tileidx].self, tileidx, "Enemy");
                if (GameObject.ReferenceEquals(Player_e.current_tile, Tiles_e[tileidx].self))
                {

                    if (Tiles_e[tileidx].SpaceBy() == "Enemy")
                    {
                        Debug.Log(Player_e.SpaceBy());
                        Player_e.SubtractHealth();
                        Debug.Log(Player_e.health);
                    }
                }
            }
            if (tileidx + tiles.Length / 4 <= tiles.Length)
            {
                tileidx = tileidx + tiles.Length / 4;
                t = tiles[tileidx];
                //original_tile = t.GetComponent<SpriteRenderer>().color;
                Tiles_e[tileidx].self.GetComponent<SpriteRenderer>().color = Enemies[u].self.GetComponent<SpriteRenderer>().color;

                Tiles_e[tileidx] = new Entity(Tiles_e[tileidx].self, tileidx, "Enemy");
                if (GameObject.ReferenceEquals(Player_e.current_tile, Tiles_e[tileidx].self))
                {

                    if (Tiles_e[tileidx].SpaceBy() == "Enemy")
                    {
                        Debug.Log(Player_e.SpaceBy());
                        Player_e.SubtractHealth();
                        Debug.Log(Player_e.health);
                    }
                }
            }

            if ((tileidx) % 7 != 0)
            {
                tileidx = tileidx - 1;
                t = tiles[tileidx];
                //original_tile = t.GetComponent<SpriteRenderer>().color;
                Tiles_e[tileidx].self.GetComponent<SpriteRenderer>().color = Enemies[u].self.GetComponent<SpriteRenderer>().color;

                Tiles_e[tileidx] = new Entity(Tiles_e[tileidx].self, tileidx, "Enemy");
                if (GameObject.ReferenceEquals(Player_e.current_tile, Tiles_e[tileidx].self))
                {

                    if (Tiles_e[tileidx].SpaceBy() == "Enemy")
                    {
                        Debug.Log(Player_e.SpaceBy());
                        Player_e.SubtractHealth();
                        Debug.Log(Player_e.health);
                    }
                }
            }
            eidx++;
            //var reset = enemy.current_tile;
            //reset.GetComponent<SpriteRenderer>().color = //original_tile;
        }


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


