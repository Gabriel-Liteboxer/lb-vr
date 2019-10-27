using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PadHitbox : MonoBehaviour
{
    public delegate float PadHit (int PadIndex);
    public PadHit padHit;

    int PadIndex;

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag == "GloveL" || collision.gameObject.tag == "GloveR")
        {
            padHit(PadIndex);


        }


    }
}
