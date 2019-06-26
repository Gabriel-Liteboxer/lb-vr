using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameplayController1 : MonoBehaviour
{
    float StartTimeMS; //the time the game started in ms

    float TimePassedMS; // the time passed in ms

    float HitThresholdMS = 500; // the amount of time in ms a user is allowed to hit the note prior or after the target time. This time can not be larger than the time between notes

    float HitTargetTimeOffsetMS = 0; // the amount of time in ms to add to the notedata time value to get the target hit time

    float NoteAppearTimeMS = 800; // the amount of time in ms prior to the target hit time that a warning visual should appear (i.e. a dot starting its lerp from a start position to a target position) 

    public class NoteObject
    {
        public JsonDecoder.NoteData noteData;

        public bool expired;

        public bool born;

        public bool canHit;

        public bool beenHit;

    }

    public NoteObject[] noteObjects;

    public List<NoteObject> ActiveNoteObjects;

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
            noteObjects[i].noteData = noteDatas[i];
            
        }

    }

    private void Update()
    {

        for (int i = 0; i < noteObjects.Length; i++)
        {
            if (noteObjects[i].noteData.time + HitTargetTimeOffsetMS + HitThresholdMS < TimePassedMS)
            {
                noteObjects[i].expired = true;

            }
            else if (noteObjects[i].noteData.time + HitTargetTimeOffsetMS - HitThresholdMS < TimePassedMS)
            {
                noteObjects[i].canHit = true;

            }
            else if (noteObjects[i].noteData.time + HitTargetTimeOffsetMS - NoteAppearTimeMS < TimePassedMS)
            {
                noteObjects[i].born = true;

            }

        }

        TimePassedMS += Time.deltaTime * 1000;
    }

    public void PadHit(int padIndex)
    {
        Dictionary<int, NoteObject> notesOnPad = new Dictionary<int, NoteObject>();

        for (int i = 0; i < noteObjects.Length; i++)
        {
            if(noteObjects[i].noteData.pad == padIndex && noteObjects[i].expired == false && noteObjects[i].beenHit == false && noteObjects[i].canHit == true)
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
            if(n.Value.noteData.time < oldestNote.Value.noteData.time || firstNote)
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
