using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using TMPro;

public class MainMenu : MonoBehaviour
{
    #region Private
    private bool controlsOpen;
    #endregion

    #region Public

    #endregion

    void Start()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    //Called when we click the play button. Starts the game
    public void OnPlay()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    //Quits the game
    public void OnQuit()
    {
        Application.Quit();
    }

    //Toggles the control menu 
    public void ToggleControls()
    {
        GameObject buttonHit = EventSystem.current.currentSelectedGameObject;

        controlsOpen = !controlsOpen;
        if (controlsOpen)
        {
            buttonHit.GetComponent<TextMeshProUGUI>().text = "v";
            buttonHit.GetComponent<RectTransform>().position += new Vector3(0, 430);
        }
        else
        {
            buttonHit.GetComponent<TextMeshProUGUI>().text = "^ \n Controls";
            buttonHit.GetComponent<RectTransform>().position -= new Vector3(0, 430);
        }
    }
}
