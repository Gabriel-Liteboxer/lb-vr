using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoteVisuals : MonoBehaviour
{
    public GameObject spriteRing;

    private void Awake()
    {
        spriteRing.SetActive(false);
    }

    public void CanBeHit(bool value)
    {
        spriteRing.SetActive(value);

    }

    public void SetColor(Color color)
    {
        Renderer rend = GetComponent<Renderer>();

        rend.material = new Material(rend.material);

        rend.material.color = color;

        rend.material.SetColor("_EmissionColor", color);

        SpriteRenderer sprite = GetComponentInChildren<SpriteRenderer>();

        sprite.color = color;

    }
}
