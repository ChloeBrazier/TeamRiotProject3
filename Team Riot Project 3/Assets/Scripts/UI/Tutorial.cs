using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tutorial : MonoBehaviour
{
    //check if the tutorial was shown
    private bool shownTutorial = false;

    // Start is called before the first frame update
    void Start()
    {
        if(shownTutorial != true)
        {
            GetComponent<FlavorText>().ShowText();
            shownTutorial = true;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
