using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MariAI : MonoBehaviour
{
    #region Private
    private NavMeshAgent agent;
    private Transform target;

    private int currentWaypoint;
    #endregion

    #region Public
    [Tooltip("The speed of the AI movement.")]
    public float patrolSpeed = 3f;
    public float huntSpeed = 10f;
    [Tooltip("Minimum distance from the waypoint")]
    public float minDistFromWaypoint;
    public float viewAngle = 25f;
    #endregion

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        target = FindObjectOfType<PlayerController>().transform;

        agent.speed = patrolSpeed;
    }

    //Check if Mari can hear the target
    bool CanHear()
    {
        float distFromTarget = Vector3.Distance(transform.position, target.position);

        //If the distance from the target is less than the noise the player is making, then we can hear them
        if (Mathf.Abs(distFromTarget) < Mathf.Abs(PlayerController.noiseLevel))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    //Check if Mari can see the target
    bool CanSee()
    {
        float distFromTarget = Vector3.Distance(transform.position, target.position);
        float angle = Vector3.Angle(transform.forward, target.position - transform.position);
        bool canSee = false;
        //Create a cone of vision between two angles, and if the player is between those two points
        if (Mathf.Abs(angle) < viewAngle)
        {
            //The player is in front of Mari
            //And is close enough
            if (distFromTarget < 30f)
            {
                //Then create a linecast
                if (Physics.Linecast(transform.position, target.position+Vector3.up, out RaycastHit hit))
                {
                    //And if the linecast succesfully hits the player, then we can see them
                    if (hit.transform == target)
                    {
                        return true;
                    }
                }
            }
        }

        return canSee;
    }

    //This handles what happens when Mari spawns in. She will start patrolling
    public void OnSpawn()
    {
        Vector3 targetLastPosition = target.position;
        agent.SetDestination(targetLastPosition);
        StartCoroutine(Patrol());
    }

    //Will handle patrolling, which is the state Mari takes when she does not know the targets location
    IEnumerator Patrol()
    {
        Debug.Log("Starting Patrol()");
        agent.speed = patrolSpeed;
        while (!CanHear())
        {
            agent.SetDestination(MariManager.instance.wayPoints[currentWaypoint].position);
            float dist = Vector3.Distance(transform.position, MariManager.instance.wayPoints[currentWaypoint].position);
            dist = Vector3.Distance(transform.position, MariManager.instance.wayPoints[currentWaypoint].position);
            if (dist < minDistFromWaypoint)
            {
                if (currentWaypoint < MariManager.instance.wayPoints.Count - 1)
                    currentWaypoint++;
                else
                    currentWaypoint = 0;
            }

            yield return null;
        }

        //If we get to here then we can hear the target, so search
        StartCoroutine(Search());
    }

    //This is the state Mari will go into when she hears the player
    //Will work when Mari hears the player
    IEnumerator Search()
    {
        Debug.Log("Started Search()");
        agent.speed = patrolSpeed;
        Vector3 lastPos = target.position;
        agent.SetDestination(lastPos);
        yield return new WaitUntil(()=>Vector3.Distance(transform.position, lastPos) > 2f || CanSee());

        bool foundTarget = false;
        float timer = 5f;
        while (timer > 0f)
        {
            timer -= Time.deltaTime;
            if (CanHear())
            {
                break;
            }
            if (CanSee())
            {
                foundTarget = true;
                break;
            }
            yield return null;
        }
        if (foundTarget)
        {   
            StartCoroutine(Hunt());
        }
        else
        {
            StartCoroutine(Patrol());
        }
    }

    //This is the state Mari will go into when she sees the player
    IEnumerator Hunt()
    {
        Debug.Log("Started Hunt()");
        agent.speed = huntSpeed;
        while (CanSee())
        {
            agent.SetDestination(target.position);
            if (Vector3.Distance(transform.position, target.position) < 3.25f)
            {
                if (!OrganHandler.hasPlayer)
                {
                    KillPlayer();
                    StopAllCoroutines();
                    break;
                }
            }
            yield return null;
        }

        //If we get here then we can't see the target anymore
        StartCoroutine(Search());
    }

    //Kill the player
    void KillPlayer()
    {
        Debug.Log("Mari has killed the player!");
        GameManager.instance.OnPlayerDeath();
    }
}
