using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArmCalibration : TagModularity
{
    public GameObject Arm;

    private GameObject ArmParent;

    private Vector3 armAngleOffset;

    private Vector3 armScaleDefault;

    public TextMesh ArmOffsetText;

    static float armOffset = -0.04f; // old value -0.1714f

    static float armDistance = 0.2f;

    static float startingDistance;

    static float armScale = 1;

    static GameManager gameCont;

    bool armCalibrationActive;

    //OVRInput.GetLocalControllerAcceleration <-- this could be interesting

    // new idea, press fists together to calibrate arm distance. use dot product to determine if the controllers are pointing at each other

    private void Start()
    {
        ArmParent = Arm.transform.parent.gameObject;

        Arm.transform.parent = null;

        armAngleOffset = Arm.transform.eulerAngles - ArmParent.transform.eulerAngles;

        armScaleDefault = Arm.transform.localScale;

        gameCont = FindTaggedObject("GameController").GetComponent<GameManager>();

        if (!gameCont.UsingWristStraps)
        {
            gameCont.isArmCalibrated = true;
            gameCont.NextState();
        }
    }

    private void Update()
    {
        if (gameCont.StateOfGame != GameManager.GameState.armCalibration)
            return;

        if (OVRInput.GetDown(OVRInput.RawButton.Y))
        {
            /*if (armCalibrationActive)
            {
                armCalibrationActive = false;
                //Arm.transform.parent = null;
            }
            else
            {
                armCalibrationActive = true;
                armDistance = 1f;
                startingDistance = ArmParent.transform.position.y;
            }*/

            gameCont.isArmCalibrated = false;
            armCalibrationActive = true;
            armDistance = 0.2f;
            startingDistance = ArmParent.transform.position.y;

        }

        if (armCalibrationActive)
        {
            armOffset += OVRInput.Get(OVRInput.RawAxis2D.LThumbstick).y * Time.deltaTime / 10;

            armScale += OVRInput.Get(OVRInput.RawAxis2D.RThumbstick).y * Time.deltaTime / 10;

            float dist = ArmParent.transform.position.y;

            if (dist < armDistance)
            {
                armDistance = dist;

            }
            /*
            if(dist > startingDistance / 2 && armDistance < startingDistance / 3)
            {
                armCalibrationActive = false;
                
            }*/

            if (dist > armDistance + startingDistance/3)
            {
                //Application.Quit();
                armCalibrationActive = false;
                gameCont.isArmCalibrated = true;
            }

            ArmOffsetText.text = "dist: " + dist;
        }
        
    }

    private void LateUpdate()
    {
        UpdateArmTransform();
    }

    private void UpdateArmTransform()
    {
        Arm.transform.position = ArmParent.transform.position;

        Arm.transform.rotation = ArmParent.transform.rotation;

        Arm.transform.eulerAngles += armAngleOffset;

        Arm.transform.position += Arm.transform.forward * -armDistance;

        Arm.transform.localScale = armScaleDefault * armScale*0.8f;

    }

    void GetOffsetValues()
    {
        if (OVRInput.GetDown(OVRInput.RawButton.Y))
        {


            if (Arm.transform.parent == gameObject.transform)
            {
                Arm.transform.parent = null;

            }
            else
            {
                Arm.transform.parent = gameObject.transform;

            }

        }
        ArmOffsetText.text =
            "Position: \n" +
            "X" + Arm.transform.localPosition.x.ToString() + "\n" +
            "Y" + Arm.transform.localPosition.y.ToString() + "\n" +
            "Z" + Arm.transform.localPosition.z.ToString() + "\n" +

            "Rotation: \n" +
            "X" + Arm.transform.localRotation.eulerAngles.x.ToString() + "\n" +
            "Y" + Arm.transform.localRotation.eulerAngles.y.ToString() + "\n" +
            "Z" + Arm.transform.localRotation.eulerAngles.z.ToString();
    }


}
