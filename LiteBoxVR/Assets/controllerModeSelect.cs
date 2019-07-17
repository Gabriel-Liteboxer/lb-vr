using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class controllerModeSelect : TagModularity
{
    public GuideWindow guide;

    private GameManager gameMgr;

    private void Start()
    {
        gameMgr = FindTaggedObject("GameController").GetComponent<GameManager>();

    }


    void Update()
    {
        if (OVRInput.GetDown(OVRInput.RawButton.Y))
        {
            gameMgr.SetControllerMode(true);
            guide.SetInfoScreen(2);
        }
        else if(OVRInput.GetDown(OVRInput.RawButton.X))
        {
            gameMgr.SetControllerMode(false);
            guide.SetInfoScreen(1);
        }
    }
}
