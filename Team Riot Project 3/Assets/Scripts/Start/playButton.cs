using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class playButton : MonoBehaviour
{
    public Text text;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public void PointerEnter()
    {
        text.color = Color.red;
    }

    public void PointerExit()
    {
        text.color = Color.white;
    }

    public void PointerClick()
    {
        text.color = Color.black;
        SceneManager.LoadScene(sceneName: "OverworldScene");
    }
}
