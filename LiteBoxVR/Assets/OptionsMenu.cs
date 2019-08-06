using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OptionsMenu : MonoBehaviour
{
    Transform playerhead;

    public bool FollowCamera;

    // Start is called before the first frame update
    void Awake()
    {
        //playerhead = Camera.main.gameObject.transform;

        gameObject.SetActive(false);

        DontDestroyOnLoad(gameObject);
    }

    private void OnEnable()
    {
        if (playerhead == null)
            playerhead = Camera.main.gameObject.transform;
        if (playerhead == null)
            return;

        transform.rotation = playerhead.rotation;

        transform.eulerAngles = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y, 0);

        transform.position = playerhead.transform.position;

        transform.position += transform.forward * 0.5f;

        transform.localScale = Vector3.zero;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if (transform.localScale.y < 1)
        {
            transform.localScale = Vector3.Lerp(transform.localScale, Vector3.one, Time.deltaTime * 20);

        }

        if (!FollowCamera)
            return;

        //transform.rotation = playerhead.transform.rotation;

        transform.rotation = Quaternion.Lerp(transform.rotation, playerhead.transform.rotation, Time.deltaTime*20);

        transform.eulerAngles = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y, 0);

        Vector3 targetPosition = playerhead.transform.position;
        
        targetPosition += transform.forward*0.5f;

        transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime);

        

    }
}
