using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArmCalibrator : TagModularity
{
    private Transform LeftHand;

    private Transform RightHand;

    public bool CalibrationActive;

    private Transform PlayerHead;

    private Sprite[] circleSprites;

    public SpriteRenderer FacingCircle;

    public SpriteRenderer CalibratingCircle;

    public float CalibrationTimer;

    public float CalibrationSpeed;

    private GameManager GameMgr;

    public GuideWindow guideWindow;

    private void Start()
    {
        GameMgr = FindTaggedObject("GameController").GetComponent<GameManager>();

        //SprRend = GetComponentInChildren<SpriteRenderer>();

        FacingCircle.material.renderQueue = 4000;

        //FacingCircle.material.shader.

        //CalibratingCircle.material.renderQueue = 50000;

        circleSprites = Resources.LoadAll<Sprite>("CircleLoading");

        if(Camera.main != null)
            PlayerHead = Camera.main.gameObject.transform;

        LeftHand = FindTaggedObject("HandL").transform;

        RightHand = FindTaggedObject("HandR").transform;

        if (!GameMgr.UsingWristStraps)
        {
            GameMgr.isArmCalibrated = true;
            GameMgr.NextState();
        }
    }

    void Update()
    {
        //RenderLoadingCircle(CircleLerp, FacingCircle);

        if (OVRInput.GetDown(OVRInput.RawButton.Y))
        {
            CalibrationActive = true;
            GameMgr.isArmCalibrated = false;
        }

        if (RightHand == null || LeftHand == null)
            CalibrationActive = false;

        if (CalibrationActive)
            ArmCalibration();

        if (PlayerHead != null)
            transform.forward = PlayerHead.forward;


    }

    void RenderLoadingCircle(float value, SpriteRenderer SprRend)
    {
        if (value > 1)
            value = 1;
        else if (value < 0)
            value = 0;

        int CircleToRender = (int)Mathf.Lerp(0, circleSprites.Length, value);

        

        if (CircleToRender < circleSprites.Length)
            SprRend.sprite = circleSprites[CircleToRender];

        if (CircleToRender == 0)
        {
            SprRend.sprite = null;

        }

    }

    void ArmCalibration()
    {
        transform.position = (RightHand.position + LeftHand.position) / 2;

        //Vector3 LookDir = RightHand.position - LeftHand.position;

        float FacingValue = -Vector3.Dot(RightHand.forward.normalized, LeftHand.forward.normalized);

        Debug.Log(FacingValue);

        //CircleLerp = -FacingValue;

        if (FacingValue > 0.9f)
        {
            CalibrationTimer += CalibrationSpeed * Time.deltaTime;

            guideWindow.SetInfoScreen(2);

            if (CalibrationTimer >= 1)
            {
                CalibrationActive = false;

                CalibrationTimer = 0;

                SetCalibration();

                FacingCircle.sprite = null;

                guideWindow.SetInfoScreen(3);
            }

            RenderLoadingCircle(CalibrationTimer, CalibratingCircle);
        }
        else
        {
            CalibrationTimer = 0;
            CalibratingCircle.sprite = null;

            guideWindow.SetInfoScreen(1);
        }

        RenderLoadingCircle(FacingValue, FacingCircle);

    }

    void SetCalibration()
    {
        ArmPositioning LarmPos = LeftHand.GetComponent<ArmPositioning>();

        ArmPositioning RarmPos = RightHand.GetComponent<ArmPositioning>();

        float dist = Vector3.Distance(RarmPos.ArmParent.position, LarmPos.ArmParent.position);

        RarmPos.armDistance = dist / 2;

        LarmPos.armDistance = dist / 2;

        GameMgr.isArmCalibrated = true;
    }
}
