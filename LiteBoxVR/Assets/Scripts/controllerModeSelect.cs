using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class controllerModeSelect : TagModularity
{
    public GuideWindow guide;

    private GameManager gameMgr;

    private void Start()
    {
        //gameMgr = FindTaggedObject("GameController").GetComponent<GameManager>();
        GameManager.Instance.controllerModeSelected = false;
    }


    void Update()
    {
        if (OVRInput.GetDown(OVRInput.RawButton.Y) || Input.GetKeyDown(KeyCode.Y))
        {
            GameManager.Instance.SetControllerMode(true);
            guide.SetInfoScreen(2);

            GameManager.Instance.NextState();


            FindTaggedObject("HandR").GetComponent<ArmPositioning>().ChangeControllerMode();
            FindTaggedObject("HandL").GetComponent<ArmPositioning>().ChangeControllerMode();
        }
        else if(OVRInput.GetDown(OVRInput.RawButton.X) || Input.GetKeyDown(KeyCode.X))
        {
            GameManager.Instance.SetControllerMode(false);
            guide.SetInfoScreen(1);

            GameManager.Instance.NextState();

            FindTaggedObject("HandR").GetComponent<ArmPositioning>().ChangeControllerMode();
            FindTaggedObject("HandL").GetComponent<ArmPositioning>().ChangeControllerMode();

        }
    }
}
