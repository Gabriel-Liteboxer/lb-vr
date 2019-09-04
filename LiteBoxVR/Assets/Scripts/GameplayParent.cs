using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameplayParent : MonoBehaviour
{
    // these functions are overridden by gameplay classes inheriting from this class
    public virtual void CreateNoteObject(uint id, uint pad) { }
    public virtual void UpdateNoteLerp(uint id, float lerpProgress) { }
    public virtual void DestroyNoteObject(uint id) { }
    public virtual void NoteObjectHit(uint id) { }
    public virtual void NoteObjectCanBeHit(uint id) { }

    float TimePassedMS; // the time passed in ms

    public float HitThresholdMS = 180; // old val 250; the amount of time in ms a user is allowed to hit the note prior or after the target time. This time can not be larger than the time between notes

    public float HitTargetTimeOffsetMS = -500; // the amount of time in ms to add to the notedata time value to get the target hit time

    public float NoteAppearTimeMS = 800; // the amount of time in ms prior to the target hit time that a warning visual should appear (i.e. a dot starting its lerp from a start position to a target position)

    public float TimeScale = 1; // how quickly time passes. (1 = normal, 2 = double speed)

    public int GameScore;

    public int GameStreak;

    //public TMPro.TextMeshProUGUI ScoreText;

    //public TMPro.TextMeshProUGUI StreakText;

    [System.Serializable]
    public class NoteData
    {
        public uint id;

        public uint time;

        public uint pad;

        public bool expired;

        public bool born;

        public bool canHit;

        public bool beenHit;

        public float hitAccuracy;

        public float LerpProgress;

        public float StartTime;

        public float CanHitTime;

        public float TargetTime;

        public float ExpireTime;

    }

    public UnityEngine.UI.Text DataText;

    public NoteData[] noteObjects;

    public List<NoteData> ActiveNoteObjects;

    public delegate void UpdateVisualsDelegate(NoteData[] activeNotes);

    public UpdateVisualsDelegate UpdateVisuals = delegate { };

    //public JsonDecoder.NoteData[] noteDatas;

    public void GenerateNoteData()
    {
        NotesForDifficulty midiNotes = GameManager.Instance.songLoader.songLibrary.songs[GameManager.Instance.SelectedSong].difficulties[GameManager.Instance.SongDifficulty];

        noteObjects = new NoteData[midiNotes.notes.Count];

        ActiveNoteObjects = new List<NoteData>();

        for (int i = 0; i < midiNotes.notes.Count; i++)
        {
            noteObjects[i] = new NoteData();

            noteObjects[i].time = midiNotes.notes[i].time;

            noteObjects[i].pad = midiNotes.notes[i].pad;

            noteObjects[i].id = (uint)i;

            noteObjects[i].StartTime = noteObjects[i].time + HitTargetTimeOffsetMS - NoteAppearTimeMS;

            noteObjects[i].TargetTime = noteObjects[i].time + HitTargetTimeOffsetMS;

            noteObjects[i].ExpireTime = noteObjects[i].time + HitTargetTimeOffsetMS + HitThresholdMS;

            noteObjects[i].CanHitTime = noteObjects[i].time + HitTargetTimeOffsetMS - HitThresholdMS;

            //CreateNoteObject((uint)i, midiNotes.notes[i].pad);
        }

    }

    public virtual void Start()
    {
        if (GameManager.Instance == null)
            return;

        StartCoroutine(StartGameDelayed());

        GameObject go = gameObject;

        GameManager.Instance.calibratedObject.GetCalibration(ref go);
    }

    IEnumerator StartGameDelayed()
    {
        yield return new WaitForSeconds(1.5f);

        

        GenerateNoteData();

        AudioSource au = GetComponent<AudioSource>();

        //au.clip = GameManager.Instance.songLoader.songLibrary.songs[GameManager.Instance.SelectedSong].Audio(0);

        au.clip = GameManager.Instance.SongAudioToPlay;

        au.Play();
    }

    public virtual void Update()
    {
        UpdateNotes();

        //UpdateVisuals(ActiveNoteObjects.ToArray());

        UpdateVisualsOld();
    }

    void UpdateNotes()
    {
        for (int i = 0; i < noteObjects.Length; i++)
        {

            if (TimePassedMS > noteObjects[i].StartTime && TimePassedMS < noteObjects[i].ExpireTime && noteObjects[i].born == false)
            {
                noteObjects[i].born = true;
                ActiveNoteObjects.Add(noteObjects[i]);
                CreateNoteObject((uint)i, noteObjects[i].pad);
            }
            else if (TimePassedMS > noteObjects[i].ExpireTime)
            {
                noteObjects[i].expired = true;
                ActiveNoteObjects.Remove(noteObjects[i]);

                DestroyNoteObject(noteObjects[i].id);

                if (!noteObjects[i].beenHit)
                {
                    GameStreak = 0;
                    //StreakText.text = "STREAK: " + GameStreak.ToString() + "x";
                }
            }
            

            noteObjects[i].canHit = TimePassedMS > noteObjects[i].CanHitTime && TimePassedMS < noteObjects[i].ExpireTime;

            //NoteObjectCanBeHit(noteObjects[i].id, noteObjects[i].canHit);
        }

        TimePassedMS += Time.deltaTime * 1000 * TimeScale;

        //DataText.text = TimePassedMS.ToString();

    }

    void UpdateVisualsOld()
    {
        foreach (NoteData n in ActiveNoteObjects)
        {
            //Debug.LogError("updated notes");
            n.LerpProgress = Mathf.InverseLerp(n.StartTime, n.ExpireTime, TimePassedMS);

            UpdateNoteLerp(n.id, n.LerpProgress);
        }

    }

    public float PadHit(int padIndex)
    {
        Debug.Log("Hit pad: " + padIndex);

        List<NoteData> notesOnPad = new List<NoteData>();

        foreach (NoteData n in ActiveNoteObjects)
        {
            if (n.pad == padIndex && !n.beenHit)
                notesOnPad.Add(n);
        }

        if (notesOnPad.Count == 0)
            return -1;

        NoteData oldestNote = notesOnPad[0];

        foreach (NoteData n in notesOnPad)
        {
            if (n.time < oldestNote.time)
            {
                oldestNote = n;

            }

        }

        if (TimePassedMS < oldestNote.time + HitTargetTimeOffsetMS - HitThresholdMS)
            return -1;

        oldestNote.beenHit = true;

        //DestroyNoteObject(oldestNote.id);

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

        GameScore += (int)(oldestNote.hitAccuracy * 100);

        GameStreak++;

        NoteObjectHit(oldestNote.id);
        /*
        if (ScoreText != null)
            ScoreText.text = "SCORE: " + GameScore.ToString();

        if (StreakText != null)
            StreakText.text = "STREAK: " + GameStreak.ToString() + "x";
            */
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


}
