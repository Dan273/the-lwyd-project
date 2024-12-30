using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using TMPro;

//Dialogue
public struct Dialogue
{
	//Dialogue variables declared
	public string name;
	public string content;
	public int time;

	public Dialogue(string n, string con, int tim)
	{
		name = n;
		content = con;
		time = tim;
	}

}

public class DialogueDatabase : MonoBehaviour
{
	public static DialogueDatabase instance;

	public TextMeshProUGUI dialogueText;

	//Creates an instance of Dialogue called 'dialogue'.
	static Dialogue[] dialogue;

	// Start is called before the first frame update
	void Awake()
	{
		instance = this;

		//Sets filename to text file with dialogue included.
		string dialoguefile = Application.dataPath + "/Databases/Dialogue.txt";
		Debug.Log("DataPath: " + dialoguefile);
		//Change the path if we are in the editor;
		if (Application.isEditor)
		{
			dialoguefile = "Assets/Databases/Dialogue.txt";
		}

		//Creates an array of lines and reads from Dialogue.txt.
		string[] lines = File.ReadAllLines(dialoguefile);

		//Adding new attributes to dialogue array.
		dialogue = new Dialogue[]
		{
			new Dialogue("Candle Event", lines[0], 6),
			new Dialogue("Organ Event", lines[1], 6),
			new Dialogue("Mari Spawn", lines[2], 6),
			new Dialogue("Mari Defeat", lines[3], 6),
			new Dialogue("Main Door", lines[4], 6),
			new Dialogue("Side Door", lines[5], 6),
			new Dialogue("Door Locked", lines[6], 6)
		};
	}
 
	//Find dialogue from the array of saved Dialogues
	Dialogue GetDialogue(string name)
    {
		Dialogue dial = new Dialogue();
        foreach (Dialogue dialogue in dialogue)
        {
            if (dialogue.name == name)
            {
				dial = dialogue;
            }
        }

		return dial;
    }

	//Used to call the dialogue function from elsewhere
	public void OnDialogue(string name)
    {
		StartCoroutine(ShowDialogue(name));
	}

	//Shows the dialogue that we have searched for
	IEnumerator ShowDialogue(string name)
	{
		Dialogue dialogue = GetDialogue(name);

		string newDial = dialogue.content;
		newDial = System.Text.RegularExpressions.Regex.Unescape(newDial);
		dialogueText.text = newDial;
		yield return new WaitForSeconds(dialogue.time);
		dialogueText.text = "";
	}
}
