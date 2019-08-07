using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PunchingBagCalibration : TagModularity
{
    public List<GameObject> AnchorPoints;

    public GameObject AnchorPointPrefab;

    public Transform FistEndpoint;

    public GuideWindow guide;

    public bool calibrationActive = true;

    Vector3 Centerpoint;

    public Transform PunchingBagObj;

    private void Start()
    {
        FistEndpoint = FindTaggedObject("HandL").transform;

        //guide.SetInfoScreen(0);
    }

    // Update is called once per frame
    void Update()
    {
        if (OVRInput.GetDown(OVRInput.RawButton.X) && calibrationActive)
        {
            if (AnchorPoints.Count >= 3)
            {
                
            }
            else
            {
                GameObject NewAnchorpoint = GameObject.Instantiate(AnchorPointPrefab, FistEndpoint.position, FistEndpoint.rotation);

                AnchorPoints.Add(NewAnchorpoint);

            }

            //guide.SetInfoScreen(AnchorPoints.Count);

        }

        if (OVRInput.GetDown(OVRInput.RawButton.Y))
        {
            calibrationActive = true;
            foreach (GameObject an in AnchorPoints)
            {
                Destroy(an);

            }
            AnchorPoints.Clear();

            //guide.SetInfoScreen(0);
        }

        if (AnchorPoints.Count >= 3)
        {
            calibrationActive = false;

            FindCircumcenter findC = GetComponent<FindCircumcenter>();

            List<Vector2> anchorPoints2D = new List<Vector2>();

            foreach (GameObject go in AnchorPoints)
            {
                anchorPoints2D.Add(new Vector2(go.transform.position.x, go.transform.position.z));

            }

            Vector2 center = findC.FindCircumCenter(anchorPoints2D[0], anchorPoints2D[1], anchorPoints2D[2]);

            Centerpoint = new Vector3(center.x, 1, center.y);

            PunchingBagObj.transform.position = Centerpoint;

            float distFromCenter = Vector2.Distance(anchorPoints2D[0], center);

            PunchingBagObj.transform.localScale = Vector3.one * distFromCenter*2;

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
}
