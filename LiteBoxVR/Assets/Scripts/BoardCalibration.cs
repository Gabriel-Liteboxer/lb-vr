using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardCalibration : TagModularity
{
    public List<GameObject> AnchorPoints;

    public GameObject AnchorPointPrefab;

    public Renderer BoardRender;

    public Transform FistEndpoint;

    GameManager gameCont;

    public GuideWindow guide;

    public bool calibrationActive = true;

    // note: reasearch cross product to determine direction based on normal direction

    private void Start()
    {
        gameCont = FindTaggedObject("GameController").GetComponent<GameManager>();

        FistEndpoint = FindTaggedObject("HandL").transform;

        //Board = GameObject.Instantiate(BoardPrefab);
        transform.position = Vector3.zero;

        //PlayerHead = GameObject.Find("CenterEyeAnchor").transform;

        guide.SetInfoScreen(0);
    }

    // Update is called once per frame
    void Update()
    {
        if (OVRInput.GetDown(OVRInput.RawButton.X) && calibrationActive)
        {
            if (AnchorPoints.Count >= 3)
            {
                // disabling this for now

                /*
                foreach (GameObject an in AnchorPoints)
                {
                    Destroy(an);

                }
                AnchorPoints.Clear();

                transform.position = Vector3.zero;*/
            }
            else
            {
                GameObject NewAnchorpoint = GameObject.Instantiate(AnchorPointPrefab, FistEndpoint.position, FistEndpoint.rotation);

                AnchorPoints.Add(NewAnchorpoint);

            }

            guide.SetInfoScreen(AnchorPoints.Count);

        }

        if (OVRInput.GetDown(OVRInput.RawButton.Y))
        {
            calibrationActive = true;
            foreach (GameObject an in AnchorPoints)
            {
                Destroy(an);

            }
            AnchorPoints.Clear();

            guide.SetInfoScreen(0);
        }

        if (AnchorPoints.Count >= 3)
        {
            calibrationActive = false;

            transform.position = (AnchorPoints[0].transform.position + AnchorPoints[1].transform.position) / 2;

            transform.forward = -GetNormal(AnchorPoints[0].transform.position, AnchorPoints[1].transform.position, AnchorPoints[2].transform.position);
            /*
            if (GetFacing(FindTaggedObject("HandR").transform.position) > 0)
            {
                transform.forward = -transform.forward;

            }*/

            if (GetFacing(Camera.main.gameObject.transform.position) > 0)
            {
                transform.forward = -transform.forward;

            }

            gameCont.SetBoardPosition(transform.position, transform.forward);

            foreach (GameObject an in AnchorPoints)
            {
                Destroy(an);

            }
            AnchorPoints.Clear();

        }

        if (OVRInput.GetDown(OVRInput.RawButton.RIndexTrigger))
        {
            transform.forward = -transform.forward;

        }


    }

    Vector3 GetNormal(Vector3 a, Vector3 b, Vector3 c)
    {
        Vector3 side1 = b - a;
        Vector3 side2 = c - a;

        return Vector3.Cross(side1, side2).normalized;
    }

    float GetFacing(Vector3 targetpos)
    {

        Vector3 toOther = targetpos - transform.position;

        return Vector3.Dot(transform.forward, toOther);
    }



}
