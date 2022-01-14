using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class GameMenu : MonoBehaviour
{
    public GameObject menuPanel;

    public void QuitGame()
    {
        Application.Quit();
    }

    public void Continue()
    {
        menuPanel.SetActive(false);
    }
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            menuPanel.SetActive(true);
        }
    }
}
