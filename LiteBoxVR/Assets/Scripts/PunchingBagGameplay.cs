using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandBagContact
{
    public bool inContact;

    public Transform handTransform;

    public HandBagContact(Transform handTransform)
    {
        this.handTransform = handTransform;

    }
    /*
    public bool BagContact(Transform bagOrigin, float bagRadius)
    {
        if (handTransform == null)
            return false;

        Vector2 hand = new Vector2(handTransform.position.x, handTransform.position.z);

        Vector2 bag = new Vector2(bagOrigin.position.x, bagOrigin.position.z);

        if ((hand - bag).sqrMagnitude < bagRadius * bagRadius)
            return true;

        return false;
    }*/
    
        // this will return true if bag contact has just started
    public bool BagContactStart(Transform bagOrigin, float hitboxRadius)
    {
        if (handTransform == null)
            return false;

        if (inContact)
        {
            return false;

        }

        Vector2 hand = new Vector2(handTransform.position.x, handTransform.position.z);

        Vector2 bag = new Vector2(bagOrigin.position.x, bagOrigin.position.z);

        if ((hand - bag).sqrMagnitude < hitboxRadius * hitboxRadius)
        {
            inContact = true;
            return true;
        }

        inContact = false;

        return false;
    }



}

public class PunchingBagGameplay : ExampleGameplayChild
{
    public float bagRadius = 0.5f;

    float bagCircumfernce;

    float unitsToRadians;

    float pi;

    public GameObject PadPrefab;

    public GameObject TrackEndPrefab;

    public GameObject TrackMiddlePrefab;

    public float StartRadius;

    public float TargetRadius;

    public float EndRadius;

    public int TrackSegments;

    public float[] TrackAngleOffsets;

    HandBagContact RightHandContact;

    HandBagContact LeftHandContact;

    public float HitboxRadiusMultiplier = 1.1f;

    public float PadRadius = 0.07f;

    // punching pad should illuminate with each hit
    // could illuminate with the color of the hit note and brightness relative to the hit velocity

    // Debug Stuff
    string DebugInfo;
    public TMPro.TextMeshPro DebugText;

    [System.Serializable]
    public class Pad
    {
        static PunchingBagGameplay punchingbagGameplay;

        public Vector2 StartPosition;

        public Vector2 TargetPosition;

        public Vector2 EndPosition;

        public GameObject gameObject;

        public GameObject[] LightTrack;

        public Pad(Vector2 startPosition, Vector2 targetPosition, Vector2 endPosition, GameObject padPrefab, GameObject trackEndPrefab, GameObject trackMiddlePrefab, int trackSegments, PunchingBagGameplay aPunchingBagGameplay, float angle, int padIndex)
        {
            if (punchingbagGameplay == null)
                punchingbagGameplay = aPunchingBagGameplay;

            StartPosition = startPosition;

            TargetPosition = targetPosition;

            EndPosition = endPosition;

            gameObject = GameObject.Instantiate(padPrefab);

            gameObject.name = "Pad " + padIndex;

            gameObject.transform.position = punchingbagGameplay.GetPositionOnBag(TargetPosition);

            gameObject.transform.eulerAngles = punchingbagGameplay.GetRotationOnBag(TargetPosition);

            //generate light track

            LightTrack = new GameObject[trackSegments + 1];

            float trackAngle = 240 - (padIndex * 60);

            //track start cap
            LightTrack[0] = Instantiate(trackEndPrefab);
            LightTrack[0].transform.position = punchingbagGameplay.GetPositionOnBag(startPosition);
            LightTrack[0].transform.eulerAngles = punchingbagGameplay.GetRotationOnBag(StartPosition);
            LightTrack[0].transform.localEulerAngles = new Vector3(LightTrack[0].transform.localEulerAngles.x, LightTrack[0].transform.localEulerAngles.y, trackAngle);
            LightTrack[0].transform.parent = gameObject.transform;

            //track end cap
            LightTrack[trackSegments] = Instantiate(trackEndPrefab);
            LightTrack[trackSegments].transform.position = punchingbagGameplay.GetPositionOnBag(EndPosition);
            LightTrack[trackSegments].transform.eulerAngles = punchingbagGameplay.GetRotationOnBag(EndPosition);
            LightTrack[trackSegments].transform.localEulerAngles = new Vector3(LightTrack[trackSegments].transform.localEulerAngles.x, LightTrack[trackSegments].transform.localEulerAngles.y, trackAngle + 180);
            LightTrack[trackSegments].transform.parent = gameObject.transform;

            float trackLength = Vector2.Distance(StartPosition, EndPosition);

            for (int i = 1; i < trackSegments; i++)
            {
                LightTrack[i] = Instantiate(trackMiddlePrefab);

                Vector2 pos = Vector2.Lerp(StartPosition, EndPosition, (float)i / trackSegments);

                //Debug.Log("lerp: " + (float)i / trackSegments);

                LightTrack[i].transform.position = punchingbagGameplay.GetPositionOnBag(pos);

                LightTrack[i].transform.eulerAngles = punchingbagGameplay.GetRotationOnBag(pos);

                LightTrack[i].transform.localEulerAngles = new Vector3(LightTrack[i].transform.localEulerAngles.x, LightTrack[i].transform.localEulerAngles.y, trackAngle);

                LightTrack[i].transform.parent = gameObject.transform;

            }

            Debug.Log("pad constructor");

        }

