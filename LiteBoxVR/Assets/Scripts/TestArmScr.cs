using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestArmScr : MonoBehaviour
{
    Animator anim;

    public float recoilStrengh;

    public bool punch;

    public bool counter;

    bool isIdle;

    public int ArmIndex;

    public GameObject ArmEndpoint;

    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
        ArmEndpoint.GetComponent<RobotArmEndpoint>().ObjectToNotify = gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        anim.SetFloat("RecoilStrengh", recoilStrengh);

        AnimatorStateInfo animState = anim.GetCurrentAnimatorStateInfo(0);

        if (animState.IsName("Idle"))
        {
            isIdle = true;
            counter = false;
        }
        else
        {
            isIdle = false;
            punch = false;
        }

        if (punch && isIdle)
        {
            anim.Play("Punch");
            
            punch = false;
        }

        if(animState.IsName("Punch"))
        {
            ArmEndpoint.GetComponent<SphereCollider>().enabled = true;
            if (animState.normalizedTime > 0.98f)
            {
                counter = true;
            }
        }
        else
        {
            ArmEndpoint.GetComponent<SphereCollider>().enabled = false;

        }

        
        anim.SetBool("StartRecoil", counter);

    }

    public void PunchCountered (bool LeftGlove)
    {
        counter = true;

    }

}
