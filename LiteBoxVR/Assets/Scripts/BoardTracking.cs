using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardTracking : MonoBehaviour
{
    public List<GameObject> AnchorPoints;

    public GameObject AnchorPointPrefab;

    public Renderer BoardRender;

    public Transform FistEndpoint;

    public Transform PlayerHead;

    // note: reasearch cross product to determine direction based on normal direction

    private void Start()
    {

        //Board = GameObject.Instantiate(BoardPrefab);
        transform.position = Vector3.zero;

        //PlayerHead = GameObject.Find("CenterEyeAnchor").transform;
    }

    // Update is called once per frame
    void Update()
    {
        if (OVRInput.GetDown(OVRInput.RawButton.LIndexTrigger))
        {

            GameObject NewAnchorpoint = GameObject.Instantiate(AnchorPointPrefab, FistEndpoint.position, FistEndpoint.rotation);


            AnchorPoints.Add(NewAnchorpoint);


        }

        if (OVRInput.GetDown(OVRInput.RawButton.X))
        {
            foreach (GameObject an in AnchorPoints)
            {
                Destroy(an);

            }
            AnchorPoints.Clear();
        }

        if (AnchorPoints.Count > 2)
        {
            BoardRender.enabled = true;

            Vector3 AveragePosition = Vector3.zero;

            foreach (GameObject an in AnchorPoints)
            {
                AveragePosition += an.transform.position;

            }
            transform.position = (AnchorPoints[0].transform.position + AnchorPoints[1].transform.position) / 2;

            transform.forward = -GetNormal(AnchorPoints[0].transform.position, AnchorPoints[1].transform.position, AnchorPoints[2].transform.position);

            if(GetFacing(PlayerHead.position) > 0)
            {
                transform.forward = -transform.forward;

            }

        }
        else
        {
            BoardRender.enabled = false;
            transform.position = Vector3.zero;
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