        public void UpdateLightTrack(int padIndex, float offset, int trackSegments)
        {

            float trackAngle = offset;

            //track start cap
            LightTrack[0].transform.parent = null;
            LightTrack[0].transform.position = punchingbagGameplay.GetPositionOnBag(StartPosition);
            LightTrack[0].transform.eulerAngles = punchingbagGameplay.GetRotationOnBag(StartPosition);
            LightTrack[0].transform.localEulerAngles = new Vector3(LightTrack[0].transform.localEulerAngles.x, LightTrack[0].transform.localEulerAngles.y, trackAngle);
            LightTrack[0].transform.parent = gameObject.transform;

            //track end cap
            LightTrack[trackSegments].transform.parent = null;
            LightTrack[trackSegments].transform.position = punchingbagGameplay.GetPositionOnBag(EndPosition);
            LightTrack[trackSegments].transform.eulerAngles = punchingbagGameplay.GetRotationOnBag(EndPosition);
            LightTrack[trackSegments].transform.localEulerAngles = new Vector3(LightTrack[trackSegments].transform.localEulerAngles.x, LightTrack[trackSegments].transform.localEulerAngles.y, trackAngle + 180);
            LightTrack[trackSegments].transform.parent = gameObject.transform;

            float trackLength = Vector2.Distance(StartPosition, EndPosition);

            for (int i = 1; i < trackSegments; i++)
            {
                LightTrack[i].transform.parent = null;

                Vector2 pos = Vector2.Lerp(StartPosition, EndPosition, (float)i / trackSegments);

                Debug.Log("lerp: " + (float)i / trackSegments);

                LightTrack[i].transform.position = punchingbagGameplay.GetPositionOnBag(pos);

                LightTrack[i].transform.eulerAngles = punchingbagGameplay.GetRotationOnBag(pos);

                LightTrack[i].transform.localEulerAngles = new Vector3(LightTrack[i].transform.localEulerAngles.x, LightTrack[i].transform.localEulerAngles.y, trackAngle);

                LightTrack[i].transform.parent = gameObject.transform;

            }

        }

    }

    [SerializeField]
    public List<Pad> Pads;

    [System.Serializable]
    public class BagObject
    {
        public Vector2 position;

        public GameObject gameObj;

        static noteMovementTest noteMovement;

        public BagObject(Vector2 position, GameObject prefab, noteMovementTest aNoteMovement)
        {
            if (noteMovement == null)
                noteMovement = aNoteMovement;

            this.position = position;

            gameObj = Instantiate(prefab);

            gameObj.name = "bagObject";

        }

        public void SetPosition(Vector2 position)
        {
            this.position = position;

            gameObj.transform.position = noteMovement.GetPositionOnBag(position);

            gameObj.transform.eulerAngles = noteMovement.GetRotationOnBag(position);

        }

    }

    public override void Start()
    {
        base.Start();

        pi = Mathf.PI;

        bagRadius = transform.localScale.x * 0.5f;

        SetTargetRadius();

        BagSizeChanged();

        StartCoroutine(GeneratePadsDelayed());

        //GenerateBagPixels(0.05f);
        if (ArmPositioning.LeftHandInstance != null)
            LeftHandContact = new HandBagContact(ArmPositioning.LeftHandInstance.transform);

        if (ArmPositioning.RightHandInstance != null)
            RightHandContact = new HandBagContact(ArmPositioning.RightHandInstance.transform);
    }

    IEnumerator GeneratePadsDelayed()
    {
        yield return new WaitForSeconds(3);

        Debug.Log("about to generate pads");
        GeneratePads(StartRadius, TargetRadius, EndRadius);

    }

