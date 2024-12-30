using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum MusicNote
{
    C,
    D,
    E,
    F,
    G,
    A,
    B
}

public enum MusicLength
{
    Short,
    Medium,
    Long
}

//This holds the information for Songs
[System.Serializable]
public class Song
{
    public string name;
    public MusicNote[] notes;
    public MusicLength length;
    public GameObject noteObject;

    public Song(string n, MusicNote[] note, MusicLength l, GameObject noteObj)
    {
        name = n;
        notes = note;
        length = l;
        noteObject = noteObj;
    }

    public bool hasSpawned = false;
}

public class MusicDatabase : MonoBehaviour
{
    //Instance for the database
    public static MusicDatabase instance;

    //The array of songs
    public Song[] songs;

    void Awake()
    {
        //Telling the game that this is the MusicDatabase;
        instance = this;

        //This is where we will add songs
        //Format is: Name of song, notes for song, length of song
        songs = new Song[]
        { 
            //Short Songs
            NewSong("Song 1", new MusicNote[]{MusicNote.D, MusicNote.F, MusicNote.A, MusicNote.G }, MusicLength.Short),

            NewSong("Song 2", new MusicNote[]{MusicNote.A, MusicNote.F, MusicNote.E, MusicNote.D }, MusicLength.Short),

            NewSong("Song 3", new MusicNote[]{MusicNote.B, MusicNote.G, MusicNote.A, MusicNote.E }, MusicLength.Short),

            //Medium Songs
            NewSong("Song 4", new MusicNote[]{MusicNote.A, MusicNote.F, MusicNote.G, MusicNote.A,
                                                MusicNote.G, MusicNote.E, MusicNote.F, MusicNote.G}, MusicLength.Medium),

            NewSong("Song 5", new MusicNote[]{MusicNote.D, MusicNote.A, MusicNote.G, MusicNote.F,
                                                MusicNote.G, MusicNote.F, MusicNote.E, MusicNote.D}, MusicLength.Medium),

            NewSong("Song 6", new MusicNote[]{MusicNote.E, MusicNote.C, MusicNote.A, MusicNote.G,
                                                MusicNote.E, MusicNote.G, MusicNote.D, MusicNote.C}, MusicLength.Medium),

            //Long Songs
            NewSong("Song 7", new MusicNote[]{MusicNote.B, MusicNote.E, MusicNote.G, MusicNote.F,
                                                MusicNote.E, MusicNote.G, MusicNote.F, MusicNote.E,
                                                MusicNote.B, MusicNote.G, MusicNote.A, MusicNote.F,
                                                MusicNote.A, MusicNote.G, MusicNote.E, MusicNote.C}, MusicLength.Long),

            NewSong("Song 8", new MusicNote[]{MusicNote.C, MusicNote.C, MusicNote.G, MusicNote.E,
                                                MusicNote.B, MusicNote.G, MusicNote.A, MusicNote.G,
                                                MusicNote.F, MusicNote.E, MusicNote.D, MusicNote.C,
                                                MusicNote.E, MusicNote.F, MusicNote.F, MusicNote.D}, MusicLength.Long),

            NewSong("Song 9", new MusicNote[]{MusicNote.F, MusicNote.E, MusicNote.D, MusicNote.A,
                                                MusicNote.G, MusicNote.E, MusicNote.F, MusicNote.G,
                                                MusicNote.G, MusicNote.F, MusicNote.E, MusicNote.F,
                                                MusicNote.G, MusicNote.E, MusicNote.C, MusicNote.C}, MusicLength.Long)

        };
    }

    //Create a new song based on the variables entered
    Song NewSong(string name, MusicNote[] notes, MusicLength length)
    {
        GameObject gameObject = Resources.Load<GameObject>("Songs/" + name);

        if (gameObject != null)
        {
            return new Song(name, notes, length, gameObject);
        }
        else
        {
            Debug.LogWarning("Could not find GameObject for " + name + "! Ensure the name of the GameObject matches the name of the song! " +
                "               \n Placeholder will be used.");
            return new Song(name, notes, length, Resources.Load<GameObject>("Songs/Placeholder"));
        }
    }
}
