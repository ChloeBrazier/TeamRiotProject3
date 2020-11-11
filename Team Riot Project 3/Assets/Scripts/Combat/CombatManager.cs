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
    public GameObject Enemy;
    float move_width;
    float move_height;
    GameObject[] tiles;
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
        Enemy.transform.position = tiles[14].transform.position;
        currentOpt = combatOptions.none;
        Debug.Log(currentOpt);
        CheckIntersect(Player, starttile);

       // mainCam.rect.width;
        /*if(right == null && left == null)
        {
            right = (GameObject)Resources.Load("tile_right");
            left = (GameObject)Resources.Load("tile_left");
            reftile = (GameObject)Resources.Load("tile");
        }*/

        void GenFloor()
        {
            var cw = mainCam.rect.width;
            var ch = mainCam.rect.height;
            Debug.Log(mainCam.pixelWidth);
            Debug.Log(mainCam.pixelHeight);
            Debug.Log(ch);
            var id = 0;
            //Debug.Log(reftile.GetComponent<SpriteRenderer>().size);
            for (int c = 0; c < cols; c++)
            {
                for (int r = 0; r < rows; r++)
                {

                    var tile = (GameObject)Instantiate(reftile, transform);
                    var size = tile.GetComponent<SpriteRenderer>().size;
                    var sprite = tile.GetComponent<SpriteRenderer>().sprite;
                    
                    

                    float posX = (r * size.x) + cw;
                    float posY = (c * -size.y) + ch;
                    
                    tile.transform.position = new v3(posX, posY, -2);
                    var rot = tile.transform.rotation;
                    
                    //var d_c = cols - c;
                    //float diff = (float)(d_r * d_c);
                    if(r < 3)
                    {
                        

                    }
                    if (r >= 3)
                    {
                       // var d_r = r * 6;
                        //var n_p = new v3(posX, posY, -2);
                        //tile.transform.Translate(new v3(-1, 0, 0));
                        //tile.transform.position = n_p;
                        //tile.transform.Rotate(new v3(0f, 1f, 0), d_r);

                        
                    }
                   /* if (c < 2)
                    {
                        var d_c = (cols - c) * 4;
                        tile.transform.Rotate(new v3(1f, 0, 0), -d_c);
                    }
                    if (c >= 2)
                    {
                        var d_c = c * 4;
                        tile.transform.Rotate(new v3(1f, 0, 0), d_c);
                    }*/

                    tile.name = id.ToString();
                    id++;



                }
                
            }
            
        }
        //GenFloor();
        //Destroy(reftile);




    }
    bool playerTurn = true;
    // Update is called once per frame
    void Update()
    {
        if (currentOpt == combatOptions.none)
        {

        }
        if (playerTurn) 
        {
            PlayerMove();
        }
        else
        {
            EnemyMove();
        }
    }

    void PlayerMove() 
    {
        if (Input.GetKeyDown(KeyCode.W)) 
        {
            if (currentPlayerTileIndex-tiles.Length/4>=0) 
            {
                currentPlayerTileIndex = currentPlayerTileIndex - tiles.Length / 4;
                Player.transform.position = tiles[currentPlayerTileIndex].transform.position;
                playerTurn = false;
            }
        }
        else if (Input.GetKeyDown(KeyCode.A))
        {
            if ((currentPlayerTileIndex+1)%7!=0) {
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
    private bool CheckIntersect(GameObject obj1, GameObject obj2)
    {
        var p1 = obj1.transform.position;
        var p2 = obj2.transform.position;
        var x = p1.x / p2.x;
        var y = p1.y / p2.y;
        if(x > 0.5 || y > 0.5)
        {
            //Debug.Log(x);
            //Debug.Log(y);
            return true;
        }
        
        return false;
    }
}
