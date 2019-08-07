using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardSelection : MonoBehaviour
{
    public TMPro.TextMeshPro BoardText;

    private void Start()
    {
        GameManager.Instance.isBoardTypeSelected = false;

        Transform camTransform = Camera.main.transform;

        transform.position = camTransform.position;

        transform.forward = camTransform.forward;

        transform.eulerAngles = new Vector3(0, transform.eulerAngles.y, 0);

        transform.position += transform.forward/2;

        BoardText.text = "";
    }

    public void ChangeBoardType (int aBoardType)
    {
        BoardText.text = "Type " + aBoardType + " selected";

        GameManager.Instance.boardType = (GameManager.BoardType)aBoardType;

        GameManager.Instance.isBoardTypeSelected = true;
    }
}
