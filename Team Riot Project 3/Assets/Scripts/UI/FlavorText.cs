using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FlavorText : MonoBehaviour
{
    //list of strings for flavor text
    public List<string> flavorText;

    //text box prefab
    public GameObject textPrefab;

    //TODO: make a UI manager and add the canvas to it
    public GameObject canvas;

    //TODO: get rid of this, it's for testing
    public KeyCode testButton;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(testButton))
        {
            //TODO: move this to overworld player controller, discuss implementation for interaction trigger box

            //spawn text box
            GameObject itemText = Instantiate(textPrefab, canvas.transform);
            itemText.GetComponent<TextBox>().parent = this;
            itemText.GetComponent<TextBox>().textboxContents = flavorText;
            itemText.GetComponent<TextBox>().enabled = true;
            this.enabled = false;
        }
    }
}
