using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BagDragCalibration : MonoBehaviour
{
    public LineRenderer lineRenderer;

    private float TimerCurrent;

    public float TimerMax;

    private Transform HandTargetTransform;

    public Transform testOne;

    public Transform testTwo;

    public Transform testThree;

    public float StartAngle;

    private Sprite[] circleSprites;

    public SpriteRenderer TimerCircle;

    public SpriteRenderer AlignmentCircle;

    public bool DrawingSurface;

    private void Start()
    {
        circleSprites = Resources.LoadAll<Sprite>("CircleLoading");

        HandTargetTransform = new GameObject().transform;
    }

    void Update()
    {
        //CheckHandOrientation();

        UpdateHandTranform();

        if (CheckHandAngle(ArmPositioning.LeftHandInstance.transform.forward, HandTargetTransform.forward))
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

    void CheckHandOrientation()
    {
        //testOne.eulerAngles += new Vector3(1, 0, 0);

        testTwo.eulerAngles = new Vector3(0, testOne.eulerAngles.y, 0);

        testThree.eulerAngles = new Vector3(testOne.eulerAngles.x, testOne.eulerAngles.y, 0);

        

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
