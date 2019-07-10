using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArmCalibration : MonoBehaviour
{
    public GameObject Arm;

    private GameObject ArmParent;

    private Vector3 armAngleOffset;

    private Vector3 armScaleDefault;

    public TextMesh ArmOffsetText;

    static float armOffset = -0.04f; // old value -0.1714f

    static float armDistance = 1;

    static float armScale = 1;

    bool armCalibrationActive;

    //OVRInput.GetLocalControllerAcceleration <-- this could be interesting

    private void Start()
    {
        ArmParent = Arm.transform.parent.gameObject;

        Arm.transform.parent = null;

        armAngleOffset = Arm.transform.eulerAngles - ArmParent.transform.eulerAngles;

        armScaleDefault = Arm.transform.localScale;
    }

    private void Update()
    {
        //UpdateArmTransform();

        if (OVRInput.GetDown(OVRInput.RawButton.Y))
        {
            if (armCalibrationActive)
            {
                armCalibrationActive = false;
                //Arm.transform.parent = null;
            }
            else
            {
                armCalibrationActive = true;
                armDistance = 10f;
                //Arm.transform.parent = gameObject;
            }

        }

        if (armCalibrationActive)
        {
            armOffset += OVRInput.Get(OVRInput.RawAxis2D.LThumbstick).y * Time.deltaTime / 10;

            armScale += OVRInput.Get(OVRInput.RawAxis2D.RThumbstick).y * Time.deltaTime / 10;


            //float dist = Vector3.Distance(transform.position, new Vector3(transform.position.x, 0, transform.position.z));

            //float dist = transform.position.y; worked pretty ok, just with some offset

            //Vector3 OldLocalPos = Arm.transform.localPosition;

            //Arm.transform.position = new Vector3(Arm.transform.position.x, 0, Arm.transform.position.z);

            //float dist = Arm.transform.localPosition.z;

            float dist = ArmParent.transform.position.y;

            if (dist < armDistance)
            {
                armDistance = dist;

            }

            //armDistance = dist;

            ArmOffsetText.text = "dist: " + dist + "\n arm offset: " + armOffset;



            //Arm.transform.localPosition = new Vector3(Arm.transform.localPosition.x, armDistance*armScale+armOffset*armScale, Arm.transform.localPosition.z);

            //Arm.transform.localPosition = new Vector3(Arm.transform.localPosition.x, Arm.transform.localPosition.y, -armDistance + armOffset);

            //Arm.transform.localPosition = new Vector3(OldLocalPos.x, OldLocalPos.y, armDistance);

            //Arm.transform.localScale = new Vector3(armScale, armScale, armScale);
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
