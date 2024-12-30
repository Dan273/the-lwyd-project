using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro; 

public class Inventory : MonoBehaviour
{
    #region Private
    private Transform itemInHand = null;
    private Transform itemInspecting = null;
    private Transform cam;
    private bool itemHeld;
    private bool journalOpen;
    private int currentSongSelected;
    #endregion

    #region Public
    public static Inventory instance;

    public Transform hand;

    public List<Song> songsCollected;
    public List<Item> itemsCollected;

    public GameObject journalObject;

    public TextMeshProUGUI crosshair;
    public Image noteImage;
	#endregion

    void Awake()
    {
        instance = this;

        cam = Camera.main.transform;
    }
    
    void Update()
    {
        if (!itemHeld)
        {
            //Raycast from camera to see if we hit anything
            if (Physics.Raycast(cam.position, cam.forward, out RaycastHit hit, 2.5f))
            {
                //Check if what he his is interactable so we can change the crosshair accordingly
                CheckIfInteractable(hit.transform);
                
                if (Input.GetButtonDown("Interact"))
                {
                    Debug.Log("Hit: " + hit.transform.name);
                    //If we hit something with the Pickup tag
                    if (hit.transform.tag == "Pickup")
                    {
                        //If we aren't holding anything
                        if (itemInHand == null)
                        {
                            PickUp(hit.transform);
                        }
                    }
                    //If we hit an item
                    else if (hit.transform.GetComponent<Item>())
                    {
                        if (!itemHeld)
                        {
                            InspectItem(hit.transform);
                        }
                    }
                    //If we hit a door
                    else if (hit.transform.GetComponent<Door>())
                    {
                        CheckForKey(hit.transform.GetComponent<Door>());
                    }
                }
            }
            else
            {
                //If we arent hitting anything then set the crosshair to grey
                crosshair.color = Color.black;
            }
        }
        else
        {
            if (Input.GetButtonDown("Interact"))
            {
                StoreItem(itemInspecting);
            }
            if (Input.GetButtonDown("Cancel"))
            {
                PutBack();
            }
        }

        //If we are holding something then we can drop it
        if (itemInHand != null)
        {
            if (Input.GetButtonDown("Drop"))
            {
                DropItem();
            }
        }

        //Handle opening the Journal
        if (Input.GetButtonDown("Journal"))
        {
            if (journalOpen)
            {
                OpenJournal(false);
            }
            else
            {
                OpenJournal(true);
            }
        }
    }

    //This will put the item in front of the player to inspect
    void InspectItem(Transform item)
    {
        itemInspecting = item;

        //Hold the Item in front of the camera
        item.GetComponent<Rigidbody>().isKinematic = true;

        item.position = cam.position + (cam.forward / 1.5f);
        item.rotation = cam.rotation;

        GameManager.instance.Pause(true);

        itemHeld = true;
    }

    //This will put the inspected item into the player inventory
    void StoreItem(Transform itemTransform)
    {
        //Grab the item we found
        Item item = itemTransform.GetComponent<Item>();

        //Handle Trigger event
        if (item.type == ItemType.MusicNote)
        {
            songsCollected.Add(item.song);
        }
        else if (item.type == ItemType.Key)
        {
            itemsCollected.Add(item);
        }
        
        if (item.willTrigger)
        {
            item.triggerEvent.Invoke();
        }

        //Handle Dialogue
        if (item.willDialogue)
        {
            DialogueDatabase.instance.OnDialogue(item.dialogueName);
        }

        Destroy(item.gameObject);

        GameManager.instance.Pause(false);

        itemHeld = false;
    }

    //This will drop the item inspecting
    void PutBack()
    {
        itemInspecting.GetComponent<Rigidbody>().isKinematic = false;
        GameManager.instance.Pause(false);

        itemInspecting = null;
        itemHeld = false;
    }

    //Pickup an item into the players hand
    void PickUp(Transform item)
    {
        itemInHand = item;
        itemInHand.GetComponent<Rigidbody>().isKinematic = true;
        itemInHand.GetComponent<Collider>().enabled = false;
        itemInHand.parent = hand;
        itemInHand.localPosition = Vector3.zero;
        itemInHand.localRotation = Quaternion.identity;
    }

    //Drop the item in the players hand
    void DropItem()
    {
        itemInHand.parent = null;
        itemInHand.GetComponent<Rigidbody>().isKinematic = false;
        itemInHand.GetComponent<Collider>().enabled = true;

        itemInHand = null;
    }

    //Check if the player has the key for the door
    void CheckForKey(Door doorHit)
    {
        bool hasKey = false;
        foreach (Item item in itemsCollected)
        {
            if (item.type == ItemType.Key)
            {
                if (item.door == doorHit)
                {
                    hasKey = true;
                    doorHit.Open();
                }
            }
        }

        if (!hasKey)
        {
            doorHit.Locked();
        }
    }

    //Check if what we are looking at is interactable
    void CheckIfInteractable(Transform hit)
    {
        if (hit.tag == "Pickup" || hit.GetComponent<Item>() || hit.root.GetComponent<OrganHandler>() || hit.GetComponent<Door>())
        {
            crosshair.color = Color.white;
        }
        else
        {
            crosshair.color = Color.black;
        }
    }

    //Open/Close the journal UI
    void OpenJournal(bool willOpen)
    {
        journalOpen = willOpen;
        journalObject.SetActive(willOpen);

        if (songsCollected.Count < 1)
        {
            noteImage.enabled = false;
        }
        else
        {
            noteImage.enabled = true;
            if (noteImage.sprite == null)
            {
                noteImage.sprite = songsCollected[currentSongSelected].noteObject.GetComponent<SpriteRenderer>().sprite;
            }
        }
    }

    //Change the image used in the journal for the songs you have stored
    public void ChangeSong(int amount)
    {
        if (songsCollected.Count > 0)
        {
            currentSongSelected += amount;
            if (currentSongSelected > songsCollected.Count-1)
            {
                currentSongSelected = 0;
            }
            if (currentSongSelected < 0)
            {
                currentSongSelected = songsCollected.Count-1;
            }

            noteImage.sprite = songsCollected[currentSongSelected].noteObject.GetComponent<SpriteRenderer>().sprite;
        }
        else
        {
            noteImage.enabled = false;
        }
    }
}
