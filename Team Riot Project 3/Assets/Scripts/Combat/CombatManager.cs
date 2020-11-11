
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.ComTypes;
using UnityEngine;
using v3 = UnityEngine.Vector3;
using v2 = UnityEngine.Vector2;
using UnityEngine.Animations;

public class CombatManager : MonoBehaviour
{
    // Start is called before the first frame update
    GameObject cam;
    GameObject right;
    GameObject left;
    Camera mainCam;
    int rows = 6;
    int cols = 4;
    float tileSize = 1;
    public GameObject reftile;
    public GameObject Player;
    private int currentPlayerTileIndex;
    public GameObject floor;
    //public GameObject Enemy;
    float move_width;
    float move_height;
    GameObject[] tiles;
    //Menus
    public GameObject combatmenu;
    public GameObject elementmenu;
    public GameObject movemenu;
    public GameObject fleemenu;



    bool w = false;
    bool a = false;
    bool s = false;
    bool d = false;

    bool playerTurn = false;
    int player_lvl = 0;
    float xp_nextlvl = 100.0f;
    v3 menu_pos;
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

        GameObject starttile = tiles[0];
        currentPlayerTileIndex = 0;
        var spr_render = starttile.GetComponent<SpriteRenderer>().bounds.size;
        move_width = spr_render.x;
        move_height = spr_render.y;
        //var n_p = starttile.transform.position;
        //n_p.z = -3;
        Player.transform.position = starttile.transform.position;
        //ENEMY POS
        //Enemy.transform.position = tiles[14].transform.position;

        currentOpt = combatOptions.none;
        Debug.Log(currentOpt);
        CheckIntersectXY(Player.transform.position, starttile.transform.position);
        playerTurn = true;
        // mainCam.rect.width;



        //Destroy(reftile);


        // Update is called once per frame

        if (playerTurn == true)
        {
            Debug.Log(playerTurn);
            elementmenu.SetActive(false);
            movemenu.SetActive(false);
            fleemenu.SetActive(false);
        }

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
        Debug.Log("Back");
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
        if (playerTurn == true)
        {
            switch (currentOpt)
            {
                case combatOptions.move:
                    PlayerMove();
                    break;
                case combatOptions.attack:
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



    

    void PlayerMove()
    {
        if (Input.GetKeyDown(KeyCode.W))
        {
            if (currentPlayerTileIndex - tiles.Length / 4 >= 0)
            {
                currentPlayerTileIndex = currentPlayerTileIndex - tiles.Length / 4;
                Player.transform.position = tiles[currentPlayerTileIndex].transform.position;
                playerTurn = false;
            }
        }
        else if (Input.GetKeyDown(KeyCode.A))
        {
            if ((currentPlayerTileIndex + 1) % 7 != 0)
            {
                currentPlayerTileIndex = currentPlayerTileIndex + 1;
                Player.transform.position = tiles[currentPlayerTileIndex].transform.position;
                playerTurn = false;
            }
        }
        else if (Input.GetKeyDown(KeyCode.S))
        {
            if (currentPlayerTileIndex + tiles.Length / 4 <= tiles.Length)
            {
                currentPlayerTileIndex = currentPlayerTileIndex + tiles.Length / 4;
                Player.transform.position = tiles[currentPlayerTileIndex].transform.position;
                playerTurn = false;
            }
        }
        else if (Input.GetKeyDown(KeyCode.D))
        {
            if ((currentPlayerTileIndex) % 7 != 0)
            {
                currentPlayerTileIndex = currentPlayerTileIndex - 1;
                Player.transform.position = tiles[currentPlayerTileIndex].transform.position;
                playerTurn = false;
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


