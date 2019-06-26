using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArmCalibration : MonoBehaviour
{
    public GameObject Arm;

    public TextMesh ArmOffsetText;

    static float armOffset = -0.1714f;

    static float armDistance = 1;

    static float armScale = 1;

    bool armCalibrationActive;

    //OVRInput.GetLocalControllerAcceleration <-- this could be interesting

    private void Update()
    {

        

        if (OVRInput.GetDown(OVRInput.RawButton.Y))
        {
            if (armCalibrationActive)
            {
                armCalibrationActive = false;

            }
            else
            {
                armCalibrationActive = true;
                armDistance = 10f;

            }

        }

        if (armCalibrationActive)
        {
            armOffset += OVRInput.Get(OVRInput.RawAxis2D.LThumbstick).y * Time.deltaTime / 10;

            armScale += OVRInput.Get(OVRInput.RawAxis2D.RThumbstick).y * Time.deltaTime / 10;


            float dist = Vector3.Distance(transform.position, new Vector3(transform.position.x, 0, transform.position.z));
            if (dist < armDistance)
            {
                armDistance = dist;

            }

            //armDistance = dist;

            ArmOffsetText.text = "dist: " + dist + "\n arm offset: " + armOffset;

            Arm.transform.localPosition = new Vector3(Arm.transform.localPosition.x, armDistance*armScale+armOffset*armScale, Arm.transform.localPosition.z);

            Arm.transform.localScale = new Vector3(armScale, armScale, armScale);
        }
        
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
