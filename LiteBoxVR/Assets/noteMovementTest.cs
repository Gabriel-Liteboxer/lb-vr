using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class noteMovementTest : MonoBehaviour
{
    public Vector2 TestPosition2D;

    public float speed = 1;

    public float bagRadius = 0.5f;

    float bagCircumfernce;

    float unitsToRadians;

    float pi;

    public Transform bagOrigin;

    public float velocity;

    public GameObject PadPrefab;

    public GameObject TrackEndPrefab;

    public GameObject TrackMiddlePrefab;

    public float StartRadius;

    public float TargetRadius;

    public float EndRadius;

    public int TrackSegments;

    // punching pad should illuminate with each hit
    // could illuminate with the color of the hit note and brightness relative to the hit velocity


    [System.Serializable]
    public class Pad
    {
        static noteMovementTest noteMovement;

        public Vector2 StartPosition;

        public Vector2 TargetPosition;

        public Vector2 EndPosition;

        public GameObject PadObject;

        public GameObject[] LightTrack;

        public Pad(Vector2 startPosition, Vector2 targetPosition, Vector2 endPosition, GameObject padPrefab)
        {
            StartPosition = startPosition;

            TargetPosition = targetPosition;

            EndPosition = endPosition;

            PadObject = GameObject.Instantiate(padPrefab);

        }

        public Pad(Vector2 startPosition, Vector2 targetPosition, Vector2 endPosition, GameObject padPrefab, GameObject trackEndPrefab, GameObject trackMiddlePrefab, int trackSegments, noteMovementTest aNoteMovement, float angle, int padIndex)
        {
            if (noteMovement == null)
                noteMovement = aNoteMovement;

            StartPosition = startPosition;

            TargetPosition = targetPosition;

            EndPosition = endPosition;

            PadObject = GameObject.Instantiate(padPrefab);

            PadObject.name = "Pad " + padIndex;

            PadObject.transform.position = noteMovement.GetPositionOnBag(TargetPosition);

            PadObject.transform.eulerAngles = noteMovement.GetRotationOnBag(TargetPosition);

            //generate light track

            LightTrack = new GameObject[trackSegments + 1];

            //track start cap
            LightTrack[0] = Instantiate(trackEndPrefab);
            LightTrack[0].transform.position = noteMovement.GetPositionOnBag(startPosition);
            LightTrack[0].transform.eulerAngles = noteMovement.GetRotationOnBag(StartPosition);
            LightTrack[0].transform.parent = PadObject.transform;

            //track end cap
            LightTrack[trackSegments] = Instantiate(trackEndPrefab);
            LightTrack[trackSegments].transform.position = noteMovement.GetPositionOnBag(EndPosition);
            LightTrack[trackSegments].transform.eulerAngles = noteMovement.GetRotationOnBag(EndPosition);
            LightTrack[trackSegments].transform.parent = PadObject.transform;

            float trackLength = Vector2.Distance(StartPosition, EndPosition);

            for (int i = 1; i < trackSegments; i++)
            {
                LightTrack[i] = Instantiate(trackMiddlePrefab);

                Vector2 pos = Vector2.Lerp(StartPosition, EndPosition, (float)i / trackSegments);

                Debug.Log("lerp: " + (float)i / trackSegments);

                LightTrack[i].transform.position = noteMovement.GetPositionOnBag(pos);

                LightTrack[i].transform.eulerAngles = noteMovement.GetRotationOnBag(pos);

                //LightTrack[i].transform.localScale = new Vector3(1, trackLength / trackSegments, 1);



                LightTrack[i].transform.localEulerAngles = new Vector3(LightTrack[i].transform.localEulerAngles.x, LightTrack[i].transform.localEulerAngles.y, (angle*180/Mathf.PI +90)/2);



                //LightTrack[i].transform.right = Vector3.Normalize(LightTrack[i].transform.position - LightTrack[i-1].transform.position);

                //LightTrack[i].transform.eulerAngles = noteMovement.GetPreservedRotationOnBag(pos, LightTrack[i].transform.eulerAngles);

                LightTrack[i].transform.parent = PadObject.transform;

            }

        }
        /*
        public void SetPadObject(Vector3 position, Vector3 eulerAngles)
        {
            PadObject.transform.position = position;

            PadObject.transform.eulerAngles = eulerAngles;

        }*/

    }

    [SerializeField]
    public List<Pad> Pads;

    [System.Serializable]
    public class BagObject
    {
        public Vector2 position;

        public GameObject gameObj;

        public BagObject(Vector2 position, GameObject prefab)
        {
            this.position = position;

            gameObj = Instantiate(prefab);

            gameObj.name = "bagObject";

        }

        public void UpdateTransform (Vector3 position, Vector3 eulerAngles)
        {
            gameObj.transform.position = position;

            gameObj.transform.eulerAngles = eulerAngles;

            gameObj.transform.localScale = Vector3.one*10;
        }
    }

    [SerializeField]
    public List<BagObject> BagPixels;

    public void UpdateBagObject (ref BagObject bagObject)
    {
        bagObject.UpdateTransform(GetPositionOnBag(bagObject.position), GetRotationOnBag(bagObject.position));

    }

    

    private void Start()
    {
        pi = Mathf.PI;

        //GameManager.Instance

        BagSizeChanged();

        GeneratePads(StartRadius, TargetRadius, EndRadius);

        //GenerateBagPixels(0.05f);
    }

    void Update()
    {
        BagSizeChanged();

        MoveBallOnbag();

        /*
        foreach(BagObject b in BagPixels)
        {
            BagObject bg = b;
            UpdateBagObject(ref bg);
        }*/
    }

    void GenerateBagPixels(float padding)
    {
        BagPixels = new List<BagObject>();

        int numX = (int)(bagCircumfernce / padding);

        float bagHeight = 2;

        int numY = (int)(bagHeight / padding);

        Debug.Log(bagCircumfernce/padding);

        for (int x = 0; x < numX; x++)
        {
            for (int y = 0; y < numY; y++)
            {
                Vector2 position = new Vector2(x * padding, y*padding - bagHeight/2);

                BagObject bi = new BagObject(position, PadPrefab);

                BagPixels.Add(bi);

            }
            

        }

    }

    void GeneratePads (float startRadius, float targetRadius, float endRadius)
    {
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
        Debug.Log(new Vector2(origin.x + radius * Mathf.Cos(angle), origin.y + radius * Mathf.Sin(angle)));
        return new Vector2(origin.x + radius * Mathf.Cos(angle), origin.y + radius * Mathf.Sin(angle));
    }

    void MoveBallOnbag()
    {
        Vector2 movement = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical")) * speed * Time.deltaTime;

        TestPosition2D += movement;

        transform.position = GetPositionOnBag(TestPosition2D);

        transform.eulerAngles = GetRotationOnBag(TestPosition2D);

    }


    void BagSizeChanged()
    {
        bagCircumfernce = 2 * pi * bagRadius;

        unitsToRadians = (2 * pi) / bagCircumfernce;
    }

    public Vector3 GetPositionOnBag(Vector2 position2D)
    {
        float angle = position2D.x * unitsToRadians;

        return new Vector3(bagOrigin.position.x + bagRadius * Mathf.Cos(angle), bagOrigin.position.y + position2D.y, bagOrigin.position.z + bagRadius * Mathf.Sin(angle));
    }

    public Vector3 GetRotationOnBag (Vector2 position2D)
    {
        float angle = position2D.x * unitsToRadians * (180/pi);

        return new Vector3(0, -angle + 90, 0);
    }

    public Vector3 GetPreservedRotationOnBag(Vector2 position2D, Vector3 OrigionalRotation)
    {
        float angle = position2D.x * unitsToRadians * (180 / pi);

        return new Vector3(OrigionalRotation.x, -angle + 90, OrigionalRotation.z);
    }
}
