using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    //This script holds all the main features that can be used globally throughout the game

    public static GameManager instance;

    #region Public
    public bool isPaused;
    public Transform ladder;
    #endregion

    #region Private
    private PlayerController player;
    #endregion

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        player = FindObjectOfType<PlayerController>();
    }

    //Handles being able to pause the game from anywhere
    public void Pause(bool pauseState)
    {
        isPaused = pauseState;
    }

    //Used to focus on a target
    public void FocusOn(Transform focus)
    {
        player.OnFocusOn(focus);
    }

    //What happens when the player dies
    public void OnPlayerDeath()
    {
        SceneManager.LoadScene(2);
    }

    //What happens when the player wins
    public void OnWin()
    {
        SceneManager.LoadScene(3);
    }

    //Temporary for prototype - Will be changed in full release
    public void MoveLadder()
    {
        ladder.position = new Vector3(-65, 0, -10);
        ladder.rotation = Quaternion.Euler(0, 90, -3);
    }
}
