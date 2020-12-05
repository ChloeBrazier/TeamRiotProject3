using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class playerMovement : MonoBehaviour
{
    public static playerMovement instance;

    public AudioClip impact;
    AudioSource audioSource;
    public float time;


    //movement sprites
    private SpriteRenderer sprite;
    public List<Sprite> movementSprites;

    private float step = 0.0f;

    public bool toggleEncounters = true;

    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();

        //make singleton
        if (instance != null)
        {
            Destroy(this);
        }
        else
        {
            instance = this;
        }

        //save spriterenderer
        sprite = GetComponent<SpriteRenderer>();

        //PlayerStats.player_tr = transform.position;
        if(PlayerStats.player_tr.x != 0 && PlayerStats.player_tr.y != 0 &&
            PlayerStats.player_tr.y != 0)
        {
            transform.position = PlayerStats.player_tr;
            Debug.Log(transform.position);
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        time += Time.deltaTime;
        
        // WASD to control movement
        if (Input.GetKey(KeyCode.W))
        {
            transform.position += Vector3.up * 0.1f;
            PlayerStats.player_tr = transform.position;
            step +=0.1f;
            if (time >= 0.25f)
            {
                audioSource.PlayOneShot(impact, 0.7F);
                time = 0;
            }
            sprite.sprite = movementSprites[2];
        }
        
        if (Input.GetKey(KeyCode.S))
        {
            transform.position -= Vector3.up * 0.1f;
            PlayerStats.player_tr = transform.position;
            step += 0.1f;
            if (time >= 0.25f)
            {
                audioSource.PlayOneShot(impact, 0.7F);
                time = 0;
            }
            sprite.sprite = movementSprites[1];
        }
        
        if (Input.GetKey(KeyCode.A))
        {
            transform.position -= Vector3.right * 0.1f;
            PlayerStats.player_tr = transform.position;
            step += 0.1f;
            if (time >= 0.25f)
            {
                audioSource.PlayOneShot(impact, 0.7F);
                time = 0;
            }
            sprite.sprite = movementSprites[0];
            sprite.flipX = true;
        }
        
        if (Input.GetKey(KeyCode.D))
        {
            transform.position += Vector3.right * 0.1f;
            PlayerStats.player_tr = transform.position;
            step += 0.1f;
            if (time >= 0.25f)
            {
                audioSource.PlayOneShot(impact, 0.7F);
                time = 0;
            }
            sprite.sprite = movementSprites[0];
            sprite.flipX = false;
        }

        // more than 20 step encounter a combat

        // Debug.Log(step);

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
