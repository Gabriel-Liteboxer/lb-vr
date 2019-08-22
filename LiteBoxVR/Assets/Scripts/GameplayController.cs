using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameplayController : TagModularity
{
    float StartTimeMS; //the time the game started in ms

    float TimePassedMS; // the time passed in ms

    float HitThresholdMS = 180; // old val 250; the amount of time in ms a user is allowed to hit the note prior or after the target time. This time can not be larger than the time between notes

    float HitTargetTimeOffsetMS = 0; // the amount of time in ms to add to the notedata time value to get the target hit time

    float NoteAppearTimeMS = 800; // the amount of time in ms prior to the target hit time that a warning visual should appear (i.e. a dot starting its lerp from a start position to a target position)

    public float TimeScale = 1;

    //public TextAsset testjson;

    public GameObject NotePrefab;

    public int GameScore;

    public int GameStreak;

    public TMPro.TextMeshProUGUI ScoreText;

    public TMPro.TextMeshProUGUI StreakText;

    [System.Serializable]
    public class NoteObject
    {
        //public JsonDecoder.NoteData noteData;

        public uint time;

        public uint pad;

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

    public void GenerateNoteObjects ()
    {
        NotesForDifficulty midiNotes = GameManager.Instance.songLoader.songLibrary.songs[GameManager.Instance.SelectedSong].difficulties[GameManager.Instance.SongDifficulty];

        noteObjects = new NoteObject[midiNotes.notes.Count];

        ActiveNoteObjects = new List<NoteObject>();

        for (int i = 0; i < midiNotes.notes.Count; i++)
        {
            noteObjects[i] = new NoteObject();

            noteObjects[i].time = midiNotes.notes[i].time;

            noteObjects[i].pad = midiNotes.notes[i].pad;

            noteObjects[i].StartTime = noteObjects[i].time + HitTargetTimeOffsetMS - NoteAppearTimeMS;

            noteObjects[i].TargetTime = noteObjects[i].time + HitTargetTimeOffsetMS;

            noteObjects[i].ExpireTime = noteObjects[i].time + HitTargetTimeOffsetMS + HitThresholdMS;

            noteObjects[i].CanHitTime = noteObjects[i].time + HitTargetTimeOffsetMS - HitThresholdMS;

            //noteObjects[i].NoteVisual = GameObject.Instantiate(NotePrefab, new Vector3(noteObjects[i].pad*2, 0, 0), Quaternion.identity);
        }

    }

    private void Start()
    {

        StartCoroutine(StartGameDelayed());

        GameVisualsObject.transform.position = GameManager.Instance.BoardPosition;

        GameVisualsObject.transform.forward = GameManager.Instance.BoardForward;
    }

    IEnumerator StartGameDelayed()
    {
        yield return new WaitForSeconds(1.5f);

        GenerateNoteObjects();

        PunchPadVisuals ppv = GameVisualsObject.GetComponent<PunchPadVisuals>();

        ppv.SetNotes(ref noteObjects);

        ppv.SetGameplayController(this);

        

        AudioSource au = GameVisualsObject.GetComponent<AudioSource>();

        //au.clip = GameManager.Instance.songLoader.songLibrary.songs[GameManager.Instance.SelectedSong].Audio(0);

        au.clip = GameManager.Instance.SongAudioToPlay;

        au.Play();
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

                if (!noteObjects[i].beenHit)
                {
                    GameStreak = 0;
                    StreakText.text = "STREAK: " + GameStreak.ToString() + "x";
                }
            }

        }

        TimePassedMS += Time.deltaTime * 1000 * TimeScale;

        //DataText.text = TimePassedMS.ToString();

    }

    void UpdateVisualsOld()
    {
        foreach(NoteObject n in ActiveNoteObjects)
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
            if(n.pad == padIndex && !n.beenHit)
                notesOnPad.Add(n);
        }

        if (notesOnPad.Count == 0)
            return -1;

        NoteObject oldestNote = notesOnPad[0];

        foreach (NoteObject n in notesOnPad)
        {
            if(n.time < oldestNote.time)
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

        GameScore += (int)(oldestNote.hitAccuracy*100);

        GameStreak++;

        ScoreText.text = "SCORE: " + GameScore.ToString();

        StreakText.text = "STREAK: " + GameStreak.ToString() + "x";

        return oldestNote.hitAccuracy;


    }
    /*
    public void PadHit(int padIndex)
    {
        Debug.Log("Hit pad: " + padIndex);

        Dictionary<int, NoteObject> notesOnPad = new Dictionary<int, NoteObject>();

        for (int i = 0; i < noteObjects.Length; i++)
        {
            if (noteObjects[i].pad == padIndex && noteObjects[i].expired == false && noteObjects[i].beenHit == false && noteObjects[i].canHit == true)
            {
                notesOnPad.Add(i, noteObjects[i]);

            }

        }

        foreach (NoteObject n in ActiveNoteObjects)
        {

            notesOnPad.Add(i, noteObjects[i]);
        }

        if (notesOnPad.Count == 0)
            return;

        KeyValuePair<int, NoteObject> oldestNote;

        bool firstNote = true;

        foreach (KeyValuePair<int, NoteObject> n in notesOnPad)
        {
            if (n.Value.time < oldestNote.Value.time || firstNote)
            {
                oldestNote = n;

            }
            firstNote = false;
        }

        NoteHit(oldestNote.Key);


    }*/

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
