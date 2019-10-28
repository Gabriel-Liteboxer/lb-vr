using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class testlerp : MonoBehaviour
{
    public float x;
    public float y;
    public float z;

    public float zPos;

    private void Update()
    {
        zPos = Mathf.InverseLerp(x, y, z);
    }
}
