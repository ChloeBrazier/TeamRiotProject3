using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.ComTypes;
using UnityEngine;
using v3 = UnityEngine.Vector3;
using v2 = UnityEngine.Vector2;
public class CombatManager : MonoBehaviour
{
    // Start is called before the first frame update
    GameObject cam;
    public GameObject reftile;
    Camera mainCam;
    int rows = 6;
    int cols = 4;
    float tileSize = 1;
    void Start()
    {
        cam = GameObject.FindGameObjectWithTag("MainCamera");
        mainCam = cam.GetComponent<Camera>();
        if(reftile == null)
        {
            reftile = (GameObject)Resources.Load("square");
        }

        void GenFloor()
        {
            var cw = mainCam.rect.width;
            var ch = mainCam.rect.height;
            //Debug.Log(cw);
            //Debug.Log(ch);
            var id = 0;
            //Debug.Log(reftile.GetComponent<SpriteRenderer>().size);
            for (int c = 0; c < cols; c++)
            {
                for (int r = 0; r < rows; r++)
                {

                    GameObject tile = (GameObject)Instantiate(reftile, transform);
                    var size = tile.GetComponent<SpriteRenderer>().size;
                    var sprite = tile.GetComponent<SpriteRenderer>().sprite;
                    
                    

                    float posX = (r * size.x) + cw;
                    float posY = (c * -size.y) + ch;
                    var n_p = new v2(posX, posY);
                    tile.transform.position = new v3(posX, posY, -2);
                    var rot = tile.transform.rotation;
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
