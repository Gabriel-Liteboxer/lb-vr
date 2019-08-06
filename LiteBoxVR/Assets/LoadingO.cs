using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadingO : MonoBehaviour
{
    public Sprite[] loadingSprites;

    SpriteRenderer SprRend;

    int currentSpriteIndex;

    // Start is called before the first frame update
    void Start()
    {
        SprRend = GetComponent<SpriteRenderer>();
        loadingSprites = Resources.LoadAll<Sprite>("LiteOLoading");
    }

    // Update is called once per frame
    void Update()
    {
        if (currentSpriteIndex >= loadingSprites.Length)
            currentSpriteIndex = 0;

        SprRend.sprite = loadingSprites[currentSpriteIndex];

        currentSpriteIndex++;
    }
}
