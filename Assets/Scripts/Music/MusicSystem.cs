using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MusicSystem : MonoBehaviour
{
    #region Private
    private MusicDatabase musicDatabase;
    private List<Transform> noteSpawns = new List<Transform>();
    private int spawnPoint;
    #endregion

    #region Public
    public static MusicSystem instance;

    public List<Song> songsToPlay;
    #endregion

    void Awake()
    {
        instance = this;    
    }

    void Start()
    {
        //Grabbing the MusicDatabase and setting it as a variable to make using it faster
        musicDatabase = MusicDatabase.instance;

        //Add the spawnpoints to the list
        GameObject spawnPoints = GameObject.Find("SpawnPoints_Notes");
        for (int i = 0; i < spawnPoints.transform.childCount; i++)
        {
            noteSpawns.Add(spawnPoints.transform.GetChild(i));
        }

        //Call a new song to be played
        GetSongs(3, MusicLength.Short);
    }

    //Get a certain amount of songs ready to spawn
    public void GetSongs(int amount, MusicLength length)
    {
        for (int i = 0; i < amount; i++)
        {
            songsToPlay.Add(GenerateSong(length));
        }
    }

    //This will generate a song based on the length of the music selected
    public Song GenerateSong(MusicLength musicLength)
    {
        //Generate a random number
        int randomSong = Random.Range(0, musicDatabase.songs.Length);

        //Loop through the random number until we get a song that matches the length required
        while (musicDatabase.songs[randomSong].length != musicLength || musicDatabase.songs[randomSong].hasSpawned)
        {
            randomSong = Random.Range(0, musicDatabase.songs.Length);
        }

        //Set the song found as the song
        Song newSong = musicDatabase.songs[randomSong];

        //Setting the song in the database to say it's already been played
        musicDatabase.songs[randomSong].hasSpawned = true;

        //Spawn the note in on a spawnpoint
        GameObject songObject = Instantiate(newSong.noteObject, noteSpawns[spawnPoint].position, noteSpawns[spawnPoint].rotation, 
                                                GameObject.Find("Notes").transform);
        songObject.GetComponent<Item>().type = ItemType.MusicNote;
        songObject.GetComponent<Item>().song = newSong;
        spawnPoint++;

        return newSong;
    }
}
