using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class playerMovement : MonoBehaviour
{
    public static playerMovement instance;

    //movement sprites
    private SpriteRenderer sprite;
    public List<Sprite> movementSprites;

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

        //save spriterenderer
        sprite = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        // WASD to control movement
        if (Input.GetKey(KeyCode.W))
        {
            transform.position += Vector3.up * 0.1f;
            step+=0.1f;

            sprite.sprite = movementSprites[2];
        }
        
        if (Input.GetKey(KeyCode.S))
        {
            transform.position -= Vector3.up * 0.1f;
            step += 0.1f;

            sprite.sprite = movementSprites[1];
        }
        
        if (Input.GetKey(KeyCode.A))
        {
            transform.position -= Vector3.right * 0.1f;
            step += 0.1f;

            sprite.sprite = movementSprites[0];
            sprite.flipX = true;
        }
        
        if (Input.GetKey(KeyCode.D))
        {
            transform.position += Vector3.right * 0.1f;
            step += 0.1f;

            sprite.sprite = movementSprites[0];
            sprite.flipX = false;
        }

        // more than 20 step encounter a combat

        Debug.Log(step);

        if (step >= 20.0f)
        {
            GetComponent<AudioSource>().Stop();
            step = 0.0f;
            playerMovement.instance.enabled = false;
            SceneManager.LoadScene("test-scene", LoadSceneMode.Additive);
        }


        if (Input.GetKeyDown(KeyCode.Escape))
        {
            GetComponent<AudioSource>().Stop();
            step = 0.0f;
            playerMovement.instance.enabled = false;
            SceneManager.LoadScene("test-scene", LoadSceneMode.Additive);
        }

    }
}