    public void SetTargetRadius()
    {
        float travelDistance = EndRadius - StartRadius;

        float diff = Mathf.InverseLerp(NoteAppearTimeMS, HitThresholdMS, 0);

        TargetRadius = EndRadius * diff;
    }

    public override void Update()
    {
        base.Update();

        BagSizeChanged();

        UpdateNoteObjects();
    }

    private void LateUpdate()
    {
        DebugInfo = "";
    }

    private void UpdateNoteObjects()
    {
        foreach (KeyValuePair<uint, NoteObject> pair in NoteObjectDict)
        {
            //Vector3 position = Vector3.Lerp(Vector3.zero, Vector3.one, pair.Value.lerpProgress);

            // pair.Value.SetPosition(position);

            Vector2 notePos2D = Vector2.Lerp(Pads[(int)pair.Value.pad].StartPosition, Pads[(int)pair.Value.pad].EndPosition, pair.Value.lerpProgress);

            pair.Value.SetPosition(GetPositionOnBag(notePos2D));
        }
        
    }

    private void FixedUpdate()
    {

        CheckHandContact();

    }

    void CheckHandContact()
    {

        if (LeftHandContact.BagContactStart(transform, bagRadius*HitboxRadiusMultiplier))
        {
            CheckPadContact(LeftHandContact.handTransform.position);
        }
        if (RightHandContact.BagContactStart(transform, bagRadius * HitboxRadiusMultiplier))
        {
            CheckPadContact(RightHandContact.handTransform.position);
        }

    }

    void CheckPadContact(Vector3 handPosition)
    {
        for (int i = 0; i < Pads.Count; i++)
        {
            if ((Pads[i].gameObject.transform.position - handPosition).sqrMagnitude < PadRadius * PadRadius)
            {
                PadHit(i);
                AddDebugLine("Pad " + i + "Contact");
            }

        }
 
    }

    public override void NoteObjectHit(uint id)
    {
        

        // do particle here 
        //NoteObjectDict[id].
    }

    void AddDebugLine(string entry)
    {
        DebugInfo += entry + "\n";
        DebugText.text = DebugInfo;

    }

    void GeneratePads(float startRadius, float targetRadius, float endRadius)
    {
        Debug.Log("Generate Pads");

        Pads = new List<Pad>();

        for (int i = 0; i < 6; i++)
        {
            float angle = pi / 3 * i + pi / 6;

            Debug.Log("angle: " + angle);

            Vector2 origin = Vector2.zero;

            Vector2 startPosition = PointOnCircle(startRadius, origin, angle);

            Vector2 targetPosition = PointOnCircle(targetRadius, origin, angle);

            Vector2 endPosition = PointOnCircle(endRadius, origin, angle);

            Pad newPad = new Pad(startPosition, targetPosition, endPosition, PadPrefab, TrackEndPrefab, TrackMiddlePrefab, TrackSegments, this, angle, i);

            Pads.Add(newPad);
        }

    }

    Vector2 PointOnCircle(float radius, Vector2 origin, float angle)
    {
        //Debug.Log(new Vector2(origin.x + radius * Mathf.Cos(angle), origin.y + radius * Mathf.Sin(angle)));
        return new Vector2(origin.x + radius * Mathf.Cos(angle), origin.y + radius * Mathf.Sin(angle));
    }

    
    void BagSizeChanged()
    {
        bagCircumfernce = 2 * pi * bagRadius;

        unitsToRadians = (2 * pi) / bagCircumfernce;
    }

    public Vector3 GetPositionOnBag(Vector2 position2D)
    {
        float angle = (position2D.x * unitsToRadians) - ((180 / pi)*transform.eulerAngles.y);

        return new Vector3(transform.position.x + bagRadius * Mathf.Cos(angle), transform.position.y + position2D.y, transform.position.z + bagRadius * Mathf.Sin(angle));
    }

    public Vector3 GetRotationOnBag(Vector2 position2D)
    {
        float angle = position2D.x * unitsToRadians * (180 / pi);

        return new Vector3(0, -angle + 90 + transform.eulerAngles.y, 0);
    }

    public Vector3 GetPreservedRotationOnBag(Vector2 position2D, Vector3 OrigionalRotation)
    {
        float angle = position2D.x * unitsToRadians * (180 / pi);

        return new Vector3(OrigionalRotation.x, -angle + 90, OrigionalRotation.z);
    }
}
