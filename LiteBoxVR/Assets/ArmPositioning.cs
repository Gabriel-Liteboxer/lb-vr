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

    public GameObject ControllerModel;

    public GameObject MountModel;

    // new idea, press fists together to calibrate arm distance. use dot product to determine if the controllers are pointing at each other

    private void Start()
    {
        ArmParent = transform.parent;

        transform.parent = null;

        armAngleOffset = transform.eulerAngles - ArmParent.eulerAngles;

        armScaleDefault = transform.localScale;

        gameCont = FindTaggedObject("GameController").GetComponent<GameManager>();

        ChangeControllerMode();
    }

    private void LateUpdate()
    {
        if (gameCont.UsingWristStraps)
            UpdateArmTransform();
        else
            UpdateHandHeldTransform();
    }

    private void UpdateArmTransform()
    {
        transform.position = ArmParent.position;

        transform.rotation = ArmParent.rotation;

        transform.eulerAngles += armAngleOffset;

        transform.position += transform.forward * -armDistance;

        transform.localScale = armScaleDefault * armScale * 0.8f;

    }

    private void UpdateHandHeldTransform()
    {
        transform.position = ParentController.position;

        transform.rotation = ParentController.rotation * Quaternion.Euler(new Vector3(180, 0, 0));

        //transform.localEulerAngles += new Vector3(180, 0, 0); 
    }

    public void ChangeControllerMode()
    {
        if (gameCont.UsingWristStraps)
        {
            ControllerModel.SetActive(true);
            MountModel.SetActive(true);
        }
        else
        {
            ControllerModel.SetActive(false);
            MountModel.SetActive(false);
        }
    }

}
