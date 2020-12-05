using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class playButton : MonoBehaviour
{
    public AudioClip impact;
    AudioSource audioSource;

    public Text text;
    public List<GameObject> buttons;
    public List<GameObject> credits;
    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
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
        audioSource.PlayOneShot(impact, 0.7F);
        text.color = Color.black;
        SceneManager.LoadScene(sceneName: "OverworldScene");
    }

    public void CreditsClick()
    {
        GetComponent<AudioSource>().Play();
        text.color = Color.white;

        //disable main menu buttons
        foreach (GameObject button in buttons)
        {
            button.SetActive(false);
        }

        //enable credits menu
        foreach (GameObject creditsItem in credits)
        {
            creditsItem.SetActive(true);
        }
    }

    public void CreditsExit()
    {
        GetComponent<AudioSource>().Play();
        text.color = Color.white;
        //disable credits menu and re-enable main menu buttons
        foreach (GameObject creditsItem in credits)
        {
            creditsItem.SetActive(false);
        }

        foreach (GameObject button in buttons)
        {
            button.SetActive(true);
        }
    }
}
