using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MariManager : MonoBehaviour
{
    #region Private
    private bool mariSpawned;

    private Transform theMari;

    private int deaths;
    #endregion

    #region Public
    public static MariManager instance;

    public GameObject noteToEnable;

    [Header("In Minutes")]
    public float spawnTimerMin;
    public float spawnTimerMax;

    public Transform[] spawnSpots;
    public List<Transform> wayPoints;
    #endregion

    void Awake()
    {
        instance = this;

        Transform wayPointTrans = GameObject.Find("Waypoints").transform;
        foreach (Transform item in wayPointTrans)
        {
            wayPoints.Add(item);
        }
    }

    //Called when Mari "dies". Will handle what happens on her death state
    public void OnDie()
    {
        if(theMari != null)
            Destroy(theMari.gameObject);

        if (deaths < 1)
        {
            DialogueDatabase.instance.OnDialogue("Mari Defeat");
            noteToEnable.SetActive(true);
        }
        deaths++;
        OrganHandler.canPlay = false;
        mariSpawned = false;
        StartCoroutine(SpawnTimer());
    }

    //Count down until Mari respawns
    IEnumerator SpawnTimer()
    {
        //Generate a random spawn time between to values, converted into minutes
        float timeUntilSpawn = Random.Range(spawnTimerMin * 60, spawnTimerMax * 60);
        Debug.Log("Mari spawning in " + timeUntilSpawn + " seconds.");
        yield return new WaitForSeconds(timeUntilSpawn);

        SpawnMari();
    }

    //Spawn in Mari if she is not already spawned
    public void SpawnMari()
    {
        //Check if Mari has spawned, if not then we can spawn her
        if (!mariSpawned)
        {
            int spawnPoint = Random.Range(0, spawnSpots.Length);

            Transform newMari = Instantiate(Resources.Load<Transform>("Mari_Lwyd"), spawnSpots[spawnPoint].position, Quaternion.identity);
            theMari = newMari;
            theMari.GetComponent<MariAI>().OnSpawn();

            OrganHandler.canPlay = true;
            mariSpawned = true;
        }
        else
        {
            Debug.Log("Mari has already spawned!");
        }
    }
}
