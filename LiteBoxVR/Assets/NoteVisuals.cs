using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoteVisuals : MonoBehaviour
{
    public GameObject spriteRing;

    public Material MyMat;

    private void Awake()
    {
        spriteRing.SetActive(false);

        //SetColor(Color.Lerp(Color.red, Color.green, Random.Range(0f, 1f)));

        //Debug.Log(Random.Range(0f, 1f));
    }

    public void CanBeHit(bool value)
    {
        spriteRing.SetActive(value);

    }

    

    public void SetColor(Color color)
    {
        

        Renderer rend = GetComponent<Renderer>();

        rend.material = new Material(rend.material);

        MyMat = rend.material;

        rend.material.color = color;

        rend.material.SetColor("_EmissionColor", color);

        SpriteRenderer sprite = spriteRing.GetComponent<SpriteRenderer>();

        sprite.color = color;

    }

    public void UpdateColor(Color color)
    {

        GetComponent<Renderer>().material.color = color;

        GetComponent<Renderer>().material.SetColor("_EmissionColor", color);

        SpriteRenderer sprite = spriteRing.GetComponent<SpriteRenderer>();

        sprite.color = color;

    }
}
