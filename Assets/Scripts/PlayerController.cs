using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PlayerController : MonoBehaviour
{
    #region Private
    private NavMeshAgent agent;
    private Transform cam;

    private float moveMulti = 1f;
    private float xRaw, yRaw;

    private bool isCrouched;
    private bool isSprinting;

    private Vector3 camOriginalPos;
    #endregion

    #region Public
    [Header("Movement")]
    [Tooltip("The speed of the player movement.")]
    public float moveSpeed = 3;
    public static float noiseLevel;

    [Header("Camera")]
    [Tooltip("How far down the camera can look.")]
    public int minRot = -75;
    [Tooltip("How far up the camera can look.")]
    public int maxRot = 75;
    [Tooltip("The speed of the camera rotation.")]
    public float sensitivity = 3f;
    #endregion

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        cam = Camera.main.transform;
        camOriginalPos = cam.localPosition;

        agent.speed = moveSpeed;
    }

    void Update()
    {
        PlayerMove();
        CameraLook();
    }

    //Allows for player movement
    void PlayerMove()
    {
        //If the GameManager says we are paused, then don't let the code get to the movement part
        if (GameManager.instance.isPaused)
        {
            return;
        }

        //Move player in the vertical and horizontal axis
        agent.Move(Input.GetAxis("Vertical") * transform.forward * moveSpeed * moveMulti * Time.deltaTime);
        agent.Move(Input.GetAxis("Horizontal") * transform.right * moveSpeed * moveMulti * Time.deltaTime);

        noiseLevel = GetSpeed();

        //Crouch
        if (Input.GetButtonDown("Crouch"))
        {
            //Check if something is above the player
            if (isCrouched)
            {
                if (Physics.Raycast(transform.position, Vector3.up, out RaycastHit hit, 1f))
                {
                    //We hit something above our head whilst crouched, and therefore we cannot stand, so return
                    return;
                }
            }

            Crouch();
        }

        //For Sprinting
        if (Input.GetButton("Sprint") && !isCrouched)
        {
            isSprinting = true;
            Sprint();
        }
   
        if (Input.GetButtonUp("Sprint") && !isCrouched)
        {
            isSprinting = false;
            Sprint();
        }
    }

    //Allows the player to crouch
    void Crouch()
    {
        if (isCrouched)
        {   //Stand
            isCrouched = false;
            cam.localPosition = new Vector3(cam.localPosition.x, camOriginalPos.y, cam.localPosition.z);
            moveMulti = 1f;
            agent.height *= 2;


        }
        else
        {   //Crouch
            isCrouched = true;
            cam.localPosition = new Vector3(cam.localPosition.x, camOriginalPos.y / 3f, cam.localPosition.z);
            moveMulti = .5f;
            agent.height /= 2;
        }
    }

    //Allows the player to sprint
    void Sprint()
    {
        if (isSprinting)
        {
            moveMulti = 1.5f;
        }
        else
        {
            moveMulti = 1f;
        }
    }

    //Gets the players speed
    Vector3 lastPos = Vector3.zero;
    float GetSpeed()
    {
        float speed = (transform.position - lastPos).magnitude * 100;
        lastPos = transform.position;

        return speed;
    }

    //Allows the player to look around
    void CameraLook()
    {
        //If the GameManager says we are paused, then unlock the cursor and don't let the code get to the camera movement part
        if (GameManager.instance.isPaused)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            return;
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        xRaw = Input.GetAxisRaw("Mouse X") * sensitivity;
        yRaw += Input.GetAxisRaw("Mouse Y") * sensitivity;

        yRaw = Mathf.Clamp(yRaw, minRot, maxRot);

        //Rotate player
        transform.Rotate(0, xRaw, 0);

        //Rotate camera
        cam.eulerAngles = new Vector3(-yRaw, transform.eulerAngles.y, 0);
    }

    //Focuses on a specific transform
    public void OnFocusOn(Transform focus)
    {
        //Move the player in front of focus transform
        transform.position = focus.position + (focus.forward*3f);

        //The rotation the transform needs to go to to look at the target
        Quaternion lookRot = Quaternion.LookRotation((focus.position - transform.position).normalized);
        lookRot.x = 0;
        lookRot.z = 0;
        transform.rotation = lookRot;
        cam.LookAt(focus);
    }
}
