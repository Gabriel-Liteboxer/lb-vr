using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class HitMarkerInfo : MonoBehaviour
{
    private TextMeshPro textMesh;

    private Renderer textRend;

    public void SetHitMarker(int aScore, Color aColor, float aLifetime)
    {
        textMesh = GetComponent<TextMeshPro>();

        textRend = GetComponent<MeshRenderer>();

        textRend.material = new Material(textRend.material);

        textMesh.text = aScore.ToString();

        textMesh.outlineColor = (Color32)aColor;

        Destroy(gameObject, aLifetime);

    }

    private void Update()
    {
        transform.position += transform.up * Time.deltaTime * 0.05f;
    }
}
