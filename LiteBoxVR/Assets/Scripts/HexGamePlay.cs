using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexGameplay : TagModularity
{
    float StartTimeMS; //the time the game started in ms

    float TimePassedMS; // the time passed in ms

    float HitThresholdMS = 250; // the amount of time in ms a user is allowed to hit the note prior or after the target time. This time can not be larger than the time between notes

    float HitTargetTimeOffsetMS = 0; // the amount of time in ms to add to the notedata time value to get the target hit time

    float NoteAppearTimeMS = 800; // the amount of time in ms prior to the target hit time that a warning visual should appear (i.e. a dot starting its lerp from a start position to a target position)

    public float TimeScale = 1;

    public TextAsset testjson;

    public GameObject NotePrefab;

    public bool EnableDebugLines;

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

        public float hitAccuracy;

        public GameObject NoteVisual;

        public float LerpProgress;


        public float StartTime;

        public float CanHitTime;

        public float TargetTime;

        public float ExpireTime;



    }

    public UnityEngine.UI.Text DataText;

    public NoteObject[] noteObjects;

    public List<NoteObject> ActiveNoteObjects;

    public delegate void UpdateVisualsDelegate(NoteObject[] activeNotes);

    public UpdateVisualsDelegate UpdateVisuals = delegate { };

    public GameObject GameVisualsObject;

    //public JsonDecoder.NoteData[] noteDatas;

    public void GenerateNoteObjectsFromJson(TextAsset songJson)
    {
        JsonDecoder.NoteData[] noteDatas;

        JsonDecoder jDecoder = GetComponent<JsonDecoder>();

        if (jDecoder == null)
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

            noteObjects[i].StartTime = noteObjects[i].time + HitTargetTimeOffsetMS - NoteAppearTimeMS;

            noteObjects[i].TargetTime = noteObjects[i].time + HitTargetTimeOffsetMS;

            noteObjects[i].ExpireTime = noteObjects[i].time + HitTargetTimeOffsetMS + HitThresholdMS;

            noteObjects[i].CanHitTime = noteObjects[i].time + HitTargetTimeOffsetMS - HitThresholdMS;
        }

        NotesGenerated();
    }

    private void Start()
    {
        GenerateNoteObjectsFromJson(testjson);


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
            //float thisNoteStartTime = noteObjects[i].time + HitTargetTimeOffsetMS - NoteAppearTimeMS;

            //float thisNoteExpireTime = noteObjects[i].time + HitTargetTimeOffsetMS + HitThresholdMS;

            if (TimePassedMS > noteObjects[i].StartTime && TimePassedMS < noteObjects[i].ExpireTime && noteObjects[i].born == false)
            {
                noteObjects[i].born = true;
                ActiveNoteObjects.Add(noteObjects[i]);
            }
            else if (TimePassedMS > noteObjects[i].ExpireTime)
            {
                noteObjects[i].expired = true;
                ActiveNoteObjects.Remove(noteObjects[i]);
            }

        }

        TimePassedMS += Time.deltaTime * 1000 * TimeScale;

        //DataText.text = TimePassedMS.ToString();

    }

    void UpdateVisualsOld()
    {
        foreach (NoteObject n in ActiveNoteObjects)
        {
            /*float thisNoteStartTime = n.time + HitTargetTimeOffsetMS - NoteAppearTimeMS;

            float thisNoteTargetTime = n.time + HitTargetTimeOffsetMS;

            float thisNoteEndTime = n.time + HitTargetTimeOffsetMS + HitThresholdMS;

            //float NoteLerpProgress = Mathf.InverseLerp(thisNoteStartTime, thisNoteTargetTime, TimePassedMS);

            float NoteLerpProgress = Mathf.InverseLerp(thisNoteStartTime, thisNoteEndTime, TimePassedMS);

            n.LerpProgress = NoteLerpProgress;

            //n.NoteVisual.transform.position = Vector3.Lerp(new Vector3(n.pad*2, 0, 0), new Vector3(n.pad*2, 5, 0), NoteLerpProgress);
            */


            n.LerpProgress = Mathf.InverseLerp(n.StartTime, n.ExpireTime, TimePassedMS);

        }

    }

    public float PadHit(int padIndex)
    {
        Debug.Log("Hit pad: " + padIndex);

        List<NoteObject> notesOnPad = new List<NoteObject>();

        foreach (NoteObject n in ActiveNoteObjects)
        {
            if (n.pad == padIndex && !n.beenHit)
                notesOnPad.Add(n);
        }

        if (notesOnPad.Count == 0)
            return -1;

        NoteObject oldestNote = notesOnPad[0];

        foreach (NoteObject n in notesOnPad)
        {
            if (n.time < oldestNote.time)
            {
                oldestNote = n;

            }

        }

        if (TimePassedMS < oldestNote.time + HitTargetTimeOffsetMS - HitThresholdMS)
            return -1;

        oldestNote.beenHit = true;

        //ActiveNoteObjects.Remove(oldestNote);

        float missTime;

        if (TimePassedMS > oldestNote.TargetTime)
        {
            missTime = oldestNote.ExpireTime;

        }
        else
        {
            missTime = oldestNote.CanHitTime;

        }


        oldestNote.hitAccuracy = Mathf.InverseLerp(missTime, oldestNote.TargetTime, TimePassedMS);

        Debug.Log("can hit time: " + oldestNote.CanHitTime);

        Debug.Log("expire time: " + oldestNote.ExpireTime);

        Debug.Log("miss time: " + missTime);

        Debug.Log("target time: " + oldestNote.TargetTime);

        Debug.Log("time passed: " + TimePassedMS);

        Debug.Log("Hit Accuracy: " + oldestNote.hitAccuracy);

        return oldestNote.hitAccuracy;


    }

    void NoteHit(int noteIndex)
    {
        noteObjects[noteIndex].beenHit = true;

        float missTime = noteObjects[noteIndex].time + HitTargetTimeOffsetMS;

        float thisNoteTargetTime = noteObjects[noteIndex].time + HitTargetTimeOffsetMS;

        if (TimePassedMS < noteObjects[noteIndex].time + HitTargetTimeOffsetMS)
        {
            missTime += HitThresholdMS;

        }
        else
        {
            missTime -= HitThresholdMS;

        }


        noteObjects[noteIndex].hitAccuracy = Mathf.InverseLerp(missTime, thisNoteTargetTime, TimePassedMS);

        Debug.Log("Hit Accuracy: " + noteObjects[noteIndex].hitAccuracy);

    }

    bool WithinCylinder(Vector3 TargetPos, Vector3 OriginPosLocal, float outerRadius, float height)
    {
        Vector3 TargetPosLocal = gameObject.transform.InverseTransformPoint(TargetPos);


        Vector3 TargetPosLocalNoZ = new Vector3(TargetPosLocal.x, TargetPosLocal.y, 0);

        Vector3 OriginPosLocalNoZ = new Vector3(OriginPosLocal.x, OriginPosLocal.y, 0);

        float SqrDistance = Vector3.SqrMagnitude(TargetPosLocal - OriginPosLocal);

        for (int i = 0; i < 20; i++)
        {


            Vector3 originWorld = gameObject.transform.TransformPoint(OriginPosLocal + new Vector3(0, 0, height / 2));

            Vector3 originEnd = OriginPosLocal + new Vector3(0, 0, -height / 2);

            Vector3 originEndWorld = gameObject.transform.TransformPoint(originEnd);

            //Debug.DrawLine(originWorld, originEndWorld);

            Vector3 PointOnEdgeLocal = new Vector3(OriginPosLocal.x + outerRadius * Mathf.Cos(i), OriginPosLocal.y + outerRadius * Mathf.Sin(i), OriginPosLocal.z);

            Vector3 PointOnEdgeWorld = gameObject.transform.TransformPoint(PointOnEdgeLocal + new Vector3(0, 0, height / 2));


            Vector3 PointOnEdgeLocal2 = new Vector3(originEnd.x + outerRadius * Mathf.Cos(i), originEnd.y + outerRadius * Mathf.Sin(i), originEnd.z);

            Vector3 PointOnEdgeWorld2 = gameObject.transform.TransformPoint(PointOnEdgeLocal2);

            if (EnableDebugLines)
            {

                Debug.DrawLine(originWorld, PointOnEdgeWorld);

                Debug.DrawLine(originEndWorld, PointOnEdgeWorld2);

                Debug.DrawLine(PointOnEdgeWorld2, PointOnEdgeWorld);
            }

        }

        Vector3 originWorld4 = gameObject.transform.TransformPoint(OriginPosLocal);

        if (EnableDebugLines)
            Debug.DrawLine(TargetPos, originWorld4);


        if (SqrDistance < outerRadius * outerRadius && TargetPosLocal.z > OriginPosLocal.z - height / 2 && TargetPosLocal.z < OriginPosLocal.z + height / 2)
            return true;

        return false;
    }

    public void PadContact(int padIndex)
    {
        PadHit(padIndex);

    }


    // Virtual functions to be overridden by the

    public virtual void NotesGenerated()
    {


    }

}
