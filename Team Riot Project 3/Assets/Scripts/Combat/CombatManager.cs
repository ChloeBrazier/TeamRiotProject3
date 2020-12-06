
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
using UnityEngine.SceneManagement;

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
    public GameObject submitmenu;
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
    List<int> space;
    List<Entity> Tiles_e;
    private int updateInterval = 3;
    Entity Player_e;
    public enum combatOptions
    {
        move, attack, flee, none, enemy
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

    void Awake()
    {
        currentHealth = maxHealth;
        healthBar.SetMaxHealth(maxHealth);               // health bar stuff
        space = new List<int>();
        //Debug.Log("Game START");
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
        //Debug.Log((Color)starttile.GetComponent<SpriteRenderer>().color);
        original_tile = starttile.GetComponent<SpriteRenderer>().color;

        
        //ENEMY POS
        //Enemy.transform.position = tiles[14].transform.position;

        currentOpt = combatOptions.none;
        
        
        
        playerTurn = true;
        Tiles_e = new List<Entity>();
        int c = 0;
        attackBy = new List<string>();
        foreach (var tile in tiles)
        {
            tile.name = c.ToString();
            Tiles_e.Add(new Entity(tile, c, ""));
            attackBy.Add("empty");
            space.Add(-1);
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
            ////Debug.Log(playerTurn);
            elementmenu.SetActive(false);
            movemenu.SetActive(false);
            fleemenu.SetActive(false);
            attackmenu.SetActive(false);
            
            submitmenu.SetActive(false);
            player_attacks = ElementAttacks.none;
            player_dir = Dir.none;
        }
        if (player_lvl < PlayerStats.lvl)
        {
            player_lvl = PlayerStats.lvl;
        }
        //Application.targetFrameRate = 30;
    }



    public void Attack(GameObject obj)
    {
        attackmenu.SetActive(false);
        submitmenu.SetActive(true);
        moveselected = true;
        if (back == false && submitmenu.activeSelf == true
                && attackmenu.activeSelf == false)
        {
           // ResetTileColor();
            //back = true;
        }


        switch (obj.GetComponentInChildren<Text>().text)
        {
            case "Quake":
                
               // //Debug.Log("Enemy health: " + Enemies[0].health);
                atk_prop.attackColor = Color.green;
                player_attacks = ElementAttacks.Quake;
                Debug.Log(player_attacks);
                break;
            case "Ember":
               
               // //Debug.Log("Enemy health: " + Enemies[0].health);
                atk_prop.attackColor = Color.red;
                player_attacks = ElementAttacks.Ember;
                Debug.Log(player_attacks);
                break;
            case "Douse":
           
               // //Debug.Log("Enemy health: " + Enemies[0].health);
                atk_prop.attackColor = Color.blue;
                player_attacks = ElementAttacks.Douse;
                Debug.Log(player_attacks);
                break;
            case "Bind":
              
               // Debug.Log("Enemy health: " + Enemies[0].health);
                atk_prop.attackColor = Color.Lerp(Color.yellow, Color.green, 0.75f);
                player_attacks = ElementAttacks.Bind;
                Debug.Log(player_attacks);
                break;
            case "Harden":
                
               // Debug.Log("Enemy health: " + Enemies[0].health);
                atk_prop.attackColor = Color.grey;
                player_attacks = ElementAttacks.Harden;
                Debug.Log(player_attacks);
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
                //ResetTileColor();
                var weak = Color.red;
                var strong = Color.blue;
                atk_prop = new AttackProperties(weak, strong);
                atk_prop.attackColor = Color.green;
                one.GetComponentInChildren<Text>().text = "Quake";
                two.SetActive(false);

                break;
            case "Fire":
               // ResetTileColor();
                weak = Color.blue;
                strong = Color.red;
                atk_prop = new AttackProperties(weak, strong);
                atk_prop.attackColor = Color.red;
                one.GetComponentInChildren<Text>().text = "Ember";
                two.SetActive(false);
                
                break;
            case "Water":
                //ResetTileColor();
                weak = Color.green;
                strong = Color.red;
                atk_prop = new AttackProperties(weak, strong);
                atk_prop.attackColor = Color.blue;
                one.GetComponentInChildren<Text>().text = "Douse";
                two.SetActive(false);
                
                break;
            case "Wood":
              //  ResetTileColor();
                weak = Color.grey;
                strong = Color.green;
                atk_prop = new AttackProperties(weak, strong);
                atk_prop.attackColor = Color.Lerp(Color.yellow, Color.green, 0.75f);
                one.GetComponentInChildren<Text>().text = "Bind";
                two.SetActive(false);
                
                break;
            case "Metal":
               // ResetTileColor();
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
        num_moves = 3;
        movemenu.SetActive(true);
        combatmenu.SetActive(!combatmenu.activeSelf);
        currentOpt = combatOptions.move;
    }

    public void FleeOption()
    {
       // Debug.Log("Flee");
        fleemenu.SetActive(true);
        combatmenu.SetActive(!combatmenu.activeSelf);
        currentOpt = combatOptions.flee;
    }
    
    public void Flee()
    {
        if (currentOpt == combatOptions.flee)
        {
            var range = (int)UnityEngine.Random.Range(0, Enemies.Count);
            if (range % 2 != 0)
            {
                playerTurn = false;
                currentOpt = combatOptions.none;

            }
            else if (range % 2 == 0)
            {
                EndBattle();
            }
        }
    }

    bool move = false;
    public void MoveEnter()
    {
        movemenu.SetActive(false);
        //currentOpt = combatOptions.enemy;
        //enemyTurn = enemyTurnPhase.move;
        //StartCoroutine(CheckEnemyDMG());
        //playerTurn = false;
       // CheckEnemyDMG();
        move = true;

    }

    bool submit = false;
    public void AttackEnter()
    {
        //submitmenu.SetActive(false);
        submitmenu.SetActive(false);
        
        //enemyTurn = enemyTurnPhase.move;
        //playerTurn = false;
        
        //StartCoroutine(CheckEnemyDMG());
        submit = true;
    }

    bool back = false;
    public void Back()
    {
        Debug.Log("BACK MAIN");
        //move menu
        if (submitmenu.activeSelf == true)
        {
            Debug.Log("BACK: " + back);
           
            submitmenu.SetActive(false);
            attackmenu.SetActive(true);
            moveselected = false;

            ResetTileColor();

            
        }
        //attack menu
        if(attackmenu.activeSelf == true)
        {
            
            
            attackmenu.SetActive(false);
            elementmenu.SetActive(true);
            return;
        }
        if (elementmenu.activeSelf == true)
        {
            //back = false;
            elementmenu.SetActive(false);
            combatmenu.SetActive(true);
            
            return;
        }
        switch (currentOpt)
        {
            case combatOptions.move:
                movemenu.SetActive(false);
                combatmenu.SetActive(!combatmenu.activeSelf);
                
                break;
            case combatOptions.attack:
                elementmenu.SetActive(false);
                combatmenu.SetActive(!combatmenu.activeSelf);

                break;
            case combatOptions.flee:
                fleemenu.SetActive(false);
                combatmenu.SetActive(!combatmenu.activeSelf);
                
                break;
            case combatOptions.none:
                break;
            default:
                break;
        }
        return;
    }

    private enum enemyTurnPhase 
    {
        move,attack, none
    }
    enemyTurnPhase enemyTurn;
    void Update()
    {
        if(player_lvl >= PlayerStats.lvl)
        {
            PlayerStats.lvl = player_lvl;
        }
        if(combatmenu.activeSelf == true && currentOpt != combatOptions.move)
        {
            //num_moves = 3;
        }

        //SET BACK ALSO CHECK CheckEnemyDmg

        //Debug.Log("Player LVL: " + player_lvl);
        /* if (player_lvl == 1)
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
         if (player_lvl >= 6)
         {
             elements[0].SetActive(true);
             elements[1].SetActive(true);
             elements[2].SetActive(true);
             elements[3].SetActive(true);
             elements[4].SetActive(true);
         }*/
        
        switch (currentOpt)
        {
            case combatOptions.move:
                if (move == true)
                {
                    movemenu.SetActive(false);
                    currentOpt = combatOptions.enemy;
                    move = false;
                    num_moves = 3;
                }
                if (num_moves >= 0 && move == false && currentOpt == combatOptions.move)
                {
                    PlayerMove();

                }

                break;
            case combatOptions.attack:
                bool c1 = moveselected && submit == false;
                if (c1)
                {
                    if (originalidx == -1)
                    {
                        originalidx = currentPlayerTileIndex;
                    }
                    Debug.Log(player_dir);
                    if(player_dir == Dir.none)
                    {
                        player_dir = Dir.left;
                    }
                    AttackSpaceMove(atk_prop.attackColor);
                    
                }
                else if(c1 == false && submit == true)
                {
                    CheckEnemyDMG();
                    //player_attacks = ElementAttacks.none;
                    //currentOpt = combatOptions.enemy;
                }
                break;
            case combatOptions.flee:
                break;
            case combatOptions.none:
                
                break;
            case combatOptions.enemy:

                Debug.Log("ENEMY PHASE");

                //movemenu.SetActive(false);
                if (Enemies.Count == 0)
                {
                    break;
                }
                else if (enemyTurn == enemyTurnPhase.move)
                {
                    
                    
                    EnemyMove();
                    //StartCoroutine(EnemyMove());
                    enemyTurn = enemyTurnPhase.attack;
                }
                else if (enemyTurn == enemyTurnPhase.attack)
                {

                    EnemyAttack();
                    //StartCoroutine(EnemyAttack());
                    enemyTurn = enemyTurnPhase.none;
                    
                    if (enemy_atk == true && enemy_Move == true)
                    {
                        playerTurn = true;
                        //currentOpt = combatOptions.none;
                        

                        originalidx = -1;
                        //ResetTileSpace();

                        submit = false;
                        dmgcheck = false;
                        enemy_Move = false;
                        enemy_atk = false;
                        moveselected = false;
                        dmgcheck = false;
                        
                        currentOpt = combatOptions.none;
                    }

                }

                combatmenu.SetActive(true);
                
                
                break;
        }

        
            
            

        
    }

    bool dmgcheck = false;
    void CheckEnemyDMG()
    {
        player_dir = Dir.none;
        if(submit == false)
        {
            return;
        }
       // yield return null;
        Debug.Log("Checking dmg");

        var delete = -1;
        List<Entity> Enemy_e = Enemies;
      
        for(var i = 0; i < Enemies.Count; i++)
        {

            var enemy = Enemies[i];
            //Debug.Log(i);
            

            if(enemy.self != null)
            {
                
                for (var u = 0; u < Tiles_e.Count; u++)
                {
                    var tile_e = Tiles_e[u];
                    var tile = tile_e.self;

                    if (GameObject.ReferenceEquals(enemy.current_tile, tile_e.self))
                    {

                        if (attackBy[u] == "Player")
                        {
                            //SET BACK
                            //enemyHealth[i]--;
                            enemyHealth[i] = 0;
                            Debug.Log("ENEMY TAKING DMG: " + i);
                            Debug.Log(enemyHealth[i]);
                            if (enemyHealth[i] <= 0)
                            {
                                delete = i;                
                                xp_currentlvl += 25.0f;
                                //Debug.Log("XP Gain: " + xp_currentlvl);
                                if (xp_currentlvl >= xp_nextlvl)
                                {
                                    player_lvl++;
                                    PlayerStats.lvl = player_lvl;
                                    xp_currentlvl = 0.0f;
                                }

                            }
                            //attackBy[u] = "empty";
                            tile_e.self.GetComponent<SpriteRenderer>().color = original_tile;
                            u = Tiles_e.Count;
                        }
                    }
                    
                    //tile_e.self.GetComponent<SpriteRenderer>().color = original_tile;
                   // u++;
                    
                }
            }
        }
        if(delete != -1)
        {
            //Debug.Log("ENEMY KILLED");
            Destroy(Enemies[delete].self);
            enemyHealth.RemoveAt(delete);
            Enemies.RemoveAt(delete);
            num_enemies--;

        }
        if(Enemies.Count <= 0 || currentHealth <= 0)
        {
            EndBattle();
        }
        
        
        //combatmenu.SetActive(true);
        dmgcheck = true;
        if (dmgcheck && submit)
        {
            combatmenu.SetActive(true);
            submit = false;
        }
    }

    void AttackSpaceMove(Color c)
    {
        
        if (submitmenu.activeSelf == false)
        {
            
            return;
        }
        Debug.Log("ATK SP");
        
        GameObject t;
        int moveidx = currentPlayerTileIndex;
        if (Input.GetKeyDown(KeyCode.S))
        {
            if (moveidx - 7 >= 0) //S
            {
                ResetTileColor();
                moveidx = currentPlayerTileIndex - 7;
                player_dir = Dir.down;

            }

        }
        if (Input.GetKeyDown(KeyCode.W))
        {
            if (moveidx + 7 <= tiles.Length) //N
            {
                ResetTileColor();
                moveidx = currentPlayerTileIndex + 7;
                player_dir = Dir.up;


            }

        }
        if (Input.GetKeyDown(KeyCode.D))
        {
            if (moveidx - 1 >= 0) //E
            {
                ResetTileColor();
                moveidx = currentPlayerTileIndex - 1;
                player_dir = Dir.right;


            }

        }
        if (Input.GetKeyDown(KeyCode.A))
        {
            if (moveidx + 1 <= tiles.Length) //W
            {
                ResetTileColor();
                moveidx = currentPlayerTileIndex + 1;
                player_dir = Dir.left;


            }
        }
        // Debug.Log(player_attacks);

        switch (player_dir)
        {
            case Dir.left:
                Attack(c, moveidx);
                // player_dir = Dir.none;
                break;
            case Dir.right:
                Attack(c, moveidx);
                // player_dir = Dir.none;
                break;
            case Dir.up:
                Attack(c, moveidx);
                // player_dir = Dir.none;
                break;
            case Dir.down:
                Attack(c, moveidx);
                //player_dir = Dir.none;
                break;
            case Dir.none:

                break;

            default:
                
              
                Attack(c, currentPlayerTileIndex + 1);
                // player_dir = Dir.none;
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
                if (currentPlayerTileIndex + 1 < tiles.Length) //W
                {

                    t = tiles[currentPlayerTileIndex + 1];
                    Tiles_e[currentPlayerTileIndex + 1] = new Entity(t, currentPlayerTileIndex + 1, "Player");
                    Tiles_e[currentPlayerTileIndex + 1].self.GetComponent<SpriteRenderer>().color = c;
                    attackBy[currentPlayerTileIndex + 1] = "Player";
                    space[currentPlayerTileIndex + 1] = currentPlayerTileIndex + 1;

                }
                if (currentPlayerTileIndex - 1 >= 0)
                {

                    t = tiles[currentPlayerTileIndex - 1];
                    Tiles_e[currentPlayerTileIndex - 1] = new Entity(t, currentPlayerTileIndex - 1, "Player");
                    Tiles_e[currentPlayerTileIndex - 1].self.GetComponent<SpriteRenderer>().color = c;
                    attackBy[currentPlayerTileIndex - 1] = "Player";
                    space[currentPlayerTileIndex - 1] = currentPlayerTileIndex - 1;
                }
                //up 
                if (currentPlayerTileIndex + 7 < tiles.Length) //W
                {

                    t = tiles[currentPlayerTileIndex + 7];
                    Tiles_e[currentPlayerTileIndex + 7] = new Entity(t, currentPlayerTileIndex + 7, "Player");
                    Tiles_e[currentPlayerTileIndex + 7].self.GetComponent<SpriteRenderer>().color = c;
                    attackBy[currentPlayerTileIndex + 7] = "Player";
                    space[currentPlayerTileIndex + 7] = currentPlayerTileIndex + 7;
                }
                if (currentPlayerTileIndex - 7 >= 0)
                {

                    t = tiles[currentPlayerTileIndex - 7];
                    Tiles_e[currentPlayerTileIndex - 7] = new Entity(t, currentPlayerTileIndex - 7, "Player");
                    Tiles_e[currentPlayerTileIndex - 7].self.GetComponent<SpriteRenderer>().color = c;
                    attackBy[currentPlayerTileIndex - 7] = "Player";
                    space[currentPlayerTileIndex - 7] = currentPlayerTileIndex - 7;
                }
                break;
            case ElementAttacks.Ember:
                //Debug.Log("Ember");

                //left
                if (diff == 0 && player_dir == Dir.left)
                {
                    if (moveidx + 1 < tiles.Length) //W
                    {

                        t = tiles[moveidx + 1];
                        Tiles_e[moveidx + 1] = new Entity(t, moveidx + 1, "Player");
                        Tiles_e[moveidx + 1].self.GetComponent<SpriteRenderer>().color = c;
                        attackBy[moveidx + 1] = "Player";
                        space[moveidx + 1] = moveidx + 1;
                    }
                    if (moveidx + 2 < tiles.Length) //W
                    {

                        t = tiles[moveidx + 2];
                        Tiles_e[moveidx + 2] = new Entity(t, moveidx + 2, "Player");
                        Tiles_e[moveidx + 2].self.GetComponent<SpriteRenderer>().color = c;
                        moveidx = currentPlayerTileIndex;
                        attackBy[moveidx + 2] = "Player";
                        space[moveidx + 2] = moveidx + 2;
                    }

                }
                if (diff == -1 && player_dir == Dir.right)
                {
                    if (moveidx >= 0)
                    {

                        t = tiles[moveidx];
                        Tiles_e[moveidx] = new Entity(t, moveidx, "Player");
                        Tiles_e[moveidx].self.GetComponent<SpriteRenderer>().color = c;
                        attackBy[moveidx] = "Player";
                        space[moveidx] = moveidx;
                    }
                    if (moveidx - 1 >= 0)
                    {

                        t = tiles[moveidx - 1];
                        Tiles_e[moveidx - 1] = new Entity(t, moveidx - 1, "Player");
                        Tiles_e[moveidx - 1].self.GetComponent<SpriteRenderer>().color = c;
                        attackBy[moveidx - 1] = "Player";
                        space[moveidx - 1] = moveidx - 1;
                    }

                }
                //up 
                if (diff == 7 && player_dir == Dir.up)
                {
                    if (moveidx < tiles.Length) //W
                    {

                        t = tiles[moveidx];
                        Tiles_e[moveidx] = new Entity(t, moveidx, "Player");
                        Tiles_e[moveidx].self.GetComponent<SpriteRenderer>().color = c;
                        attackBy[moveidx] = "Player";
                        space[moveidx] = moveidx;

                    }
                    if (moveidx + 7 < tiles.Length) //W
                    {

                        t = tiles[moveidx + 7];
                        Tiles_e[moveidx + 7] = new Entity(t, moveidx + 7, "Player");
                        Tiles_e[moveidx + 7].self.GetComponent<SpriteRenderer>().color = c;
                        attackBy[moveidx + 7] = "Player";
                        space[moveidx + 7] = moveidx + 7;
                    }

                }
                if (diff == -7 && player_dir == Dir.down)
                {
                    if (moveidx >= 0)
                    {

                        t = tiles[moveidx];
                        Tiles_e[moveidx] = new Entity(t, moveidx, "Player");
                        Tiles_e[moveidx].self.GetComponent<SpriteRenderer>().color = c;
                        attackBy[moveidx] = "Player";
                        space[moveidx] = moveidx;
                    }
                    if (moveidx - 7 >= 0)
                    {

                        t = tiles[moveidx - 7];
                        Tiles_e[moveidx - 7] = new Entity(t, moveidx - 7, "Player");
                        Tiles_e[moveidx - 7].self.GetComponent<SpriteRenderer>().color = c;
                        attackBy[moveidx - 7] = "Player";
                        space[moveidx - 7] = moveidx - 7;
                    }

                }
                break;
            case ElementAttacks.Douse:
                //left
                if (diff == 0 && player_dir == Dir.left)
                {
                    if (moveidx + 1 < tiles.Length) //W
                    {

                        t = tiles[moveidx + 1];
                        Tiles_e[moveidx + 1] = new Entity(t, moveidx + 1, "Player");
                        Tiles_e[moveidx + 1].self.GetComponent<SpriteRenderer>().color = c;
                        attackBy[moveidx + 1] = "Player";
                        space[moveidx + 1] = moveidx + 1;

                    }
                    if (moveidx + 2 < tiles.Length) //W
                    {

                        t = tiles[moveidx + 2];
                        Tiles_e[moveidx + 2] = new Entity(t, moveidx + 2, "Player");
                        Tiles_e[moveidx + 2].self.GetComponent<SpriteRenderer>().color = c;
                        attackBy[moveidx + 2] = "Player";
                        space[moveidx + 2] = moveidx + 2;
                        moveidx += 2;
                        if (moveidx + 7 < tiles.Length)
                        {

                            t = tiles[moveidx + 7];
                            Tiles_e[moveidx + 7] = new Entity(t, moveidx + 7, "Player");
                            Tiles_e[moveidx + 7].self.GetComponent<SpriteRenderer>().color = c;
                            attackBy[moveidx + 7] = "Player";
                            space[moveidx + 7] = moveidx + 7;

                        }
                        if (moveidx - 7 >= 0)
                        {

                            t = tiles[moveidx - 7];
                            Tiles_e[moveidx - 7] = new Entity(t, moveidx - 7, "Player");
                            Tiles_e[moveidx - 7].self.GetComponent<SpriteRenderer>().color = c;
                            attackBy[moveidx - 7] = "Player";
                            space[moveidx - 7] = moveidx - 7;

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
                        attackBy[moveidx] = "Player";
                        space[moveidx] = moveidx;
                    }
                    if (moveidx - 1 >= 0)
                    {

                        t = tiles[moveidx - 1];
                        Tiles_e[moveidx - 1] = new Entity(t, moveidx - 1, "Player");
                        Tiles_e[moveidx - 1].self.GetComponent<SpriteRenderer>().color = c;
                        attackBy[moveidx - 1] = "Player";
                        space[moveidx - 1] = moveidx - 1;
                        moveidx -= 1;
                        if (moveidx + 7 < tiles.Length)
                        {

                            t = tiles[moveidx + 7];
                            Tiles_e[moveidx + 7] = new Entity(t, moveidx + 7, "Player");
                            Tiles_e[moveidx + 7].self.GetComponent<SpriteRenderer>().color = c;
                            attackBy[moveidx + 7] = "Player";
                            space[moveidx + 7] = moveidx + 7;

                        }
                        if (moveidx - 7 >= 0)
                        {

                            t = tiles[moveidx - 7];
                            Tiles_e[moveidx - 7] = new Entity(t, moveidx - 7, "Player");
                            Tiles_e[moveidx - 7].self.GetComponent<SpriteRenderer>().color = c;
                            attackBy[moveidx - 7] = "Player";
                            space[moveidx - 7] = moveidx - 7;

                        }
                    }

                }
                //up 
                if (diff == 7 && player_dir == Dir.up)
                {
                    if (moveidx < tiles.Length)
                    {

                        t = tiles[moveidx];
                        Tiles_e[moveidx] = new Entity(t, moveidx, "Player");
                        Tiles_e[moveidx].self.GetComponent<SpriteRenderer>().color = c;
                        attackBy[moveidx] = "Player";
                        space[moveidx] = moveidx;

                    }
                    if (moveidx + 7 < tiles.Length)
                    {

                        t = tiles[moveidx + 7];
                        Tiles_e[moveidx + 7] = new Entity(t, moveidx + 7, "Player");
                        Tiles_e[moveidx + 7].self.GetComponent<SpriteRenderer>().color = c;
                        attackBy[moveidx + 7] = "Player";
                        space[moveidx + 7] = moveidx + 7;
                        moveidx += 7;
                        if (moveidx + 1 < tiles.Length)
                        {

                            t = tiles[moveidx + 1];
                            Tiles_e[moveidx + 1] = new Entity(t, moveidx + 1, "Player");
                            Tiles_e[moveidx + 1].self.GetComponent<SpriteRenderer>().color = c;
                            attackBy[moveidx + 1] = "Player";
                            space[moveidx + 1] = moveidx + 1;

                        }
                        if (moveidx - 1 < tiles.Length)
                        {

                            t = tiles[moveidx - 1];
                            Tiles_e[moveidx - 1] = new Entity(t, moveidx - 1, "Player");
                            Tiles_e[moveidx - 1].self.GetComponent<SpriteRenderer>().color = c;
                            attackBy[moveidx - 1] = "Player";
                            space[moveidx - 1] = moveidx - 1;

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
                        attackBy[moveidx] = "Player";
                        space[moveidx] = moveidx;
                    }
                    if (moveidx - 7 >= 0)
                    {

                        t = tiles[moveidx - 7];
                        Tiles_e[moveidx - 7] = new Entity(t, moveidx - 7, "Player");
                        Tiles_e[moveidx - 7].self.GetComponent<SpriteRenderer>().color = c;
                        attackBy[moveidx - 7] = "Player";
                        space[moveidx - 7] = moveidx - 7;
                        moveidx -= 7;
                        if (moveidx + 1 < tiles.Length)
                        {

                            t = tiles[moveidx + 1];
                            Tiles_e[moveidx + 1] = new Entity(t, moveidx + 1, "Player");
                            Tiles_e[moveidx + 1].self.GetComponent<SpriteRenderer>().color = c;
                            attackBy[moveidx + 1] = "Player";
                            space[moveidx + 1] = moveidx + 1;

                        }
                        if (moveidx - 1 < tiles.Length)
                        {

                            t = tiles[moveidx - 1];
                            Tiles_e[moveidx - 1] = new Entity(t, moveidx - 1, "Player");
                            Tiles_e[moveidx - 1].self.GetComponent<SpriteRenderer>().color = c;
                            attackBy[moveidx - 1] = "Player";
                            space[moveidx - 1] = moveidx - 1;

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
                attackBy[currentPlayerTileIndex] = "Player";
                space[currentPlayerTileIndex] = currentPlayerTileIndex;
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

        if (back == true && submitmenu.activeSelf == true
                && attackmenu.activeSelf == false)
        {

            back = false;
        }
    }
    void ResetTileSpace()
    {
        int u = 0;
        foreach (var item in Tiles_e)
        {
            attackBy[u] = "empty";
            
            u++;

        }
    }

    bool moving = false;
    void PlayerMove()
    {

        if(movemenu.activeSelf == false)
        {
            return;
        }
        //StartCoroutine(Movement(currentPlayerTileIndex, Player));

        if (moving && playerTurn == true)
        {
            StartCoroutine(Movement(currentPlayerTileIndex,Player));
        }
        else if (num_moves > 0 && playerTurn == true)
        {
          

            if (Input.GetKeyDown(KeyCode.S))
            {
                if (currentPlayerTileIndex - 7 >= 0)
                {
                    currentPlayerTileIndex = currentPlayerTileIndex - 7;
                    moving = true;
                    num_moves--;
                }
               /* if (currentPlayerTileIndex - tiles.Length / 4 >= 0)
                {
                    currentPlayerTileIndex = currentPlayerTileIndex - tiles.Length / 4;
                    moving = true;
                    num_moves--;
                }*/
            }
            else if (Input.GetKeyDown(KeyCode.A))
            {
                if (currentPlayerTileIndex + 1 <= tiles.Length)
                {
                    currentPlayerTileIndex = currentPlayerTileIndex + 1;
                    moving = true;
                    num_moves--;
                }
                
                /* if ((currentPlayerTileIndex + 1) % 7 != 0)
                 {
                     currentPlayerTileIndex = currentPlayerTileIndex + 1;
                     moving = true;
                     num_moves--;
                 }*/
            }
            else if (Input.GetKeyDown(KeyCode.W))
            {
                if (currentPlayerTileIndex + 7 <= tiles.Length)
                {
                    currentPlayerTileIndex = currentPlayerTileIndex + 7;
                    moving = true;
                    num_moves--;
                }
                /*if (currentPlayerTileIndex + tiles.Length / 4 <= tiles.Length)
                {
                    currentPlayerTileIndex = currentPlayerTileIndex + tiles.Length / 4;
                    moving = true;
                    num_moves--;
                }*/
            }
            else if (Input.GetKeyDown(KeyCode.D))
            {
                if (currentPlayerTileIndex - 1 >= 0)
                {
                    currentPlayerTileIndex = currentPlayerTileIndex - 1;
                    moving = true;
                    num_moves--;
                }
                /*if ((currentPlayerTileIndex) % 7 != 0)
                {
                    currentPlayerTileIndex = currentPlayerTileIndex - 1;
                    moving = true;
                    num_moves--;
                }*/
            }
            
        }
        else if (num_moves <= 0 || playerTurn == false) {

            //movemenu.SetActive(false);
            //currentOpt = combatOptions.enemy;
            //MoveEnter();
            move = true;
        }
        
        
    }

    IEnumerator Movement(int index,GameObject entity) 
    {
        
        if (tiles[index].transform.position != entity.transform.position)
        {
            entity.transform.position = Vector3.Lerp(entity.transform.position, tiles[index].transform.position, 1f);
            if ((entity.transform.position - tiles[index].transform.position).magnitude <= 0.5f)
            {
                entity.transform.position = tiles[index].transform.position;
                moving = false;
                var n_p = entity.transform.position;
                n_p.y = -2;
                Player.transform.position = n_p;
            }
        }
        //Debug.Log((entity.transform.position - tiles[index].transform.position).magnitude);
        yield return new WaitForSeconds(1);
    }


    private enum EnemyMoveDir 
    {
        none,left,right,up,down,start
    }
    private int enemy_moves = 1;
    bool enemy_Move = false;
    void EnemyMove()
    {
        
        for (int i = 0; i < num_enemies; i++) 
        {
            Entity enemy = Enemies[i];//can't use for each throws error
            EnemyMoveDir dir = EnemyMoveDir.start;
            int movesLeft = enemy_moves;
            
            while (movesLeft > 0 && dir != EnemyMoveDir.none && enemy.self != null)
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
                            ////Debug.Log(enemy.tileIndex);
                            enemy.self.transform.position = tiles[enemy.tileIndex].transform.position;
                            Enemies[i] = new Entity(enemy.self, enemy.health, tiles[enemy.tileIndex], enemy.tileIndex);
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
                            ////Debug.Log(enemy.tileIndex);
                            enemy.self.transform.position = tiles[enemy.tileIndex].transform.position;
                            Enemies[i] = new Entity(enemy.self, enemy.health, tiles[enemy.tileIndex], enemy.tileIndex);
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
                            ////Debug.Log(enemy.tileIndex);
                            enemy.self.transform.position = tiles[enemy.tileIndex].transform.position;
                            Enemies[i] = new Entity(enemy.self, enemy.health, tiles[enemy.tileIndex], enemy.tileIndex);
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
                            ////Debug.Log(enemy.tileIndex);
                            enemy.self.transform.position = tiles[enemy.tileIndex].transform.position;
                            Enemies[i] = new Entity(enemy.self, enemy.health, tiles[enemy.tileIndex], enemy.tileIndex);
                            movesLeft--;
                        }
                        else { dir = EnemyMoveDir.none; }
                    }
                }
            }
            if(enemy.self != null)
            {
                v3 e_p = enemy.self.transform.position;
                e_p.y = -2f;
                enemy.self.transform.position = e_p;
            }
            
        }
        enemy_Move = true;
    }

    bool enemy_atk = false;
    void EnemyAttack()
    {


        // //Debug.Log("Enemy Attacking");
        //int eidx = 0;
        for (int u = 0; u < Enemies.Count; u++)
        {
            Entity enemy = Enemies[u];
            //goal find the rough player pos in order to execute an attack
            //does this by figuring out if player is above, below, left or right
            //then executing an attack in that range
            //uses two EnemyMoveDir(reused from enemy move) values to get exact dir(only one doesn't get diagnols)
            EnemyMoveDir dirUpDown = EnemyMoveDir.start;
            if (enemy.self.transform.position.z < Player.transform.position.z)
            {
                dirUpDown = EnemyMoveDir.up;
            }
            else if (enemy.self.transform.position.z > Player.transform.position.z)
            {
                dirUpDown = EnemyMoveDir.down;
            }
            else 
            {
                dirUpDown = EnemyMoveDir.none;
            }

            EnemyMoveDir dirLeftRight = EnemyMoveDir.start;
            if (enemy.self.transform.position.x > Player.transform.position.x)
            {
                dirLeftRight = EnemyMoveDir.left;
            }
            else if (enemy.self.transform.position.x < Player.transform.position.x)
            {
                dirLeftRight = EnemyMoveDir.right;
            }
            else
            {
                dirLeftRight = EnemyMoveDir.none;//just in case lands on same spot
            }

            if (Enemies[u].self == null)
            {
                continue;
            }


            //dirUPDown/////////////////////////////////////////////////////////////////////////////////////
            if (dirUpDown == EnemyMoveDir.down)//right must be valid since player is down
            {
                //no check for down since player must already be below enemy(assuming other code is good)
                int tileIndex= enemy.tileIndex - tiles.Length / 4;
                tileCheck(tileIndex,enemy);
            }
            else if (dirUpDown == EnemyMoveDir.up)//right must be valid since player is to left
            {
                //no check for down since player must already be above enemy(assuming other code is good)
                int tileIndex = enemy.tileIndex + tiles.Length / 4;
                tileCheck(tileIndex, enemy);
            }
            else if (dirUpDown == EnemyMoveDir.none) {
                //do the check since don't know// always default to down first
                if (enemy.tileIndex - tiles.Length / 4 >= 0)
                {
                    int tileIndex = enemy.tileIndex - tiles.Length / 4;
                    dirUpDown = EnemyMoveDir.down;
                    tileCheck(tileIndex, enemy);
                }
                else //if down doesn't work up must
                {
                    int tileIndex = enemy.tileIndex + tiles.Length / 4;
                    dirUpDown = EnemyMoveDir.up;
                    tileCheck(tileIndex, enemy);
                }
            }

            //dirLeftRight/////////////////////////////////////////////////////////////////////////////////////
            if (dirLeftRight == EnemyMoveDir.left)//right must be valid since player is down
            {
                //no check for down since player must already be below enemy(assuming other code is good)
                int tileIndex = enemy.tileIndex +1;
                tileCheck(tileIndex, enemy);
            }
            else if (dirLeftRight == EnemyMoveDir.right)//right must be valid since player is to left
            {
                //no check for down since player must already be above enemy(assuming other code is good)
                int tileIndex = enemy.tileIndex -1 ;
                tileCheck(tileIndex, enemy);
            }
            else if (dirLeftRight == EnemyMoveDir.none)
            {
                //do the check since don't know// always default to left first
                if ((enemy.tileIndex + 1) % 7 != 0)
                {
                    int tileIndex = enemy.tileIndex +1;
                    dirLeftRight = EnemyMoveDir.left;
                    tileCheck(tileIndex, enemy);
                }
                else //if down doesn't work right must
                {
                    int tileIndex = enemy.tileIndex-1;
                    dirLeftRight = EnemyMoveDir.right;
                    tileCheck(tileIndex, enemy);
                }
            }

            //diagnols//////////////////////////////////////////////////////////////////(check both)
            if(dirUpDown==EnemyMoveDir.down&&dirLeftRight== EnemyMoveDir.left) {
                //no check since both directions must be valid do to previous calc.
                int tileIndex = enemy.tileIndex - tiles.Length / 4 +1;
                tileCheck(tileIndex, enemy);
            }
            else if (dirUpDown == EnemyMoveDir.down && dirLeftRight == EnemyMoveDir.right) 
            {
                //no check since both directions must be valid
                int tileIndex = enemy.tileIndex - tiles.Length / 4 - 1;
                tileCheck(tileIndex, enemy);
            }
            else if (dirUpDown == EnemyMoveDir.up && dirLeftRight == EnemyMoveDir.left)
            {
                int tileIndex = enemy.tileIndex + tiles.Length / 4+1;
                tileCheck(tileIndex, enemy);
            }
            else if (dirUpDown == EnemyMoveDir.up && dirLeftRight == EnemyMoveDir.right)
            {
                int tileIndex = enemy.tileIndex + tiles.Length / 4 - 1;
                tileCheck(tileIndex, enemy);
            }

            
        }

        enemy_atk = true;
    }

    void tileCheck(int index, Entity enemy) {
        Tiles_e[index].self.GetComponent<SpriteRenderer>().color = enemy.self.GetComponent<SpriteRenderer>().color;
        Tiles_e[index] = new Entity(Tiles_e[index].self, index, "Enemy");
        attackBy[index] = "Enemy";
        if (currentPlayerTileIndex==index)
        {
            //Debug.Log("Player Hit:");
            //Player_e.SubtractHealth();
            currentHealth -= 10;
            healthBar.SetHealth(currentHealth);
            attackBy[index] = "empty";
            if (currentHealth <= 0)
            {
                //Debug.Log("Player is dead");
                EndBattle();
                
            }
            //Debug.Log(currentHealth);  
        }
    }

    bool canEnd = false;
    void EndBattle()
    {
        if (canEnd == true)
            return;

        //Debug.Log("End of Battle");
        playerTurn = false;
        PlayerStats.lvl = player_lvl;
        elementmenu.SetActive(false);
        movemenu.SetActive(false);
        fleemenu.SetActive(false);
        attackmenu.SetActive(false);
        player_attacks = ElementAttacks.none;
        player_dir = Dir.none;
        for (var i = 0; i < Enemies.Count; i++)
        {
            if (Enemies[i].self != null)
            {
                Destroy(Enemies[i].self);
            }

        }
        Enemies.Clear();
        enemyHealth.Clear();
        ResetTileSpace();
        ResetTileColor();
        //Debug.Log(SceneManager.GetActiveScene().name);
        Scene ow = SceneManager.GetActiveScene();
       
        if (currentHealth <= 0)
        {
            Player_e.self.SetActive(false);
            SceneManager.LoadScene("EndScene");
        }
        else if (currentHealth > 0)
        {
            
            SceneManager.LoadScene("OverworldScene");
            canEnd = true;
        }
        
        
    }


}

// Update is called once per frame


