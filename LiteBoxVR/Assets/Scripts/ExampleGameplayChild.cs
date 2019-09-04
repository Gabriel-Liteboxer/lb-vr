using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoteObject
{
    public uint id;
    public uint pad;
    public float lerpProgress;
    GameObject gameObject;

    public NoteVisuals noteVisuals;

    public NoteObject(uint id, uint pad, GameObject prefab)
    {
        this.id = id;
        this.pad = pad;
        gameObject = GameObject.Instantiate(prefab);
        noteVisuals = gameObject.GetComponent<NoteVisuals>();
        Debug.LogWarning("Instantiated Note");
    }

    public void SetPosition(Vector3 position)
    {
        gameObject.transform.position = position;
    }

    public Vector3 GetPosition()
    {

        return gameObject.transform.position;
    }

    public void SetEulerAngles(Vector3 euler)
    {
        gameObject.transform.eulerAngles = euler;
    }

    public Vector3 GetEulerAngles()
    {

        return gameObject.transform.eulerAngles;
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


    public Dictionary<uint, NoteObject> NoteObjectDict;

    private void Awake()
    {
        NoteObjectDict = new Dictionary<uint, NoteObject>();
    }

    //override
    public override void CreateNoteObject(uint id, uint pad)
    {
        NoteObjectDict.Add(id, new NoteObject(id, pad, NotePrefab));

        
        //Color.HSVToRGB(Random.Range(0f, 1f), 1, 1);

        NoteObjectDict[id].noteVisuals.SetColor(Color.HSVToRGB(Random.Range(0f, 1f), 1, 1));
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
