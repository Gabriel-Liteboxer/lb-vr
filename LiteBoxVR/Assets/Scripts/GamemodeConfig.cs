using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Gamemode Config", menuName = "Gamemode Config")]
public class GamemodeConfig : ScriptableObject
{

    public string GamemodeName;

    public GameManager.Gamemode gamemode;

    public GameManager.BoardType boardType;

    public Texture Thumbnail;

}
