
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
    //structs for holding data in the context of this scene //_e generally refers to entity 
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
       
    }
    //attack properites//holds color values for floor attacks. 
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
    // player ttack properties
    AttackProperties atk_prop;
   
    int num_moves = 3;//note must change value in loop as well
    float tileSize = 1;
    //player health
    public HealthBar healthBar;
    public int maxHealth = 100;
    public int currentHealth;                             // health bar stuff

    //player obj
    public GameObject Player;
    //controls the tile index for player
    private int currentPlayerTileIndex;
    
    //enemy game object
    public GameObject Enemy;
    
    //list of all game object tiles 
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
    //array of elements to pick from 
    public GameObject[] elements;
    //determines player attack move selection 
    bool playerATKMoveSelection = false;
   
    //number of enemies to spawn
    int num_enemies = 2;
    //original player tile index
    int originalidx = -1;
    //default tile color
    Color original_tile;
    //determines player turn 
    bool playerTurn = false;
    //player stats
    int player_lvl = 1;
    float xp_currentlvl = 0.0f;
    float xp_nextlvl = 100.0f;

    //enemy list for number of enemies and their health
    List<Entity> Enemies;
    List<int> enemyHealth;
    //list of all tiles that recived attacks by player or enemy
    List<string> attackBy;
    //entity holding tile info 
    List<Entity> Tiles_e;
    
    //player entity 
    Entity Player_e;
    //game start value 
    bool gameStart = false;
    //combat option enums for player state 
    public enum combatOptions
    {
        move, attack, flee, none, enemy
    }
    //element enums for attacks
    public enum ElementAttacks
    {
        Quake, Ember, Douse, Bind, Harden, none
    }

    //player direction enums for input direction 
    public enum Dir
    {
        left, right, up, down, none
    }
    private enum enemyTurnPhase
    {
        move, attack, none
    }
    
    //enums
    combatOptions currentOpt;
    ElementAttacks player_attacks;
    Dir player_dir;
    enemyTurnPhase enemyTurn;

    //audio
    public List<AudioClip> combatClips;
    private AudioSource audio;

    void Awake()
    {
        //instantiate our lists for keeping track
        attackBy = new List<string>();
        enemyHealth = new List<int>();
        Tiles_e = new List<Entity>();
        Enemies = new List<Entity>();
        //finding all gameobjects in scene with tile
        tiles = GameObject.FindGameObjectsWithTag("tile");
        //counter for renaming them in order
        int c = 0;
        foreach (var tile in tiles)
        {
            //adding a new entity with no attack space by anyone 
            Tiles_e.Add(new Entity(tile, c, ""));
            //labeling attack by as empty for start
            attackBy.Add("empty");
            
            c++;
        }
        //enums
        player_attacks = ElementAttacks.none;
        player_dir = Dir.none;
        currentOpt = combatOptions.none;
    }

    void Start()
    {
        //setting player health full
        currentHealth = maxHealth;
        healthBar.SetMaxHealth(maxHealth);               // health bar stuff

        //assign audio source
        audio = GetComponent<AudioSource>();

        //start tile
        GameObject starttile = tiles[0];
        currentPlayerTileIndex = 0; //inital index
        
        //fixing height for player position 
        var n_p = starttile.transform.position;
        n_p.y = -2;
        Player.transform.position = n_p;

        //original tile color 
        original_tile = starttile.GetComponent<SpriteRenderer>().color;

        //player goes first
        playerTurn = true;
        // creating player entity 
        Player_e = new Entity(Player, currentHealth, starttile, 0);
        
        //creating enemies 
        for(int i = 0; i < num_enemies; i++)
        {
            //random location to spawn 
            var tile_range = (int)UnityEngine.Random.Range(0, tiles.Length-1);
            GameObject enemy = Instantiate(Enemy);
            //adjust height 
            v3 e_p = tiles[tile_range].transform.position;
            e_p.y = -2f;
            enemy.transform.position = e_p;
            //new enemy entities
            Enemies.Add(new Entity(enemy, 3, tiles[tile_range], tile_range));
            enemyHealth.Add(3); //basic health is 3
            
            //random color to choose from 
            var rand = (int)UnityEngine.Random.Range(0, 5);
            var renderer = enemy.GetComponent<SpriteRenderer>();
            //each element
            switch (rand)
            {
                case 0:
                    renderer.color = Color.green;
                    
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



        // if have done everything else 
        if (playerTurn == true)
        {
            //gui's off at start 
            elementmenu.SetActive(false);
            movemenu.SetActive(false);
            fleemenu.SetActive(false);
            attackmenu.SetActive(false);
            submitmenu.SetActive(false);
            two.SetActive(false);
            
        }
        if (player_lvl < PlayerStats.lvl)
        {
            player_lvl = PlayerStats.lvl;
        }
        //Application.targetFrameRate = 30;
        gameStart = true;
    }


    //from button in attack menu 
    public void MenuOptionAttacks(GameObject obj)
    {
        //hide attack menu 
        attackmenu.SetActive(false);
        //show submit button
        submitmenu.SetActive(true);
        //can move
        playerATKMoveSelection = true;

       

        //based on button name
        switch (obj.GetComponentInChildren<Text>().text)
        {
            case "Quake":
                //play sound
                audio.PlayOneShot(combatClips[5]);

                // we set the color
                atk_prop.attackColor = Color.green;
                //choose the attack
                player_attacks = ElementAttacks.Quake;
                Debug.Log(player_attacks);
                break;
            case "Ember":
                //play sound
                audio.PlayOneShot(combatClips[4]);

                atk_prop.attackColor = Color.red;
                player_attacks = ElementAttacks.Ember;
                Debug.Log(player_attacks);
                break;
            case "Douse":
                //play sound
                audio.PlayOneShot(combatClips[7]);

                atk_prop.attackColor = Color.blue;
                player_attacks = ElementAttacks.Douse;
                Debug.Log(player_attacks);
                break;
            case "Bind":
                //play sound
                audio.PlayOneShot(combatClips[8]);

                atk_prop.attackColor = Color.Lerp(Color.yellow, Color.green, 0.75f);
                player_attacks = ElementAttacks.Bind;
                Debug.Log(player_attacks);
                break;
            case "Harden":
                //play sound
                audio.PlayOneShot(combatClips[6]);

                atk_prop.attackColor = Color.grey;
                player_attacks = ElementAttacks.Harden;
                Debug.Log(player_attacks);
                break;
            default:
                break;
        }
    

    }

    //elements from menu option 
    public void MenuAttackByElement(GameObject obj)
    {
        //play sound
        audio.PlayOneShot(combatClips[0]);

        //turning on attack menu 
        attackmenu.SetActive(true);
        //turning off current element menu 
        elementmenu.SetActive(false);
        //checking by obj name 
        switch (obj.name)
        {
            case "Earth":
                

                //creates atta
                atk_prop = new AttackProperties(Color.green);
                
                one.GetComponentInChildren<Text>().text = "Quake";
                

                break;
            case "Fire":
               
                atk_prop = new AttackProperties(Color.red);
                
                one.GetComponentInChildren<Text>().text = "Ember";
                
                
                break;
            case "Water":
                
                atk_prop = new AttackProperties(Color.blue);
                
                one.GetComponentInChildren<Text>().text = "Douse";
                
                
                break;
            case "Wood":
              
                atk_prop = new AttackProperties(Color.Lerp(Color.yellow, Color.green, 0.75f));
                
                one.GetComponentInChildren<Text>().text = "Bind";
             
                
                break;
            case "Metal":
               
                atk_prop = new AttackProperties(Color.grey);
               
                one.GetComponentInChildren<Text>().text = "Harden";
               
                
                break;
            default:
                break;
        }
        //Debug.Log(obj.name);

    }

    //main menu //if we attack
    public void AttackOption()
    {
        //play sound
        audio.PlayOneShot(combatClips[0]);


        //show element sub menu 
        elementmenu.SetActive(true);
        //turn off main
        combatmenu.SetActive(!combatmenu.activeSelf);
        //current option is attack
        currentOpt = combatOptions.attack;
    }

    //move option in main
    public void MoveOption()
    {
        //play sound
        audio.PlayOneShot(combatClips[0]);

        //setting number of moves for player to go
        num_moves = 3;
        movemenu.SetActive(true);
        combatmenu.SetActive(!combatmenu.activeSelf);
        //changing current option 
        currentOpt = combatOptions.move;
    }

    //main menu flee option 
    public void FleeOption()
    {
        //play sound
        audio.PlayOneShot(combatClips[0]);

        fleemenu.SetActive(true);
        combatmenu.SetActive(!combatmenu.activeSelf);
        currentOpt = combatOptions.flee;
    }
    
    //method to flee from fight 
    public void Flee()
    {
        //play sound
        audio.PlayOneShot(combatClips[0]);

        //if we're in flee 
        if (currentOpt == combatOptions.flee)
        {
            //modulous of an even random numbe by the number of enemies 
            var range = (int)UnityEngine.Random.Range(0, Enemies.Count);
            if (range % 2 != 0) //we failed 
            {
                playerTurn = false;
                currentOpt = combatOptions.none;

            }
            else if (range % 2 == 0) //we escaped 
            {
                EndBattle();
            }
        }
    }


    //used to check MoveEnter() click on next frame 
    bool move = false;

    //move menu submit button 
    public void MoveEnter()
    {
        //play sound
        audio.PlayOneShot(combatClips[0]);

        //close menu 
        movemenu.SetActive(false);
        //currentOpt = combatOptions.enemy;
        //enemyTurn = enemyTurnPhase.move;
        //StartCoroutine(CheckEnemyDMG());
        //playerTurn = false;
       // button has been clicked 
        move = true;

    }


    //used to check AttackEnter() click on next frame 
    bool submit = false;

    //attack submit button 
    public void AttackEnter()
    {
        //play sound
        audio.PlayOneShot(combatClips[0]);

        //submitmenu.SetActive(false);
        submitmenu.SetActive(false);
        
        //enemyTurn = enemyTurnPhase.move;
        //playerTurn = false;
        
        //StartCoroutine(CheckEnemyDMG());
        submit = true;
    }

    //used to check back button on next frame
    bool back = false;
    public void Back()
    {
        //play sound
        audio.PlayOneShot(combatClips[0]);

        Debug.Log("BACK MAIN");
        //attack move submit menu 
        if (submitmenu.activeSelf == true)
        {
           // Debug.Log("BACK: " + back);
           //we change menus and turn off player selection 
            submitmenu.SetActive(false);
            attackmenu.SetActive(true);
            playerATKMoveSelection = false;
            
            

        }
        //attack menu //we exit into the main element menu from here and return 
        if(attackmenu.activeSelf == true)
        {
            if (submitmenu.activeSelf == false && elementmenu.activeSelf == false)
            {
               // playerATKMoveSelection = false;
                Debug.Log(playerATKMoveSelection);
                //ResetTileColor();
            }

            attackmenu.SetActive(false);
            elementmenu.SetActive(true);
            return;
        }
        //if we're in element go to main 
        if (elementmenu.activeSelf == true)
        {
            //back = false;
            elementmenu.SetActive(false);
            combatmenu.SetActive(true);
            
            return;
        }
        //if we're in main menu options, just switch 
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
        return;
    }


    
    void Update()
    {
        //once everything has been initialized, gamestart should be true 
        if (gameStart)
        {
            //passing player lvl between overworld and combat scene 
            if (player_lvl >= PlayerStats.lvl)
            {
                PlayerStats.lvl = player_lvl;
            }
            if (combatmenu.activeSelf == true && currentOpt != combatOptions.move)
            {
                //num_moves = 3;
            }

            //SET BACK ALSO CHECK CheckEnemyDmg

            //displays abilities based off of player lvl 
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
             //by the current option afforded to the player 
            switch (currentOpt)
            {
                case combatOptions.move:
                    
                    if (move == true) //if move is true, then we have moved 
                    {
                        movemenu.SetActive(false); //close menu 
                        currentOpt = combatOptions.enemy; //change for enemy turn 
                        //reset move 
                        move = false;
                        num_moves = 3;
                    }
                    //if move is false, then we havent moved yet. 
                    if (num_moves >= 0 && move == false && currentOpt == combatOptions.move)
                    {
                        PlayerMove();

                    }

                    break;
                case combatOptions.attack:
                    //bool statement to check attack and submit 
                    bool c1 = playerATKMoveSelection && submit == false;
                    if (c1)
                    {
                        //stores the original tile index by battl ephase 
                        if (originalidx == -1)
                        {
                            originalidx = currentPlayerTileIndex;
                        }

                        //Debug.Log(player_dir);
                        //default player direction 
                        if (player_dir == Dir.none)
                        {
                            player_dir = Dir.left;
                        }
                        //displays the range of attack and our direction 
                        AttackSpaceMove(atk_prop.attackColor);

                    } //if out statement is false and we hti the submit button, check the dmg
                    else if (c1 == false && submit == true)
                    {
                        CheckEnemyDMG();
                       
                    }
                    break;
                case combatOptions.flee:
                    break;
                case combatOptions.none:

                    break;
                case combatOptions.enemy:
                    //in enemy phase 
                    Debug.Log("ENEMY PHASE");

                    //if no enemies in between frames, just break
                    if (Enemies.Count == 0)
                    {
                        break;
                    } //if the enemy turn is move 
                    else if (enemyTurn == enemyTurnPhase.move)
                    {

                        //moves enemy 
                        EnemyMove();
                        //then the enemy will attack 
                        enemyTurn = enemyTurnPhase.attack;
                    }
                    else if (enemyTurn == enemyTurnPhase.attack)
                    {

                        EnemyAttack();
                        //end enemy phase //switch back to move 
                        enemyTurn = enemyTurnPhase.move;
                        //if we have processed through enemy attack and move then we can reset our values 
                        if (enemy_atk == true && enemy_Move == true)
                        {
                            playerTurn = true;
 
                            originalidx = -1;

                            submit = false;
                            dmgcheck = false;
                            enemy_Move = false;
                            enemy_atk = false;
                            playerATKMoveSelection = false;
                            dmgcheck = false;

                            currentOpt = combatOptions.none;
                        }

                    }
                    //open main menu 
                    combatmenu.SetActive(true);


                    break;
            }





        }

    }

    bool dmgcheck = false;
    void CheckEnemyDMG()
    {
        //reset player direction just in case 
        player_dir = Dir.none;
        //if we didn't hit submit, exit out 
        if(submit == false)
        {
            return;
        }
       
        Debug.Log("Checking dmg");

        //index for destroyign enemy 
        var delete = -1;
        
        //iterate through enemies 
        for(var i = 0; i < Enemies.Count; i++)
        {

            var enemy = Enemies[i];
            //if its not null 
            if(enemy.self != null)
            {
                //check each tile reference with the enemy tile reference to see if they are the same
                for (var u = 0; u < Tiles_e.Count; u++)
                {
                    var tile_e = Tiles_e[u];
                    var tile = tile_e.self;

                    if (GameObject.ReferenceEquals(enemy.current_tile, tile_e.self))
                    {
                        //if the tile space was done by the player 
                        if (attackBy[u] == "Player")
                        {
                            //SET BACK*********
                            //enemyHealth[i]--;*****

                            //enemy takes dmg 
                            enemyHealth[i] = 0;
                            Debug.Log("ENEMY TAKING DMG: " + i);
                            //play sound
                            audio.PlayOneShot(combatClips[1]);
                            Debug.Log(enemyHealth[i]);
                            if (enemyHealth[i] <= 0)
                            {
                                delete = i;          
                                //xp gain 
                                xp_currentlvl += 25.0f;
                                //Debug.Log("XP Gain: " + xp_currentlvl);
                                if (xp_currentlvl >= xp_nextlvl)
                                {
                                    player_lvl++;
                                    PlayerStats.lvl = player_lvl;
                                    xp_currentlvl = 0.0f;
                                }

                            }
                            //tile recolor 
                            tile_e.self.GetComponent<SpriteRenderer>().color = original_tile;
                            u = Tiles_e.Count; //exit portion of sub loop 
                        }
                    }
                    
                    //tile_e.self.GetComponent<SpriteRenderer>().color = original_tile;
                   // u++;
                    
                }
            }
        }
        if(delete != -1)//delete enemy fully 
        {
            //Debug.Log("ENEMY KILLED");
            Destroy(Enemies[delete].self);
            enemyHealth.RemoveAt(delete);
            Enemies.RemoveAt(delete);
            num_enemies--;

        }
        if(Enemies.Count <= 0 || currentHealth <= 0) //end of battle 
        {
            EndBattle();
        }
        //if we made it this far and we hit submit
        dmgcheck = true;
        if (dmgcheck && submit)
        {
            combatmenu.SetActive(true);
            submit = false; //break out
        }
    }

    //displays player attack by space 
    void AttackSpaceMove(Color c)
    {
        //if submit menu is off break out 
        if (submitmenu.activeSelf == false)
        {
            return;
        }
        Debug.Log("ATK SP");
        
        GameObject t;
        //temp index 
        int moveidx = currentPlayerTileIndex;
        //checks each direction 
        if (Input.GetKeyDown(KeyCode.S))
        {
            if (moveidx - 7 >= 0) //S
            {
                //reset the color
                ResetTileColor();
                //change temp index 
                moveidx = currentPlayerTileIndex - 7;
                //change direction 
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

        // switch by direction 
        switch (player_dir)
        {
            case Dir.left:
                //passing in attack color and move index for attack 
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
                
              //default left attack 
                Attack(c, currentPlayerTileIndex + 1);
                // player_dir = Dir.none;
                break;
        }

        
    }

    void Attack(Color c, int curr)
    {

        
        GameObject t;

        int moveidx = curr;
        //difference from original positinon in phase 
        var diff = moveidx - originalidx;

        //by the player attacks from our menu attack options 
        switch (player_attacks)
        {
            case ElementAttacks.Quake:

                //left
                if (currentPlayerTileIndex + 1 < tiles.Length) //W
                {
                    //temp for tile that we want 
                    t = tiles[currentPlayerTileIndex + 1];
                    //new tile 
                    Tiles_e[currentPlayerTileIndex + 1] = new Entity(t, currentPlayerTileIndex + 1, "Player");
                    //set color 
                    Tiles_e[currentPlayerTileIndex + 1].self.GetComponent<SpriteRenderer>().color = c;
                    attackBy[currentPlayerTileIndex + 1] = "Player"; //reassigning space by 
                    

                }
                if (currentPlayerTileIndex - 1 >= 0)
                {

                    t = tiles[currentPlayerTileIndex - 1];
                    Tiles_e[currentPlayerTileIndex - 1] = new Entity(t, currentPlayerTileIndex - 1, "Player");
                    Tiles_e[currentPlayerTileIndex - 1].self.GetComponent<SpriteRenderer>().color = c;
                    attackBy[currentPlayerTileIndex - 1] = "Player";
                }
                //up 
                if (currentPlayerTileIndex + 7 < tiles.Length) //W
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
                    }
                    if (moveidx + 2 < tiles.Length) //W
                    {

                        t = tiles[moveidx + 2];
                        Tiles_e[moveidx + 2] = new Entity(t, moveidx + 2, "Player");
                        Tiles_e[moveidx + 2].self.GetComponent<SpriteRenderer>().color = c;
                        moveidx = currentPlayerTileIndex;
                        attackBy[moveidx + 2] = "Player";
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
                    }
                    if (moveidx - 1 >= 0)
                    {

                        t = tiles[moveidx - 1];
                        Tiles_e[moveidx - 1] = new Entity(t, moveidx - 1, "Player");
                        Tiles_e[moveidx - 1].self.GetComponent<SpriteRenderer>().color = c;
                        attackBy[moveidx - 1] = "Player";
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

                    }
                    if (moveidx + 7 < tiles.Length) //W
                    {

                        t = tiles[moveidx + 7];
                        Tiles_e[moveidx + 7] = new Entity(t, moveidx + 7, "Player");
                        Tiles_e[moveidx + 7].self.GetComponent<SpriteRenderer>().color = c;
                        attackBy[moveidx + 7] = "Player";
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
                    }
                    if (moveidx - 7 >= 0)
                    {

                        t = tiles[moveidx - 7];
                        Tiles_e[moveidx - 7] = new Entity(t, moveidx - 7, "Player");
                        Tiles_e[moveidx - 7].self.GetComponent<SpriteRenderer>().color = c;
                        attackBy[moveidx - 7] = "Player";
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

                    }
                    if (moveidx + 2 < tiles.Length) //W
                    {

                        t = tiles[moveidx + 2];
                        Tiles_e[moveidx + 2] = new Entity(t, moveidx + 2, "Player");
                        Tiles_e[moveidx + 2].self.GetComponent<SpriteRenderer>().color = c;
                        attackBy[moveidx + 2] = "Player";
                        moveidx += 2;
                        if (moveidx + 7 < tiles.Length)
                        {

                            t = tiles[moveidx + 7];
                            Tiles_e[moveidx + 7] = new Entity(t, moveidx + 7, "Player");
                            Tiles_e[moveidx + 7].self.GetComponent<SpriteRenderer>().color = c;
                            attackBy[moveidx + 7] = "Player";

                        }
                        if (moveidx - 7 >= 0)
                        {

                            t = tiles[moveidx - 7];
                            Tiles_e[moveidx - 7] = new Entity(t, moveidx - 7, "Player");
                            Tiles_e[moveidx - 7].self.GetComponent<SpriteRenderer>().color = c;
                            attackBy[moveidx - 7] = "Player";

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
                    }
                    if (moveidx - 1 >= 0)
                    {

                        t = tiles[moveidx - 1];
                        Tiles_e[moveidx - 1] = new Entity(t, moveidx - 1, "Player");
                        Tiles_e[moveidx - 1].self.GetComponent<SpriteRenderer>().color = c;
                        attackBy[moveidx - 1] = "Player";
                        moveidx -= 1;
                        if (moveidx + 7 < tiles.Length)
                        {

                            t = tiles[moveidx + 7];
                            Tiles_e[moveidx + 7] = new Entity(t, moveidx + 7, "Player");
                            Tiles_e[moveidx + 7].self.GetComponent<SpriteRenderer>().color = c;
                            attackBy[moveidx + 7] = "Player";

                        }
                        if (moveidx - 7 >= 0)
                        {

                            t = tiles[moveidx - 7];
                            Tiles_e[moveidx - 7] = new Entity(t, moveidx - 7, "Player");
                            Tiles_e[moveidx - 7].self.GetComponent<SpriteRenderer>().color = c;
                            attackBy[moveidx - 7] = "Player";

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

                    }
                    if (moveidx + 7 < tiles.Length)
                    {

                        t = tiles[moveidx + 7];
                        Tiles_e[moveidx + 7] = new Entity(t, moveidx + 7, "Player");
                        Tiles_e[moveidx + 7].self.GetComponent<SpriteRenderer>().color = c;
                        attackBy[moveidx + 7] = "Player";
                        moveidx += 7;
                        if (moveidx + 1 < tiles.Length)
                        {

                            t = tiles[moveidx + 1];
                            Tiles_e[moveidx + 1] = new Entity(t, moveidx + 1, "Player");
                            Tiles_e[moveidx + 1].self.GetComponent<SpriteRenderer>().color = c;
                            attackBy[moveidx + 1] = "Player";

                        }
                        if (moveidx - 1 < tiles.Length)
                        {

                            t = tiles[moveidx - 1];
                            Tiles_e[moveidx - 1] = new Entity(t, moveidx - 1, "Player");
                            Tiles_e[moveidx - 1].self.GetComponent<SpriteRenderer>().color = c;
                            attackBy[moveidx - 1] = "Player";

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
                    }
                    if (moveidx - 7 >= 0)
                    {

                        t = tiles[moveidx - 7];
                        Tiles_e[moveidx - 7] = new Entity(t, moveidx - 7, "Player");
                        Tiles_e[moveidx - 7].self.GetComponent<SpriteRenderer>().color = c;
                        attackBy[moveidx - 7] = "Player";
                        moveidx -= 7;
                        if (moveidx + 1 < tiles.Length)
                        {

                            t = tiles[moveidx + 1];
                            Tiles_e[moveidx + 1] = new Entity(t, moveidx + 1, "Player");
                            Tiles_e[moveidx + 1].self.GetComponent<SpriteRenderer>().color = c;
                            attackBy[moveidx + 1] = "Player";

                        }
                        if (moveidx - 1 < tiles.Length)
                        {

                            t = tiles[moveidx - 1];
                            Tiles_e[moveidx - 1] = new Entity(t, moveidx - 1, "Player");
                            Tiles_e[moveidx - 1].self.GetComponent<SpriteRenderer>().color = c;
                            attackBy[moveidx - 1] = "Player";

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
                break;
            case ElementAttacks.none:
                
                break;
            default:
                break;
        }



    }

    //resets tile color 
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

    //resets tile space 
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

    //player movement 
    void PlayerMove()
    {
        //if our movement menu is closed, then break out 
        if(movemenu.activeSelf == false)
        {
            return;
        }
       
        //if its the players turn and we're moving then set the players position 
        if (moving && playerTurn == true)
        {
            StartCoroutine(Movement(currentPlayerTileIndex,Player));
        } //if we have num moves and its our turn //then we can use keyboard 
        else if (num_moves > 0 && playerTurn == true)
        {
          

            if (Input.GetKeyDown(KeyCode.S))
            {
                //play sound
                audio.PlayOneShot(combatClips[3]);

                if (currentPlayerTileIndex - 7 >= 0)
                {
                    currentPlayerTileIndex = currentPlayerTileIndex - 7;
                    moving = true; //set move true 
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
                //play sound
                audio.PlayOneShot(combatClips[3]);

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
                //play sound
                audio.PlayOneShot(combatClips[3]);

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
                //play sound
                audio.PlayOneShot(combatClips[3]);

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
            
        } //if we ran out of moves or its not our turn anymore inbetween frames 
        else if (num_moves <= 0 || playerTurn == false) {

            //set move true to save last position 
            move = true;
        }
        
        
    }
    //sets player positon based off of tile index 
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
            //play sound
            audio.PlayOneShot(combatClips[2]);
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

        //reset all values
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
        //dispose of enemie entities 
        Enemies.Clear();
        enemyHealth.Clear();
        //reset tiles 
        ResetTileSpace();
        ResetTileColor();
        
        //load scenes
        if(PlayerStats.finalEncounter == true)
        {
            SceneManager.LoadScene("WinScene");
        }
        else if (currentHealth <= 0)
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


