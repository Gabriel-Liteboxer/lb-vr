using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GuideWindow : MonoBehaviour
{
    [System.Serializable]
    public class InfoScreen
    {
        public string infoText;

        public Sprite infoSprite;

        public string infoHeader;
    }

    public InfoScreen[] InfoScreens;

    public Canvas GuideCanvas;

    public Image ImageRenderer;

    public Text TextField;


    public bool[] testScreen;

    public Transform playerHead;

    private void Start()
    {
        playerHead = Camera.main.gameObject.transform;


    }

    private void Update()
    {
        


    }

    public void SetInfoScreen(int screenIndex)
    {
        if (screenIndex > InfoScreens.Length)
            screenIndex = InfoScreens.Length - 1;
        else if (screenIndex < 0)
            screenIndex = 0;

        TextField.text = InfoScreens[screenIndex].infoText;

        if (InfoScreens[screenIndex] != null)
        {
            ImageRenderer.gameObject.SetActive(true);
            ImageRenderer.sprite = InfoScreens[screenIndex].infoSprite;

        }
        else
        {
            ImageRenderer.gameObject.SetActive(false);
        }

    }
}
