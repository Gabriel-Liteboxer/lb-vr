using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PunchingBagGameplay : MonoBehaviour
{
    private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.tag != "GloveL" && collision.gameObject.tag != "GloveR")
        {
            return;

        }


    }
}
