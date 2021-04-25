using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    public GameObject Confirmation;


    // Start is called before the first frame update
    public void LoadGame()
    {
        SceneManager.LoadScene("MainScene");
    }

    void Update()
    {
        if (Input.GetKeyUp("escape"))
        {
            ConditionalExit();
        }
    }

    // Update is called once per frame
    private void ConditionalExit()
    {
        if (Confirmation != null)
        {
            var timeSystem = GameObject.FindObjectOfType<TimeSystem>();
            if (timeSystem != null)
            {
                timeSystem.Pause();
            }
            Confirmation.SetActive(true);
        }
        else
        {
            Exit();
        }
    }

    public void Exit()
    {
        Application.Quit();
    }
}

