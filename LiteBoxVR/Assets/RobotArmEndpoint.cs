using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RobotArmEndpoint : MonoBehaviour
{
    public GameObject ObjectToNotify;

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.gameObject.tag == "GloveL")
        {
            ObjectToNotify.SendMessage("PunchCountered", true);
            //GetComponent<SphereCollider>().enabled = false;
        }
        else if (other.transform.gameObject.tag == "GloveR")
        {
            ObjectToNotify.SendMessage("PunchCountered", false);
            //GetComponent<SphereCollider>().enabled = false;
        }

    }
}
