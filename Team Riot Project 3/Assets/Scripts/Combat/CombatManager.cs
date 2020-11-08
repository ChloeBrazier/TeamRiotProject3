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
    public GameObject right;
    public GameObject left;
    Camera mainCam;
    int rows = 6;
    int cols = 4;
    float tileSize = 1;
    public GameObject reftile;
    void Start()
    {
        cam = GameObject.FindGameObjectWithTag("MainCamera");
        mainCam = cam.GetComponent<Camera>();
       // mainCam.rect.width;
        if(right == null && left == null)
        {
            right = (GameObject)Resources.Load("tile_right");
            left = (GameObject)Resources.Load("tile_left");
            reftile = (GameObject)Resources.Load("tile");
        }

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
        GenFloor();
        //Destroy(reftile);


        // Update is called once per frame
        void Update()
        {

        }
    }
}
