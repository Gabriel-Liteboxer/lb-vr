using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class testchildinher : testinher
{
    public override void Movement()
    {
        Debug.Log("yes");
        base.Movement();
    }
}
