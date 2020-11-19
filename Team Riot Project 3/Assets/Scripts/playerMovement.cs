using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class playerMovement : MonoBehaviour
{
    public static playerMovement instance;

    private float step = 0.0f;

    public bool toggleEncounters = true;
    // Start is called before the first frame update
    void Start()
    {
        //make singleton
        if(instance != null)
        {
            Destroy(this);
        }
        else
        {
            instance = this;
        }
    }

    // Update is called once per frame
    void Update()
    {
        // WASD to control movement
        if (Input.GetKey(KeyCode.W))
        {
            transform.position += Vector3.up * 0.1f;
            step+=0.1f;
        }
        
        if (Input.GetKey(KeyCode.S))
        {
            transform.position -= Vector3.up * 0.1f;
            step += 0.1f;
        }
        
        if (Input.GetKey(KeyCode.A))
        {
            transform.position -= Vector3.right * 0.1f;
            step += 0.1f;
        }
        
        if (Input.GetKey(KeyCode.D))
        {
            transform.position += Vector3.right * 0.1f;
            step += 0.1f;
        }

        // more than 20 step encounter a combat
        if(toggleEncounters)
        {
            if (step >= 20.0f)
            {
                step = 0.0f;
                SceneManager.LoadScene(sceneName: "CombatScene");
            }
        }

        if (Input.GetKey(KeyCode.Escape))
        {
            SceneManager.LoadScene(sceneName: "CombatScene");
        }

    }
}
