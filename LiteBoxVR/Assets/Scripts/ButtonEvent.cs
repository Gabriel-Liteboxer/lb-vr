using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonEvent : MonoBehaviour
{
    public GameObject TargetObject;

    public string MethodName;

    [Tooltip("keeping ParameterToUse blank will pass no parameters")]

    public string ParameterToUse = "";

    [Tooltip("Only one of the following can be used")]

    public int IntParameter;

    public string StringParameter;

    public bool BoolParameter;

    public float HoverScale = 1;

    Vector3 StartingScale;

    public float ScalingSpeed = 10;

    public float TargetHoverScale = 1;

    private void Start()
    {
        StartingScale = transform.localScale;
         
    }

    public void ButtonAction ()
    {
        if(ParameterToUse == "")
            TargetObject.SendMessage(MethodName);
        else if(ParameterToUse == "int")
            TargetObject.SendMessage(MethodName, IntParameter);
        else if (ParameterToUse == "string")
            TargetObject.SendMessage(MethodName, StringParameter);
        else if (ParameterToUse == "bool")
            TargetObject.SendMessage(MethodName, BoolParameter);
    }

    public void SetHoverState(bool isHovering)
    {
        if (isHovering)
            TargetHoverScale = 1.2f;
        
    }

    private void Update()
    {
        HoverScale = Mathf.Lerp(HoverScale, TargetHoverScale, Time.deltaTime*10f);

        transform.localScale = StartingScale * HoverScale;

        TargetHoverScale = 1;
    }
}
