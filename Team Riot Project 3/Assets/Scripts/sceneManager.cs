using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class sceneManager : MonoBehaviour
{
    public GameObject player;
    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("audio");
    }

    // Update is called once per frame
    void Update()
    {
        // go back to OverworldScene
        if (Input.GetKeyDown(KeyCode.P))
        {
            playerMovement.instance.enabled = true;
            player.GetComponent<AudioSource>().Play();
           // Application.UnloadLevel("test-scene");
        }
    }
}
