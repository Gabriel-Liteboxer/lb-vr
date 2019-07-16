using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TouchButton : TagModularity
{
    public SpriteRenderer loadingCircleRenderer;

    private Sprite[] circleSprites;

    [System.Serializable]
    public class ButtonPart
    {
        public Transform buttonTransfrom;

        public float hoverScale = 1;

        public float hoverPosition;

    }

    public ButtonPart[] buttonParts;
    
    [Header("State of Button")]

    public float LerpSpeed = 1;

    public bool isHovering;

    public float HoverDistance;

    private float CallFunctionTimer;

    public float CallFunctionTimerSpeed = 1;

    public string FunctionToCall;

    private void Start()
    {
        circleSprites = Resources.LoadAll<Sprite>("CircleLoading");
    }

    void Update()
    {
        if ((FindTaggedObject("HandR").transform.position - transform.position).sqrMagnitude < HoverDistance * HoverDistance
            || (FindTaggedObject("HandL").transform.position - transform.position).sqrMagnitude < HoverDistance * HoverDistance)
            isHovering = true;
        else
            isHovering = false;

        foreach (ButtonPart b in buttonParts)
        {
            float targetPos = 0;

            float targetScale = 1;

            if (isHovering)
            {
                targetPos = b.hoverPosition;
                targetScale = b.hoverScale;
            }

            b.buttonTransfrom.localPosition = new Vector3(0, 0, Mathf.Lerp(b.buttonTransfrom.position.z, targetPos, Time.deltaTime * LerpSpeed));

            b.buttonTransfrom.localScale = Vector3.one * Mathf.Lerp(b.buttonTransfrom.localScale.z, targetScale, Time.deltaTime * LerpSpeed);

        }

        if(isHovering)
        {
            CallFunctionTimer += Time.deltaTime * CallFunctionTimerSpeed;

            if(CallFunctionTimer >= 1)
            {
                Debug.Log("called function " + FunctionToCall);
                //call function

            }
        }
        else
        {
            CallFunctionTimer = 0;

        }

        RenderLoadingCircle(CallFunctionTimer, loadingCircleRenderer);
        
    }

    void RenderLoadingCircle(float value, SpriteRenderer SprRend)
    {
        if (value > 1)
            value = 1;
        else if (value < 0)
            value = 0;

        int CircleToRender = (int)Mathf.Lerp(0, circleSprites.Length, value);



        if (CircleToRender < circleSprites.Length)
            SprRend.sprite = circleSprites[CircleToRender];

        if (CircleToRender == 0)
        {
            SprRend.sprite = null;

        }

    }
}
