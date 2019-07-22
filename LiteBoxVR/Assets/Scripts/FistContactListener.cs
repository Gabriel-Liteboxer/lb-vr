using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FistContactListener : MonoBehaviour
{
    public HexGameplay ListenerObj;

    public int padIndex;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "HandR" || other.gameObject.tag == "HandL")
            ListenerObj.PadContact(padIndex);


    }
}
