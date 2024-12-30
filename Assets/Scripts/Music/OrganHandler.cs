using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

//Holds information for notes used on the Organ (vital for converting key presses)
[System.Serializable]
public class Note
{
    public MusicNote noteName;
    public AudioClip noteAudio;
    public string keyInput;
    public AudioSource audioSource;
    public bool isPlaying;

    public Note(MusicNote k, AudioClip clip, string kI)
    {
        noteName = k;
        noteAudio = clip;
        keyInput = kI;
    }
}

public class OrganHandler : MonoBehaviour
{
    #region Private
    private AudioSource audioSource;

    private bool canInteract = true;
    private bool playingNotes;

    private int keyPressing = -1;
    private int songsPlayed;

    private float timeLeft = 5f;

    private List<Note> notesPlayed = new List<Note>();
    #endregion

    #region Public
    public static bool hasPlayer;
    public static bool canPlay;

    [Tooltip("How much time should be given before the key tracking resets.")]
    public float resetTimer = 3f;
    [Tooltip("The notes used for the intstrument, with the sound played and keyboard key pressed.")]
    public Note[] notes;
	#endregion

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        if (!canPlay)
        {
            return;
        }

        if (hasPlayer)
        {
            if (Input.GetButtonDown("Interact"))
            {
                OrganInteract(canInteract);
            }

            if (keyPressing >= 0)
            {
                if (!notes[keyPressing].isPlaying)
                {
                    PlayNote(keyPressing);
                }
            }
            for (int i = 0; i < notes.Length; i++)
            {
                if (Input.GetKeyDown(notes[i].keyInput))
                {
                    keyPressing = i;
                    break;
                }
                else if (Input.GetKeyUp(notes[i].keyInput))
                {
                    StopNote(i);
                }
            }
        }
        else
        {
            //If the organ doesnt have a player look for its interaction
            if (Input.GetButtonDown("Interact"))
            {
                if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out RaycastHit hit, 2.5f))
                {
                    if (hit.transform == this.transform || hit.transform == this.transform.GetComponentInChildren<Collider>().transform)
                    {
                        OrganInteract(canInteract);
                    }
                }
            }
        }
    }

    //Interact with the Organ
    void OrganInteract(bool interact)
    {
        if (interact)
        {   //If we can interact then let the organ know there is a player at it, and then pause the player
            hasPlayer = true;
            canInteract = false;
            GameManager.instance.Pause(true);
            GameManager.instance.FocusOn(this.transform);
        }
        else
        {   //Otherwise let the organ know there is no longer a player, and unpause the player
            hasPlayer = false;
            canInteract = true;
            GameManager.instance.Pause(false);
        }
    }

    //The play the required note
    void PlayNote(int note)
    {
        //Create the audiosource for the new note
        GameObject newKey = new GameObject(notes[note].noteName.ToString());
        newKey.AddComponent<AudioSource>().spatialBlend = 1;

        //Set the variables for the note playing and the play
        notes[note].audioSource = newKey.GetComponent<AudioSource>();
        notes[note].isPlaying = true;
        notes[note].audioSource.clip = notes[note].noteAudio;
        notes[note].audioSource.Play();

        //Here we will track the notes playing
        notesPlayed.Add(notes[note]);

        //Reset the timer
        timeLeft = resetTimer;
        if (!playingNotes)
        {
            StartCoroutine(RecordTimer());
        }

        //Check to see if the notes played array matches the song we need to play
        if (CheckSong(note))
        {
            StartCoroutine(SongSuccess());
        }
    }

    //Check to see if what we are playing matches a song in the inventory
    bool CheckSong(int note)
    {
        bool flag = false;
        //Go through every playable song and compare the notes to the notes being played
        foreach (Song song in Inventory.instance.songsCollected)
        {
            Debug.Log("Checking Song: " + song.name);
            for (int i = 0; i < song.notes.Length; i++)
            {
                if (notesPlayed.Count >= song.notes.Length)
                {
                    if (notesPlayed[i].noteName == song.notes[i])
                    {
                        flag = true;
                    }
                    else
                    {
                        //If the song we played hasn't been picked up, then it can't work
                        flag = false;
                        break;
                    }
                }
            }

            if (flag)
            {
                Inventory.instance.songsCollected.Remove(song);
                Inventory.instance.ChangeSong(1);
                break;
            }
        }

        return flag;
    }

    //Gets called when we have played a correct song
    IEnumerator SongSuccess()
    {
        yield return new WaitForSeconds(1);

        //We will play back the pre recorded version of the song
        //Then we will tell MariAI that we played the right song, and that she should dissapear
        Debug.Log("Song Success!");

        OrganInteract(false);

        //Stop all notes being played
        for (int i = 0; i < notes.Length; i++)
        {
            StopNote(i);
        }

        //Increase song count and if we have played 3, spawn 3 more
        songsPlayed++;
        if (songsPlayed == 3)
        {
            MusicSystem.instance.GetSongs(3, MusicLength.Medium);
        }
        if (songsPlayed == 6)
        {
            MusicSystem.instance.GetSongs(3, MusicLength.Long);
        }

        //Register Mari "dying"
        MariManager.instance.OnDie();        
    }

    //How long before the key recording resets
    IEnumerator RecordTimer()
    {
        playingNotes = true;
        //Count down the timer
        while (timeLeft > 0)
        {
            timeLeft -= Time.deltaTime;

            yield return null;
        }
        //If we get down to this point, then the timer has finished
        //We can clear the notes just played
        ClearNotes();
    }

    //Clear the recorded notes
    void ClearNotes()
    {
        notesPlayed.Clear();
        playingNotes = false;
    }

    //Stop the notes we are playing
    void StopNote(int key)
    {
        keyPressing = -1;
        notes[key].isPlaying = false;
        if (notes[key].audioSource != null)
        {
            notes[key].audioSource.Stop();
            Destroy(notes[key].audioSource.gameObject);
        }
    }

    //Used to play the random Organ sound on a trigger
    public void PlayOrganSound()
    {
        GetComponent<AudioSource>().Play();
    }
}
