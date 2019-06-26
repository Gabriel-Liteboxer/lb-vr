using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleJSON;

public class NoteManager1 : MonoBehaviour
{
    public int Score = 0;

    public AudioClip SongClip;

    public TextAsset SongKeyframes;

    private int SongTempo;

    public GameObject NotePrefab;

    public GameObject PunchPadPrefab;

    public GameObject NotePopParticlePrefab;

    public GameObject ShockwaveParticlePrefab;

    public GameObject[] PunchPads;

    public Transform[] NoteEndTarget;

    public Transform NoteStartPosition;

    public Vector3[] NoteEndStartingPosition;

    public TextMesh ScoreText;

    public float NoteTimer = 0;

    public float NoteSpeed = 1;

    public int NoteTimingOffset;

    public float PerfectHitThreshold = 0.25f;

    public float LeftContVibrationTimer;

    public float RightContVibrationTimer;

    public float HitVibrationFrequency = 0.3f;

    public float HitVibrationAmplitude = 0.2f;

    private ControllerVibrationManager VibrationManager;

    public bool GameStarted = false;

    public class Note
    {
        public int pad;

        public int time;

        public GameObject NoteObj;

        public float lerpProgress = 0;

        public bool Active = false;

        public bool Born = false;

        public Transform EndTarget;

    }

    public Note[] Notes;
    
    public void StartGame()
    {
        if (GameStarted)
            return;

        GameStarted = true;

        VibrationManager = GetComponent<ControllerVibrationManager>();

        PunchPads = new GameObject[6];

        NoteEndTarget = new Transform[6];

        NoteEndStartingPosition = new Vector3[6];

        for (int i = 0; i < PunchPads.Length; i++)
        {
            PunchPads[i] = GameObject.Instantiate(PunchPadPrefab, transform.position, transform.rotation * Quaternion.Euler(i*60, 0, 0));

            //PunchPads[i].GetComponent<PunchPadHitDetection>().noteMgr = this;

            PunchPads[i].GetComponent<PunchPadHitDetection>().PadIndex = i;

            NoteEndTarget[i] = PunchPads[i].GetComponentInChildren<NoteMeshScr>().gameObject.transform;

            NoteEndStartingPosition[i] = NoteEndTarget[i].position;
        }
        
        SimpleJSON.JSONNode data = SimpleJSON.JSON.Parse(SongKeyframes.text);
        Debug.Log("Tempo " + data["tempo"]);

        SongTempo = data["tempo"];

        Notes = new Note[data["tracks"].Count];

        Debug.Log("Tracks Count " + data["tracks"].Count);

        for (int i = 0; i < data["tracks"].Count; i++)
        {
            Notes[i] = new Note();

            Notes[i].NoteObj = GameObject.Instantiate(NotePrefab, Vector3.zero, Quaternion.identity);

            Notes[i].NoteObj.SetActive(false);
            
            Notes[i].pad = data["tracks"][i]["p"];

            Notes[i].time = data["tracks"][i]["t"];

            Notes[i].EndTarget = NoteEndTarget[Notes[i].pad];

            Debug.Log("note pad " + i + ": " + Notes[i].pad);

            Debug.Log("note time " + i + ": " + Notes[i].time);

        }

        GetComponent<AudioSource>().clip = SongClip;
        GetComponent<AudioSource>().Play(); //just didn't want to hear this every time I hit play

        ScoreText.text = "Score: " + Score.ToString();
    }

    private void Update()
    {
        if(GameStarted)
        {
            UpdateNotes();

        }


    }


    private void UpdateNotes()
    {
        NoteTimer += Time.deltaTime*1000;

        if (RightContVibrationTimer > 0)
        {
            VibrationManager.AddVibration(false, HitVibrationFrequency, HitVibrationAmplitude);
            RightContVibrationTimer -= Time.deltaTime;
        }
        

        if(LeftContVibrationTimer > 0)
        {
            VibrationManager.AddVibration(true, HitVibrationFrequency, HitVibrationAmplitude);
            LeftContVibrationTimer -= Time.deltaTime;
        }
        

        foreach (Note n in Notes)
        {
            if (NoteTimer >= n.time + NoteTimingOffset && n.Born == false)
            {
                n.Born = true;
                n.Active = true;
                n.NoteObj.SetActive(true);
            }

            if (n.Active)
            {
                if(n.lerpProgress >= 1+PerfectHitThreshold)
                {
                    n.Active = false;
                    n.NoteObj.SetActive(false);
                }

                n.NoteObj.transform.position = Vector3.Lerp(NoteStartPosition.position, n.EndTarget.position, n.lerpProgress);

                n.lerpProgress += NoteSpeed * Time.deltaTime;
                
            }

        }

        for(int i = 0; i < 6; i++)
        {
            float vibvalue = Vector3.Distance(NoteEndTarget[i].position, NoteEndStartingPosition[i]);
            VibrationManager.AddVibration(true, vibvalue*3, vibvalue*3);

        }
        
    }

    public void PadHit(int pIndex, bool isLeftController)
    {
        GameObject sPart = Instantiate(ShockwaveParticlePrefab, NoteEndTarget[pIndex]);

        sPart.transform.localPosition = Vector3.zero;

        sPart.transform.rotation = NoteEndTarget[pIndex].rotation;

        sPart.GetComponent<ParticleSystem>().Play();

        Destroy(sPart, 1f);

        if (isLeftController)
            LeftContVibrationTimer = 0.1f;
        else
            RightContVibrationTimer = 0.1f;
        

        foreach (Note n in Notes)
        {
            if (n.Active == false)
                continue;

            if(n.pad == pIndex)//need to make it so that one hit only pops the first particle
            {
                n.Active = false;
                n.NoteObj.SetActive(false);

                GameObject nPart = Instantiate(NotePopParticlePrefab, n.NoteObj.transform.position, n.NoteObj.transform.rotation);

                nPart.GetComponent<ParticleSystem>().Play();

                Destroy(nPart, 2f);

                if(Mathf.Abs(1-n.lerpProgress) < PerfectHitThreshold)
                {
                    Score++;
                    ScoreText.text = "Score: " + Score.ToString();
                }

                Debug.Log("did the thing " + (1 - n.lerpProgress).ToString());

                break;//this will only work if the json list is ordered by the start time of each particle
            }

        }

    }


}
