using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interact : MonoBehaviour
{
    private bool interact = false;

    private GameObject interactObject = null;

    //vector2s for setting box position
    private Vector2 rightPos = new Vector2(2.83f, 0);
    private Vector2 leftPos = new Vector2(-2.83f, 0);
    private Vector2 upPos = new Vector2(0, 4);
    private Vector2 downPos = new Vector2(0, -4);

    // Start is called before the first frame update
    void Start()
    {
        transform.localPosition = rightPos;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && interact == true && interactObject != null)
        {
            interactObject.GetComponent<FlavorText>().ShowText();
            interact = false;
        }

        //move interaction box based on movement
        if(Input.GetKeyDown(KeyCode.W))
        {
            transform.localPosition = upPos;
        }

        if (Input.GetKeyDown(KeyCode.A))
        {
            transform.localPosition = leftPos;
        }

        if (Input.GetKeyDown(KeyCode.S))
        {
            transform.localPosition = downPos;
        }

        if (Input.GetKeyDown(KeyCode.D))
        {
            transform.localPosition = rightPos;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //Debug.Log("interaction box is colliding");
        //check if the colliding object is an inspectable
        if (collision.gameObject.GetComponent<FlavorText>() != null)
        {
            interactObject = collision.gameObject;
            interact = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.GetComponent<FlavorText>() != null)
        {
            //reset interaction variables
            interactObject = null;
            interact = true;
        }
    }
}
