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

    public GameObject TrackPrefab;

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

        public Pad(Vector2 startPosition, Vector2 targetPosition, Vector2 endPosition, GameObject padPrefab, GameObject trackPrefab, int trackSegments, noteMovementTest aNoteMovement)
        {
            if (noteMovement == null)
                noteMovement = aNoteMovement;

            StartPosition = startPosition;

            TargetPosition = targetPosition;

            EndPosition = endPosition;

            PadObject = GameObject.Instantiate(padPrefab);

            PadObject.transform.position = noteMovement.GetPositionOnBag(TargetPosition);

            PadObject.transform.eulerAngles = noteMovement.GetRotationOnbag(TargetPosition);

            LightTrack = new GameObject[trackSegments];

            for (int i = 0; i < trackSegments; i++)
            {
                LightTrack[i] = Instantiate(trackPrefab);

                Vector2 pos = Vector2.Lerp(StartPosition, EndPosition, (float)i / trackSegments);

                LightTrack[i].transform.position = noteMovement.GetPositionOnBag(pos);

                LightTrack[i].transform.eulerAngles = noteMovement.GetRotationOnbag(pos);

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
        bagObject.UpdateTransform(GetPositionOnBag(bagObject.position), GetRotationOnbag(bagObject.position));

    }

    private void Awake()
    {
        

    }

    private void Start()
    {
        pi = Mathf.PI;

        BagSizeChanged();

        StartCoroutine(GeneratePads(StartRadius, TargetRadius, EndRadius));

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

    IEnumerator GeneratePads (float startRadius, float targetRadius, float endRadius)
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

            Pad newPad = new Pad(startPosition, targetPosition, endPosition, PadPrefab, TrackPrefab, TrackSegments, this);

            Pads.Add(newPad);
        }

        int k = 0;

        while (k < 2)
        {
            yield return null;
            k++;
        }
        

        Debug.Log("waited 2 frames");
        /*
        foreach (Pad p in Pads)
        {
            p.SetPadObject(GetPositionOnBag(p.TargetPosition), GetRotationOnbag(p.TargetPosition));

        }*/

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

        transform.eulerAngles = GetRotationOnbag(TestPosition2D);

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

    public Vector3 GetRotationOnbag (Vector2 position2D)
    {
        float angle = position2D.x * unitsToRadians * (180/pi);

        return new Vector3(0, -angle + 90, 0);
    }
}
