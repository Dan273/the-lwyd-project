using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEditor;

#if UNITY_EDITOR
[CustomEditor(typeof(Item))]
[CanEditMultipleObjects]
//Custome Editor to show only what is required in the inspector depending on the item type
public class ItemEditor : Editor
{
    SerializedProperty iType;
    SerializedProperty iAmount;
    SerializedProperty iSong;
    SerializedProperty iDoor;

    SerializedProperty iTriggerBool;
    SerializedProperty iTriggerEvent;

    SerializedProperty iDialBool;
    SerializedProperty iDialName;

    void OnEnable()
    {
        iType = serializedObject.FindProperty("type");
        iAmount = serializedObject.FindProperty("amount");
        iSong = serializedObject.FindProperty("song");
        iDoor = serializedObject.FindProperty("door");

        iTriggerBool = serializedObject.FindProperty("willTrigger");
        iTriggerEvent = serializedObject.FindProperty("triggerEvent");

        iDialBool = serializedObject.FindProperty("willDialogue");
        iDialName = serializedObject.FindProperty("dialogueName");
    }

    public override void OnInspectorGUI()
    {
        Item item = target as Item;

        EditorGUILayout.PropertyField(iType, new GUIContent("Type"));

        if (item.type == ItemType.MusicNote)
        {
            EditorGUILayout.PropertyField(iSong, new GUIContent("Song"));
        }
        else if(item.type == ItemType.Match)
        {
            EditorGUILayout.PropertyField(iAmount, new GUIContent("Amount"));
        }
        else if (item.type == ItemType.Key)
        {
            EditorGUILayout.PropertyField(iDoor, new GUIContent("Door"));
        }

        EditorGUILayout.PropertyField(iTriggerBool, new GUIContent("Has Trigger"));

        if (item.willTrigger)
        {
            EditorGUILayout.PropertyField(iTriggerEvent, new GUIContent("Trigger Event"));
        }

        EditorGUILayout.PropertyField(iDialBool, new GUIContent("Has Dialogue"));

        if (item.willDialogue)
        {
            EditorGUILayout.PropertyField(iDialName, new GUIContent("Dialogue Name"));
        }

        serializedObject.ApplyModifiedProperties();
    }
}
#endif

public enum ItemType
{
    Match,
    Key,
    Note,
    MusicNote
}

public class Item : MonoBehaviour
{
    #region Private

    #endregion

    #region Public
    public ItemType type;
    public int amount = 1;
    public Song song;
    public Door door;

    [Header("Triggers")]
    public bool willTrigger;
    public UnityEvent triggerEvent;

    [Header("Dialogue")]
    public bool willDialogue;
    public string dialogueName;
    #endregion
}
