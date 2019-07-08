using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FacePlayerPromptUI : MonoBehaviour
{
    static Transform PlayerHead;

    public Vector3 PositionOffset;

    public float LerpSpeed;

    // Update is called once per frame
    void Update()
    {
        if(PlayerHead == null)
        {
            PlayerHead = GameObject.FindGameObjectWithTag("PlayerHead").transform;

        }

        transform.forward = Vector3.forward;

        transform.position = Vector3.Lerp(transform.position, PlayerHead.transform.position + PositionOffset, LerpSpeed*Time.deltaTime);

        transform.forward = PlayerHead.forward;

        

    }
}
