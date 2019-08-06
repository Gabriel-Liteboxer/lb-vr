using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArmPositioning : TagModularity
{
    public Transform ArmParent;

    private Vector3 armAngleOffset;

    private Vector3 armScaleDefault;

    //public TextMesh ArmOffsetText;

    //static float armOffset = -0.04f; // old value -0.1714f

    public float armDistance = 0.2f;

    //static float startingDistance;

    static float armScale = 1;

    static GameManager gameCont;

    public Transform ParentController;

    public OVRControllerHelper ControllerModel;

    public GameObject MountModel;

    public OVRInput.Controller defaultController;

    public bool TestingInEditor;

    // new idea, press fists together to calibrate arm distance. use dot product to determine if the controllers are pointing at each other

    private void Start()
    {
        ArmParent = transform.parent;

        transform.parent = null;

        armAngleOffset = transform.eulerAngles - ArmParent.eulerAngles;

        armScaleDefault = transform.localScale;

        gameCont = FindTaggedObject("GameController").GetComponent<GameManager>();

        //defaultController = ControllerModel.m_controller;

        StartCoroutine(LateStart());
    }

    // changing the control mode the first time caused the game to freeze as the controller model was loaded. 
    //LateStart ensures that the model is loaded before it is disabled
    IEnumerator LateStart()
    {
        yield return new WaitForEndOfFrame();

        ChangeControllerMode();
    }

    private void LateUpdate()
    {
        if (!TestingInEditor)
        {
            if (gameCont.UsingWristStraps)
                UpdateArmTransform();
            else
                UpdateHandHeldTransform();
        }

        armScale += OVRInput.Get(OVRInput.RawAxis2D.LThumbstick).y/10*Time.deltaTime;

        if (armScale > 1f)
        {
            armScale = 1f;

        }
        else if (armScale < 0.7f)
        {
            armScale = 0.7f;

        }

        transform.localScale = armScaleDefault * armScale;
    }

    private void UpdateArmTransform()
    {
        transform.position = ArmParent.position;

        transform.rotation = ArmParent.rotation;

        transform.eulerAngles += armAngleOffset;

        transform.position += transform.forward * -armDistance;

        //transform.localScale = armScaleDefault * armScale;

    }

    private void UpdateHandHeldTransform()
    {
        transform.position = ParentController.position;

        transform.rotation = ParentController.rotation * Quaternion.Euler(new Vector3(180, 0, 0));

        transform.position -= transform.forward * 0.05f;

        //transform.localScale = armScaleDefault * armScale;

        //transform.localEulerAngles += new Vector3(180, 0, 0); 
    }

    public void ChangeControllerMode()
    {
        if (gameCont.UsingWristStraps)
        {
            //ControllerModel.m_controller = OVRInput.Controller.All;
            ControllerModel.gameObject.SetActive(true);
            MountModel.SetActive(true);
        }
        else
        {
            //ControllerModel.m_controller = OVRInput.Controller.None;
            ControllerModel.gameObject.SetActive(false);
            MountModel.SetActive(false);
        }
    }

}
