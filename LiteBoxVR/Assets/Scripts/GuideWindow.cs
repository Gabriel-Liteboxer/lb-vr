using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GuideWindow : MonoBehaviour
{
    [System.Serializable]
    public class InfoScreen
    {
        public Sprite sprite;

        public string header;

        [TextArea]
        public string body;
    }

    public InfoScreen[] InfoScreens;

    public Canvas GuideCanvas;

    public Image ImageRenderer;

    public TextMeshProUGUI TextHeader;

    public TextMeshProUGUI TextBody;

    public bool[] testScreen;

    public Transform playerHead;

    public float RadiusFromPlayer;

    public float LerpSpeed;

    public float RotationDeadzone;

    public float PositionDeadzone;

    private Transform targetTransform;



    private void Start()
    {
        playerHead = Camera.main.gameObject.transform;

        testScreen = new bool[InfoScreens.Length];

        targetTransform = new GameObject().transform;

        SetInfoScreen(0);
    }

    private void Update()
    {
        for (int i = 0; i < testScreen.Length; i++)
        {
            if(testScreen[i])
            {
                SetInfoScreen(i);

            }


        }


    }

    private void LateUpdate()
    {
        if (Vector3.SqrMagnitude(targetTransform.position - playerHead.position) > PositionDeadzone * PositionDeadzone)
            targetTransform.position = playerHead.position - new Vector3(0, playerHead.position.y * 0.2f, 0);


        if (Mathf.Abs(targetTransform.eulerAngles.y - playerHead.eulerAngles.y) > RotationDeadzone)
            targetTransform.eulerAngles = new Vector3(0, playerHead.eulerAngles.y, 0);
        

        transform.position = Vector3.Lerp(transform.position, targetTransform.position+transform.forward*RadiusFromPlayer, Time.deltaTime*LerpSpeed);
        
        transform.forward = Vector3.Lerp(transform.forward, targetTransform.forward, Time.deltaTime*LerpSpeed);
        
    }

    public void SetInfoScreen(int screenIndex)
    {
        if (screenIndex > InfoScreens.Length)
            screenIndex = InfoScreens.Length - 1;
        else if (screenIndex < 0)
            screenIndex = 0;

        TextHeader.text = InfoScreens[screenIndex].header;

        TextBody.text = InfoScreens[screenIndex].body;

        if (InfoScreens[screenIndex].sprite != null)
        {
            ImageRenderer.gameObject.SetActive(true);
            ImageRenderer.sprite = InfoScreens[screenIndex].sprite;
            Debug.Log("has image");
        }
        else
        {
            ImageRenderer.gameObject.SetActive(false);
            Debug.Log("no image given");
        }

    }
}
