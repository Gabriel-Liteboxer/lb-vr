using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BagDragCalibration : MonoBehaviour
{
    //public LineRenderer lineRenderer;

    public float TimerCurrent;

    public float TimerMax;

    private Transform HandLTarget;

    private Transform HandL;

    //public Transform testOne;

    //public Transform testTwo;

    //public Transform testThree;

    public float StartAngle;

    private Sprite[] circleSprites;

    public SpriteRenderer TimerCircle;

    public SpriteRenderer AlignmentCircle;

    public bool DrawingSurface;

    public bool SurfaceDrawn;

    public LineRenderer SurfacePath;

    public List<Vector3> Anchors;

    public float AnchorOffset;

    public int AnchorSetNumber;

    public Transform PunchingBagTransform;

    [System.Serializable]
    public class AnchorSet
    {
        public Vector3 first;

        public Vector3 second;

        public Vector3 third;

        public float bagRadius;

        public Vector2 circumcenter;

        public AnchorSet(Vector3 first, Vector3 second, Vector3 third)
        {
            this.first = first;

            this.second = second;

            this.third = third;

        }

        public Vector3[] GetAnchors()
        {
            return new Vector3[] { first, second, third };
        }

    }

    public List<AnchorSet> AnchorSets;

    private void Start()
    {
        circleSprites = Resources.LoadAll<Sprite>("CircleLoading");

        HandLTarget = new GameObject().transform;

        HandL = ArmPositioning.LeftHandInstance.transform;
    }

    void Update()
    {
        //UpdateTestObjects();

        transform.position = HandL.position;

        transform.forward = Camera.main.transform.forward;

        UpdateHandTarget();

        if(!DrawingSurface)
        {
            StartSurfaceDraw();

        }
        else
        {
            DrawSurface(HandL.position);

        }
        
    }

    void DrawSurface(Vector3 handPosition)
    {
        if (SurfaceDrawn)
            return;

        if(Anchors.Count < 1)
        {
            Anchors.Add(handPosition);

        }
        else if ((handPosition - Anchors[Anchors.Count-1]).sqrMagnitude > AnchorOffset * AnchorOffset)
        {
            Anchors.Add(handPosition);

        }

        SurfacePath.positionCount = Anchors.Count;

        SurfacePath.SetPositions(Anchors.ToArray());

        if (Anchors.Count >= AnchorSetNumber * 3)
        {
            /*
            for (int i = 0; i < AnchorSetNumber; i++)
            {
                // instead, divide anchors into thirds, group of anchor 1s, anchor 2s, and anchor 3s
                AnchorSets.Add(new AnchorSet(Anchors[i*3], Anchors[1*3+1], Anchors[i*3+2]));

            }
            */
            List<Vector3> firstAnchors = new List<Vector3>();

            List<Vector3> secondAnchors = new List<Vector3>();

            List<Vector3> thirdAnchors = new List<Vector3>();

            for (int i = 0; i < Anchors.Count; i++)
            {
                if (i < AnchorSetNumber)
                {
                    firstAnchors.Add(Anchors[i]);

                }
                else if (i < AnchorSetNumber*2)
                {
                    secondAnchors.Add(Anchors[i]);

                }
                else
                {
                    thirdAnchors.Add(Anchors[i]);

                }

            }

            for (int i = 0; i < AnchorSetNumber; i++)
            {
                // instead, divide anchors into thirds, group of anchor 1s, anchor 2s, and anchor 3s
                AnchorSet newAnchorSet = new AnchorSet(firstAnchors[i], secondAnchors[i], thirdAnchors[i]);

                newAnchorSet.circumcenter = GetAnchorSetCircumcenter(newAnchorSet);

                AnchorSets.Add(newAnchorSet);

            }

            // reduce down anchor sets to half to remove anomalies
            // each loop the "least average" anchor set is removed, which creates a newer, refined average

            //RefineAnchorSets();

            SetBagPosition();

            SurfaceDrawn = true;

            // play confirmation sound

            Debug.Log("anchors count \n" + "first: " + firstAnchors.Count + "\n" + "second: " + secondAnchors.Count + "\n" + "third: " + thirdAnchors.Count);

        }

    }

    void SetBagPosition()
    {
        Vector3 averagePosition = Vector3.zero;

        foreach (AnchorSet a in AnchorSets)
        {
            averagePosition += new Vector3(a.circumcenter.x, a.second.y, a.circumcenter.y);

        }

        averagePosition /= AnchorSets.Count;

        PunchingBagTransform.position = averagePosition;

        //float distFromCenter = Vector2.Distance(new Vector2(AnchorSets[0].first.x, AnchorSets[0].first.z), AnchorSets[0].circumcenter);

        float distFromCenter = 0;

        foreach (AnchorSet a in AnchorSets)
        {
            distFromCenter += Vector2.Distance(new Vector2(a.first.x, a.first.z), a.circumcenter);

        }

        distFromCenter /= AnchorSets.Count;

        PunchingBagTransform.transform.localScale = Vector3.one * distFromCenter * 2;

        Vector3 averageAnchorPosition = Vector3.zero;

        foreach (Vector3 v in Anchors)
        {
            averageAnchorPosition += v;

        }

        averageAnchorPosition /= Anchors.Count;

        PunchingBagTransform.forward = (new Vector3(PunchingBagTransform.position.x, 0, PunchingBagTransform.position.z) 
            - new Vector3(averageAnchorPosition.x, 0, averageAnchorPosition.z)).normalized;

        GameManager.Instance.calibratedObject.SetCalibration(PunchingBagTransform);

    }

    public void Continue()
    {
        GameManager.Instance.NextState();

    }

    Vector2 GetAnchorSetCircumcenter(AnchorSet anchorSet)
    {
        FindCircumcenter findC = GetComponent<FindCircumcenter>();

        Vector2 first = new Vector2(anchorSet.first.x, anchorSet.first.z);

        Vector2 second = new Vector2(anchorSet.second.x, anchorSet.second.z);

        Vector2 third = new Vector2(anchorSet.third.x, anchorSet.third.z);

        return findC.FindCircumCenter(first, second, third);
    }

    void RefineAnchorSets()
    {
        Vector2 averageCircumcenter = Vector2.zero;

        foreach(AnchorSet a in AnchorSets)
        {
            averageCircumcenter += a.circumcenter;

        }

        averageCircumcenter /= AnchorSets.Count;

        AnchorSet outlierAnchorSet = AnchorSets[0];

        for (int i = 1; i < AnchorSets.Count; i++)
        {
            float sqrMag = (AnchorSets[i].circumcenter - averageCircumcenter).sqrMagnitude;

            float sqrMagOutlier = (outlierAnchorSet.circumcenter - averageCircumcenter).sqrMagnitude;

            if (sqrMag > sqrMagOutlier)
            {
                outlierAnchorSet = AnchorSets[i];

            }

        }

        AnchorSets.Remove(outlierAnchorSet);

        if (AnchorSets.Count <= AnchorSetNumber / 2)
            return;

        RefineAnchorSets();

    }

    void StartSurfaceDraw()
    {
        //if (CheckHandAngle(ArmPositioning.LeftHandInstance.transform.forward, HandTargetTransform.forward))
        if (CheckHandAngle(HandL.forward, HandLTarget.forward))
        {
            if (TimerCurrent >= TimerMax)
            {
                DrawingSurface = true;
                TimerCurrent = 0;
            }

            TimerCurrent += Time.deltaTime;
        }
        else
        {
            TimerCurrent = 0;
        }

        int currentFrame = (int)(circleSprites.Length * Mathf.InverseLerp(0, TimerMax, TimerCurrent));

        if (currentFrame > circleSprites.Length-1)
            currentFrame = circleSprites.Length-1;

        TimerCircle.sprite = circleSprites[currentFrame];

        Debug.Log("Within Angle " + CheckHandAngle(HandL.forward, HandLTarget.forward));
    }

    void UpdateHandTarget()
    {
        if (ArmPositioning.LeftHandInstance == null)
        {
            return;
        }

        Transform leftHandInstance = ArmPositioning.LeftHandInstance.transform;

        HandLTarget.position = leftHandInstance.position;

        HandLTarget.eulerAngles = new Vector3(0, leftHandInstance.eulerAngles.y, 0);
    }
    /*
    void UpdateTestObjects()
    {
        //testOne.eulerAngles += new Vector3(1, 0, 0);

        testTwo.eulerAngles = new Vector3(0, testOne.eulerAngles.y, 0);

        //testThree.eulerAngles = new Vector3(testOne.eulerAngles.x, testOne.eulerAngles.y, 0);

        

    }
    */
    bool CheckHandAngle(Vector3 handForward, Vector3 targetForward)
    {
        float angleDiff = Vector3.Angle(handForward, targetForward);

        float maxangle = 90f;

        float alignment = Mathf.InverseLerp(maxangle, 0f, angleDiff);

        int currentFrame = (int)(circleSprites.Length * alignment);

        if (currentFrame > circleSprites.Length - 1)
        {
            currentFrame = circleSprites.Length - 1;
        }

        AlignmentCircle.sprite = circleSprites[currentFrame];

        Debug.Log(angleDiff + ", " + currentFrame + ", " + alignment);

        if (angleDiff > StartAngle)
        {
            return false;
        }

        return true;
    }
}
