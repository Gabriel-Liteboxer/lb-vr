using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BagDragCalibration : MonoBehaviour
{
    public LineRenderer lineRenderer;

    public float TimerCurrent;

    public float TimerMax;

    private Transform HandTargetTransform;

    public Transform testOne;

    public Transform testTwo;

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

    [System.Serializable]
    public class AnchorSet
    {
        public Vector3 first;

        public Vector3 second;

        public Vector3 third;

        public float bagRadius;

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

        HandTargetTransform = new GameObject().transform;
    }

    void Update()
    {
        UpdateTestObjects();

        UpdateHandTranform();

        if(!DrawingSurface)
        {
            StartSurfaceDraw();

        }
        else
        {
            DrawSurface(testOne.position);

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
                AnchorSets.Add(new AnchorSet(firstAnchors[i], secondAnchors[i], thirdAnchors[i]));

            }

            SurfaceDrawn = true;

            Debug.Log("anchors count \n" + "first: " + firstAnchors.Count + "\n" + "second: " + secondAnchors.Count + "\n" + "third: " + thirdAnchors.Count);

        }

    }

    void StartSurfaceDraw()
    {
        //if (CheckHandAngle(ArmPositioning.LeftHandInstance.transform.forward, HandTargetTransform.forward))
        if (CheckHandAngle(testOne.forward, testTwo.forward))
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

        Debug.Log("Within Angle " + CheckHandAngle(testOne.forward, testTwo.forward));
    }

    void UpdateHandTranform()
    {
        if (ArmPositioning.LeftHandInstance == null)
        {
            return;
        }

        Transform leftHandInstance = ArmPositioning.LeftHandInstance.transform;

        HandTargetTransform.position = leftHandInstance.position;

        HandTargetTransform.eulerAngles = new Vector3(0, leftHandInstance.eulerAngles.y, 0);
    }

    void UpdateTestObjects()
    {
        //testOne.eulerAngles += new Vector3(1, 0, 0);

        testTwo.eulerAngles = new Vector3(0, testOne.eulerAngles.y, 0);

        //testThree.eulerAngles = new Vector3(testOne.eulerAngles.x, testOne.eulerAngles.y, 0);

        

    }

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
