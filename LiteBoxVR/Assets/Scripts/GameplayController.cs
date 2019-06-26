using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameplayController : MonoBehaviour
{
    float StartTimeMS; //the time the game started in ms

    float TimePassedMS; // the time passed in ms

    float HitThresholdMS = 500; // the amount of time in ms a user is allowed to hit the note prior or after the target time. This time can not be larger than the time between notes

    float HitTargetTimeOffsetMS = 0; // the amount of time in ms to add to the notedata time value to get the target hit time

    float NoteAppearTimeMS = 800; // the amount of time in ms prior to the target hit time that a warning visual should appear (i.e. a dot starting its lerp from a start position to a target position)

    public float TimeScale = 1;

    public TextAsset testjson;

    public GameObject NotePrefab;

    [System.Serializable]
    public class NoteObject
    {
        //public JsonDecoder.NoteData noteData;

        public float time;

        public int pad;

        public bool expired;

        public bool born;

        public bool canHit;

        public bool beenHit;

        public GameObject NoteVisual;

        public float LerpProgress;

    }

    public UnityEngine.UI.Text DataText;

    public NoteObject[] noteObjects;

    public List<NoteObject> ActiveNoteObjects;

    public delegate void UpdateVisualsDelegate(NoteObject[] activeNotes);

    public UpdateVisualsDelegate UpdateVisuals = delegate { };

    //public JsonDecoder.NoteData[] noteDatas;

    public void GenerateNoteObjectsFromJson (TextAsset songJson)
    {
        JsonDecoder.NoteData[] noteDatas;

        JsonDecoder jDecoder = GetComponent<JsonDecoder>();

        if(jDecoder == null)
        {
            jDecoder = gameObject.AddComponent<JsonDecoder>();

        }

        noteDatas = jDecoder.GetNoteDataFromJson(songJson);

        noteObjects = new NoteObject[noteDatas.Length];

        ActiveNoteObjects = new List<NoteObject>();

        for (int i = 0; i < noteDatas.Length; i++)
        {
            noteObjects[i] = new NoteObject();
            
            noteObjects[i].time = noteDatas[i].time;

            noteObjects[i].pad = noteDatas[i].pad;

            noteObjects[i].NoteVisual = GameObject.Instantiate(NotePrefab, new Vector3(noteObjects[i].pad*2, 0, 0), Quaternion.identity);
        }

    }

    private void Start()
    {
        GenerateNoteObjectsFromJson(testjson);

        GetComponent<PunchPadVisuals>().SetNotes(ref noteObjects);
    }

    private void Update()
    {
        UpdateNotes();

        //UpdateVisuals(ActiveNoteObjects.ToArray());

        UpdateVisualsOld();
    }

    void UpdateNotes()
    {
        for (int i = 0; i < noteObjects.Length; i++)
        {
            float thisNoteStartTime = noteObjects[i].time + HitTargetTimeOffsetMS - NoteAppearTimeMS;

            float thisNoteExpireTime = noteObjects[i].time + HitTargetTimeOffsetMS + HitThresholdMS;

            if (TimePassedMS > thisNoteStartTime && TimePassedMS < thisNoteExpireTime && noteObjects[i].born == false)
            {
                noteObjects[i].born = true;
                ActiveNoteObjects.Add(noteObjects[i]);
            }
            else if (TimePassedMS > thisNoteExpireTime)
            {
                noteObjects[i].expired = true;
                ActiveNoteObjects.Remove(noteObjects[i]);
            }

        }

        TimePassedMS += Time.deltaTime * 1000 * TimeScale;

        DataText.text = TimePassedMS.ToString();

    }

    void CreateVisuals() // make this a delegate?
    {



    }

    void UpdateVisualsOld() // make this a delegate?
    {
        foreach(NoteObject n in ActiveNoteObjects)
        {
            float thisNoteStartTime = n.time + HitTargetTimeOffsetMS - NoteAppearTimeMS;

            float thisNoteTargetTime = n.time + HitTargetTimeOffsetMS;

            float NoteLerpProgress = Mathf.InverseLerp(thisNoteStartTime, thisNoteTargetTime, TimePassedMS);

            n.LerpProgress = NoteLerpProgress;

            //n.NoteVisual.transform.position = Vector3.Lerp(new Vector3(n.pad*2, 0, 0), new Vector3(n.pad*2, 5, 0), NoteLerpProgress);


        }

    }

    public void PadHit(int padIndex)
    {
        Dictionary<int, NoteObject> notesOnPad = new Dictionary<int, NoteObject>();

        for (int i = 0; i < noteObjects.Length; i++)
        {
            if(noteObjects[i].pad == padIndex && noteObjects[i].expired == false && noteObjects[i].beenHit == false && noteObjects[i].canHit == true)
            {
                notesOnPad.Add(i, noteObjects[i]);

            }

        }

        if (notesOnPad.Count == 0)
            return;

        KeyValuePair<int, NoteObject> oldestNote;

        bool firstNote = true;

        foreach (KeyValuePair<int, NoteObject> n in notesOnPad)
        {
            if(n.Value.time < oldestNote.Value.time || firstNote)
            {
                oldestNote = n;

            }
            firstNote = false;
        }

        NoteHit(oldestNote.Key);

    }

    void NoteHit(int noteIndex)
    {
        noteObjects[noteIndex].beenHit = true;

    }

}
