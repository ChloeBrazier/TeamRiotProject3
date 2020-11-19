
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
            e_color = Color.black;
            tileIndex = 0;
            spaceby = null;
        }
        public Entity(GameObject obj, int h)
        {
            self = obj;
            pos = obj.transform.position;
            health = h;
            current_tile = null;
            e_color = Color.black;
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

            e_color = self.GetComponent<SpriteRenderer>().color;
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
    List<Entity> Tiles_e;
    Entity Player_e;
    public enum combatOptions
    {
        move, attack, flee, none
    }

    combatOptions currentOpt;//player

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
        Tiles_e = new List<Entity>();
        foreach (var tile in tiles)
        {
            Tiles_e.Add(new Entity(tile));

        }

        var enemies = GameObject.FindGameObjectsWithTag("enemy");
        for(int i = 0; i < num_enemies; i++)
        {
            var tile_range = (int)UnityEngine.Random.Range(0, tiles.Length-1);
            GameObject enemy = Instantiate(Enemy);
            v3 e_p = tiles[tile_range].transform.position;
            e_p.y = -2f;
            enemy.transform.position = e_p;
            Enemies.Add(new Entity(enemy, 3, tiles[tile_range], tile_range));
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
    public void Enter()
    {
        switch (currentOpt)
        {
            case combatOptions.move:
                Debug.Log(currentOpt.ToString());
                playerTurn = false;
                break;
            case combatOptions.attack:
                Debug.Log(currentOpt.ToString());
                playerTurn = false;

                break;
            case combatOptions.flee:
                Debug.Log(currentOpt.ToString());
                playerTurn = false;
                break;
            case combatOptions.none:
                break;
            default:
                break;
        }
        CheckEnemyDMG();
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
                num_moves = 3;
            }
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
            foreach (var tile_e in Tiles_e)
            {
                var tile = tile_e.self;

                if (GameObject.ReferenceEquals(enemy.current_tile, tile_e.self))
                {

                    if (tile_e.SpaceBy() == "Player")
                    {
                        Debug.Log(tile_e.SpaceBy());
                        enemy.SubtractHealth();
                        Debug.Log(enemy.health);
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
        if (currentPlayerTileIndex - tiles.Length / 4 >= 0)
        {
            currentPlayerTileIndex = currentPlayerTileIndex - tiles.Length / 4;
            t = tiles[currentPlayerTileIndex];
            // original_tile = t.GetComponent<SpriteRenderer>().color;
            Tiles_e[currentPlayerTileIndex].self.GetComponent<SpriteRenderer>().color = c;

            Tiles_e[currentPlayerTileIndex] = new Entity(Tiles_e[currentPlayerTileIndex].self, currentPlayerTileIndex, "Player");

        }
        if ((currentPlayerTileIndex + 1) % 7 != 0)
        {
            currentPlayerTileIndex = currentPlayerTileIndex + 1;
            t = tiles[currentPlayerTileIndex];
            //original_tile = t.GetComponent<SpriteRenderer>().color;
            Tiles_e[currentPlayerTileIndex].self.GetComponent<SpriteRenderer>().color = c;

            Tiles_e[currentPlayerTileIndex] = new Entity(Tiles_e[currentPlayerTileIndex].self, currentPlayerTileIndex, "Player");
        }
        if (currentPlayerTileIndex + tiles.Length / 4 <= tiles.Length)
        {
            currentPlayerTileIndex = currentPlayerTileIndex + tiles.Length / 4;
            t = tiles[currentPlayerTileIndex];
            //original_tile = t.GetComponent<SpriteRenderer>().color;
            Tiles_e[currentPlayerTileIndex].self.GetComponent<SpriteRenderer>().color = c;

            Tiles_e[currentPlayerTileIndex] = new Entity(Tiles_e[currentPlayerTileIndex].self, currentPlayerTileIndex, "Player");
        }

        if ((currentPlayerTileIndex) % 7 != 0)
        {
            currentPlayerTileIndex = currentPlayerTileIndex - 1;
            t = tiles[currentPlayerTileIndex];
            //original_tile = t.GetComponent<SpriteRenderer>().color;
            Tiles_e[currentPlayerTileIndex].self.GetComponent<SpriteRenderer>().color = c;

            Tiles_e[currentPlayerTileIndex] = new Entity(Tiles_e[currentPlayerTileIndex].self, currentPlayerTileIndex, "Player");
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
                            Debug.Log(enemy.tileIndex);
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
                            Debug.Log(enemy.tileIndex);
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
                            Debug.Log(enemy.tileIndex);
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
                            Debug.Log(enemy.tileIndex);
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
                        Debug.Log(Player_e.SpaceBy());
                        Player_e.SubtractHealth();
                        Debug.Log(Player_e.health);
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


