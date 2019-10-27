using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardPlacement : TagModularity
{
    public bool LeftHandContact;

    public bool RightHandContact;

    public GameObject InnerRing;

    public GameObject InnerCube;

    public Vector3 StartingPositionOffset;

    public Vector3 StartingAngleOffset;

    Vector3 StartingEulerAngles;

    bool GrabbingRing_Left;

    bool GrabbingCube_Left;

    GameManager GameMgr;

    private void Start()
    {
        GameMgr = FindTaggedObject("GameController").GetComponent<GameManager>();
    }

    void Update()
    {
        Vector3 LeftControllerLocalPos = transform.InverseTransformPoint(FindTaggedObject("HandL").transform.position);

        Vector3 RightControllerLocalPos = transform.InverseTransformPoint(FindTaggedObject("HandR").transform.position);

        if (WithinCylinder(LeftControllerLocalPos, Vector3.zero, 0.05f, 0.25f))
        {
            InnerRing.SetActive(true);
            InnerCube.SetActive(false);

            if (OVRInput.GetDown(OVRInput.RawButton.LIndexTrigger))
            {
                StartingAngleOffset = (transform.position - FindTaggedObject("HandL").transform.position).normalized;

                StartingAngleOffset = new Vector3(StartingAngleOffset.x, 0, StartingAngleOffset.z);

                StartingAngleOffset.Normalize();

                StartingEulerAngles = transform.eulerAngles;

                GrabbingRing_Left = true;
            }
            
        }
        else if (WithinCube(LeftControllerLocalPos, Vector3.zero, Vector3.one/4))
        {
            InnerCube.SetActive(true);
            InnerRing.SetActive(false);

            if (OVRInput.GetDown(OVRInput.RawButton.LIndexTrigger))
            {
                StartingPositionOffset = FindTaggedObject("HandL").transform.position - transform.position;
                GrabbingCube_Left = true;
            }
            

        }
        else
        {
            InnerCube.SetActive(false);
            InnerRing.SetActive(false);
        }

        if (OVRInput.GetUp(OVRInput.RawButton.LIndexTrigger))
        {
            GrabbingRing_Left = false;
            GrabbingCube_Left = false;
        }

        if (GrabbingRing_Left)
        {
            Vector3 CurrentAngle = (transform.position - FindTaggedObject("HandL").transform.position);

            CurrentAngle = new Vector3(CurrentAngle.x, 0, CurrentAngle.z);

            CurrentAngle.Normalize();

            transform.eulerAngles = StartingEulerAngles + new Vector3(0, Vector3.SignedAngle(StartingAngleOffset, CurrentAngle, Vector3.up), 0);

            InnerCube.SetActive(false);
            InnerRing.SetActive(true);

        }

        if (GrabbingCube_Left)
        {
            transform.position = FindTaggedObject("HandL").transform.position - StartingPositionOffset;

            InnerCube.SetActive(true);
            InnerRing.SetActive(false);
        }

        //GameMgr.SetBoardPosition(transform.position, transform.forward);

        GameManager.Instance.calibratedObject.SetCalibration(transform);

    }

    bool WithinCube(Vector3 targetPosition, Vector3 cubePosition, Vector3 cubeSize)
    {
        if (targetPosition.x > cubePosition.x - cubeSize.x / 2 && targetPosition.x < cubePosition.x + cubeSize.x / 2
            && targetPosition.y > cubePosition.y - cubeSize.y / 2 && targetPosition.y < cubePosition.y + cubeSize.y / 2
            && targetPosition.z > cubePosition.z - cubeSize.z / 2 && targetPosition.z < cubePosition.z + cubeSize.z / 2)
        {
            return true;

        }
        return false;
    }

    bool WithinCylinder(Vector3 targetPosition, Vector3 cylinderPosition, float height, float radius)
    {
        Vector3 targetPositionLeveled = new Vector3(targetPosition.x, 0, targetPosition.z);

        Vector3 cylinderPositionLeveled = new Vector3(cylinderPosition.x, 0, cylinderPosition.z);

        if (Vector3.Distance(targetPositionLeveled, cylinderPositionLeveled) < radius && Mathf.Abs(targetPosition.y - cylinderPosition.y) < height)
        {

            return true;
        }

        return false;
    }
}
