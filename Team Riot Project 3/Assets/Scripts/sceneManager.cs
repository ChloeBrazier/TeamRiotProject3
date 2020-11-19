using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class sceneManager : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // go back to OverworldScene
        if (Input.GetKeyDown(KeyCode.P))
        {
            playerMovement.instance.enabled = true;
            Application.UnloadLevel("test-scene");
        }
    }
}
