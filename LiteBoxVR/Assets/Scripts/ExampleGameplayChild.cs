using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoteObject
{
    public uint id;
    public float lerpProgress;
    GameObject gameObject;

    public NoteObject(uint id, uint pad, GameObject prefab)
    {
        this.id = id;
        gameObject = GameObject.Instantiate(prefab);

    }

    public void SetPosition(Vector3 position)
    {
        gameObject.transform.position = position;
    }

    public void DestroyNoteObject()
    {
        GameObject.Destroy(gameObject);

    }

    public void SetLerpProgress(float lerpProgress)
    {
        this.lerpProgress = lerpProgress;

    }
}

public class ExampleGameplayChild : GameplayParent
{
    public GameObject NotePrefab;


    Dictionary<uint, NoteObject> NoteObjectDict;

    private void Awake()
    {
        NoteObjectDict = new Dictionary<uint, NoteObject>();
    }

    //override
    public override void CreateNoteObject(uint id, uint pad)
    {
        NoteObjectDict.Add(id, new NoteObject(id, pad, NotePrefab));

    }

    //override
    public override void UpdateNoteLerp (uint id, float lerpProgress)
    {
        if (NoteObjectDict.ContainsKey(id))
        {
            NoteObjectDict[id].lerpProgress = lerpProgress;

        }

    }

    //override
    public override void DestroyNoteObject(uint id)
    {
        if (NoteObjectDict.ContainsKey(id))
        {
            NoteObjectDict[id].DestroyNoteObject();

            NoteObjectDict.Remove(id);

        }

    }

    //something like this will be on the gameplay class that inherits from this class
    /*
    private void Update()
    {
        foreach  (KeyValuePair<uint, NoteObject> pair in NoteObjectDict)
        {
            Vector3 position = Vector3.Lerp(Vector3.zero, Vector3.one, pair.Value.lerpProgress);

            pair.Value.SetPosition(position);


        }
         
    }*/
}
