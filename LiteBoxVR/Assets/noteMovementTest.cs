using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class noteMovementTest : MonoBehaviour
{
    public Vector2 TestPosition2D;

    public float speed = 1;

    public float bagRadius = 0.5f;

    float bagCircumfernce;

    float unitsToRadians;

    float pi;

    public Transform bagOrigin;

    public float velocity;

    public GameObject PadPrefab;

    public class Pad
    {
        public Vector2 StartPosition;

        public Vector2 TargetPosition;

        public Vector2 EndPosition;

        public GameObject PadObject;

        public Pad(Vector2 startPosition, Vector2 targetPosition, Vector2 endPosition)
        {
            StartPosition = startPosition;

            TargetPosition = targetPosition;

            EndPosition = endPosition;

        }

        public void SetPadObject(GameObject padPrefab, Vector3 position, Vector3 eulerAngles)
        {
            PadObject = GameObject.Instantiate(padPrefab);

            PadObject.transform.position = position;

            PadObject.transform.eulerAngles = eulerAngles;

        }

    }

    public List<Pad> Pads;

    private void Start()
    {
        pi = Mathf.PI;
    }

    void Update()
    {
        BagSizeChanged();

        MoveBallOnbag();

    }

    void GeneratePads (float startRadius, float targetRadius, float endRadius)
    {
        for (int i = 0; i < 6; i++)
        {
            Pad newPad = new Pad(Vector2.one, Vector2.one, Vector2.one);

            newPad.SetPadObject(PadPrefab, GetPositionOnBag(newPad.TargetPosition), GetRotationOnbag(newPad.TargetPosition));

            Pads.Add(newPad);
        }

    }

    void MoveBallOnbag()
    {
        Vector2 movement = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical")) * speed * Time.deltaTime;

        TestPosition2D += movement;

        transform.position = GetPositionOnBag(TestPosition2D);

        transform.eulerAngles = GetRotationOnbag(TestPosition2D);

    }


    void BagSizeChanged()
    {
        bagCircumfernce = 2 * pi * bagRadius;

        unitsToRadians = (2 * pi) / bagCircumfernce;
    }

    public Vector3 GetPositionOnBag(Vector2 position2D)
    {
        float angle = position2D.x * unitsToRadians;

        return new Vector3(bagOrigin.position.x + bagRadius * Mathf.Cos(angle), bagOrigin.position.y + position2D.y, bagOrigin.position.z + bagRadius * Mathf.Sin(angle));
    }

    public Vector3 GetRotationOnbag (Vector2 position2D)
    {
        float angle = position2D.x * unitsToRadians * (180/pi);

        return new Vector3(0, -angle + 90, 0);
    }
}
