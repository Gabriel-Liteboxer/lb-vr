using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PunchPadHitDetection : MonoBehaviour
{
    public NoteManager noteMgr;

    public int PadIndex;

    private void OnTriggerEnter(Collider collision)
    {
        if(collision.transform.gameObject.tag == "GloveR")
        {
            noteMgr.PadHit(PadIndex, false);

        }
        else if (collision.transform.gameObject.tag == "GloveL")
        {
            noteMgr.PadHit(PadIndex, true);

        }
    }

}
