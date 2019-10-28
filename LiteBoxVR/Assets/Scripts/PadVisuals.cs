using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PadVisuals : MonoBehaviour
{
    public Sprite[] loadingSprites;

    public SpriteRenderer SprRend;

    float currentSprite;

    int currentSpriteIndex;

    public float pauseTimer;

    public float pauseLength;

    public float dissapearSpeed;

    void Start()
    {
        
        loadingSprites = Resources.LoadAll<Sprite>("PercentageRing");
    }

    
    public void PadHit(float accuracy)
    {
        currentSprite = Mathf.Lerp(0, loadingSprites.Length, accuracy);

        SprRend.color = Color.Lerp(Color.red, Color.green, accuracy);

        pauseTimer = pauseLength;

    }

    private void Update()
    {
        if (pauseTimer > 0)
        {
            pauseTimer -= Time.deltaTime;

        }
        else
        {
            currentSprite = Mathf.Lerp(currentSprite, 0, Time.deltaTime*dissapearSpeed);

        }

        currentSpriteIndex = (int)currentSprite;

        if (currentSpriteIndex > loadingSprites.Length)
            currentSpriteIndex = loadingSprites.Length;
        else
        if (currentSpriteIndex < 0)
            currentSpriteIndex = 0;

        SprRend.sprite = loadingSprites[currentSpriteIndex];

        if (currentSpriteIndex == 0)
            SprRend.sprite = null;
 
    }

}
