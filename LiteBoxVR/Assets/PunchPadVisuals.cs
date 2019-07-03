using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PunchPadVisuals : MonoBehaviour
{
    public GameObject NotePrefab;

    public GameplayController.NoteObject[] Notes;

    GameObject[] NoteVisuals;

    private GameObject[] NoteStartpoint;

    private GameObject[] NoteEndpoint;

    private int NumberOfPads = 6;
    
    public bool[,] PadContact;

    public float StartRadius;

    public float EndRadius;

    public float PadSurfaceRadius;

    public float PadSurfaceHeight;

    public Transform LeftController;

    public Transform RightController;

    public GameplayController GameCont;

    class PadLEDRing
    {
        public GameObject[] LEDs;

    }

    public int RingLEDCount;

    PadLEDRing[] RingLEDs;

    public Material BaseMaterial;

    Material[] RingLEDColors;

    public Color PerfectColor;

    public Color MissColor;

    public bool EnableDebugLines = true;

    public void SetGameplayController(GameplayController newGameCont)
    {
        GameCont = newGameCont;

    }

    public void SetNotes(ref GameplayController.NoteObject[] newNotes)
    {
        Notes = newNotes;

        NoteVisuals = new GameObject[Notes.Length];

        NoteStartpoint = new GameObject[NumberOfPads];

        NoteEndpoint = new GameObject[NumberOfPads];

        PadContact = new bool[NumberOfPads, 2];

        RingLEDs = new PadLEDRing[NumberOfPads];

        RingLEDColors = new Material[RingLEDCount];


        for (int i = 0; i < NoteStartpoint.Length; i++)
        {
            //PadContact[i] = false;

            NoteStartpoint[i] = GameObject.Instantiate(NotePrefab, gameObject.transform);

            NoteEndpoint[i] = GameObject.Instantiate(NotePrefab, gameObject.transform);

            float angleRad = (i * (3.1416f / 3)) + (3.1416f / 6);

            Vector3 Origin = new Vector3(0, 0, -0.01f);

            NoteStartpoint[i].transform.localPosition = new Vector3(Origin.x + StartRadius * Mathf.Cos(angleRad), Origin.y + StartRadius * Mathf.Sin(angleRad), Origin.z);

            NoteEndpoint[i].transform.localPosition = new Vector3(Origin.x + EndRadius * Mathf.Cos(angleRad), Origin.y + EndRadius * Mathf.Sin(angleRad), Origin.z);
        }


        for (int i = 0; i < RingLEDCount; i++)
        {
            RingLEDColors[i] = new Material(BaseMaterial);

            float colorLerp = Mathf.InverseLerp(RingLEDCount, 0, i);

            Vector3 newColorVector3 = Vector3.Lerp(new Vector3(MissColor.r, MissColor.g, MissColor.b), new Vector3(PerfectColor.r, PerfectColor.g, PerfectColor.b), colorLerp);

            Color newColor = new Color(newColorVector3.x, newColorVector3.y, newColorVector3.z);

            RingLEDColors[i].color = newColor;

        }


        for (int i = 0; i < RingLEDs.Length; i++)
        {
            RingLEDs[i] = new PadLEDRing();

            RingLEDs[i].LEDs = new GameObject[RingLEDCount];

            for (int c = 0; c < RingLEDs[i].LEDs.Length; c++)
            {
                RingLEDs[i].LEDs[c] = GameObject.Instantiate(NotePrefab, gameObject.transform);

                float angleRad = c * (2 * 3.1416f)/RingLEDCount + (i * (3.1416f / 3)) + (3.1416f*1.1f);

                float ringRadius = PadSurfaceRadius;

                Vector3 Origin = NoteEndpoint[i].transform.localPosition + new Vector3(0, 0, 0.002f);

                RingLEDs[i].LEDs[c].transform.localPosition = new Vector3(Origin.x + ringRadius * Mathf.Cos(angleRad), Origin.y + ringRadius * Mathf.Sin(angleRad), Origin.z);

                RingLEDs[i].LEDs[c].GetComponentInChildren<Renderer>().material = RingLEDColors[c];
            }


        }


        for (int i = 0; i < Notes.Length; i++)
        {
            NoteVisuals[i] = GameObject.Instantiate(NotePrefab, gameObject.transform);

            NoteVisuals[i].SetActive(false);
        }
    }

    private void Update()
    {
        if (Notes.Length > 0)
        {
            for (int i = 0; i < Notes.Length; i++)
            {

                if (Notes[i].expired)
                {
                    NoteVisuals[i].SetActive(false);

                }
                else if (Notes[i].born)
                {
                    NoteVisuals[i].SetActive(true);

                }
                 

                NoteVisuals[i].transform.position = Vector3.Lerp(NoteStartpoint[Notes[i].pad].transform.position, NoteEndpoint[Notes[i].pad].transform.position, Notes[i].LerpProgress);


            }
        }

        /*
        for (int i = 0; i < NoteStartpoint.Length; i++)
        {

            float angleRad = (i * (3.1416f / 3)) + (3.1416f / 6);

            Vector3 Origin = new Vector3(0, 0, -0.005f);

            NoteStartpoint[i].transform.localPosition = new Vector3(Origin.x + StartRadius * Mathf.Cos(angleRad), Origin.y + StartRadius * Mathf.Sin(angleRad), Origin.z);

            NoteEndpoint[i].transform.localPosition = new Vector3(Origin.x + EndRadius * Mathf.Cos(angleRad), Origin.y + EndRadius * Mathf.Sin(angleRad), Origin.z);
        }
        */

        CheckPadHit();
    }

    void CheckPadHit()
    {
        for (int i = 0; i < NoteEndpoint.Length; i++)
        {
            if(WithinCylinder(LeftController.position, NoteEndpoint[i].transform.localPosition, PadSurfaceRadius, PadSurfaceHeight))
            {
                SetContactState(true, i, 0);

            }
            else
            {
                SetContactState(false, i, 0);

            }

            if (WithinCylinder(RightController.position, NoteEndpoint[i].transform.localPosition, PadSurfaceRadius, PadSurfaceHeight))
            {
                SetContactState(true, i, 1);

            }
            else
            {
                SetContactState(false, i, 1);

            }

        }

    }

    void SetContactState(bool isContact, int padIndex, int controllerIndex)
    {
        
        if(isContact)
        {
            if (!PadContact[padIndex, controllerIndex])
            {
                NotifyOfHit(padIndex, 0);

            }

            PadContact[padIndex, controllerIndex] = true;
        }
        else
        {
            PadContact[padIndex, controllerIndex] = false;

        }

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

        if(EnableDebugLines)
            Debug.DrawLine(TargetPos, originWorld4);


        if (SqrDistance < outerRadius * outerRadius && TargetPosLocal.z > OriginPosLocal.z - height / 2 && TargetPosLocal.z < OriginPosLocal.z + height / 2)
            return true;

        return false;
    }

    void NotifyOfHit(int padIndex, float controllerVelocity)
    {
        GameCont.PadHit(padIndex);

        /*
        string contName = "right controller";

        if(isLeftController)
            contName = "left controller";*/

        //Debug.Log("In contact with " + "controller" + " on pad " + padIndex);

    }

}
