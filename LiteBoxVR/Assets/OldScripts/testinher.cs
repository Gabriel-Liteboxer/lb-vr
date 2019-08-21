using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class testinher : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Movement();
    }

    public virtual void Movement()
    {
        Debug.Log("parent called");

    }
}

public class testchild : testinher
{
    //void 

}
