using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GamemodeTile : MonoBehaviour
{
    public Renderer ThumbnailImageRenderer;

    public SpriteRenderer BoardTypeIconRenderer;

    public Sprite[] BoardTypeIcons;

    public void SetTile(GamemodeConfig gamemodeConfig)
    {
        BoardTypeIconRenderer.sprite = BoardTypeIcons?[(int)gamemodeConfig.boardType];

        ThumbnailImageRenderer.material = new Material(ThumbnailImageRenderer.material);

        //ThumbnailImageRenderer.material.mainTexture = gamemodeConfig.Thumbnail;

        ThumbnailImageRenderer.material.SetTexture("_MainTex", gamemodeConfig.Thumbnail);
    }

    public Renderer GetTumbnailRenderer()
    {

        return ThumbnailImageRenderer;
    }
}
