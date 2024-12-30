using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    #region Private

    #endregion

    #region Public
    public bool isExit;
    public bool isMain;
	#endregion

    //Open the door (Currently only for the exit)
    public void Open()
    {
        if (isExit)
        {
            Win();
        }
    }

    //The door is locked so show the correct dialogue
    public void Locked()
    {
        if (isMain)
        {
            DialogueDatabase.instance.OnDialogue("Main Door");
        }
        else
        {
            DialogueDatabase.instance.OnDialogue("Door Locked");
        }
    }

    //Call the OnWin function from the GameManager class
    void Win()
    {
        GameManager.instance.OnWin();
    }
}
