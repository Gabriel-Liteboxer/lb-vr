using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class noteMovementTest : MonoBehaviour
{
    public Vector2 position2D;

    public float speed = 1;

    public float radius = 0.5f;

    float unitsToRadians;

    float pi;

    public Transform bagOrigin;

    public float velocity;

    private void Start()
    {

        pi = Mathf.PI;



    }

    void Update()
    {
        

        float circumfernce = 2 * pi * radius;

        unitsToRadians = (2 * pi) / circumfernce;

        Debug.Log(unitsToRadians);

        Vector2 movement = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical")) * speed * Time.deltaTime;

        position2D += movement;

        float angleRad = position2D.x * unitsToRadians;

        Vector3 oldPosition = transform.position;

        transform.position = new Vector3(bagOrigin.position.x + radius * Mathf.Cos(angleRad), bagOrigin.position.y + position2D.y, bagOrigin.position.z + radius * Mathf.Sin(angleRad));
        
        velocity = Vector3.Distance(oldPosition, transform.position) * Time.deltaTime*1000000;

    }
}
