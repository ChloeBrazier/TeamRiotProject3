using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TextBox : MonoBehaviour
{
    //list of strings for text
    public List<string> textboxContents;

    //child's text component and current line counter
    private Text boxText;
    private int textIndex = 0;

    //child's next icon
    public Image nextIcon;

    //parent script for interaction
    public FlavorText parent;

    // Start is called before the first frame update
    void Start()
    {
        //set text component and first line
        boxText = GetComponentInChildren<Text>();
        boxText.text = textboxContents[textIndex];
    }

    // Update is called once per frame
    void Update()
    {
        //flip through text 
        if(Input.GetKeyDown(KeyCode.Space))
        {
            FlipPage();
        }
    }

    public void FlipPage()
    {
        textIndex++;

        if(textIndex == textboxContents.Count)
        {
            //TODO: make this resume game, probably using same/similar code as exiting encounters
            parent.enabled = true;
            Destroy(this.gameObject);
            return;
        }
        else if(textIndex == textboxContents.Count - 1)
        {
            //disable next page icon
            nextIcon.enabled = false;
        }

        boxText.text = textboxContents[textIndex];
    }
}
