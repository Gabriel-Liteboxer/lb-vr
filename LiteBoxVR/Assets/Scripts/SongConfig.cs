using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Song Config", menuName = "Song Config")]
public class SongConfig : ScriptableObject
{

    public Song song;
    public TextAsset songFile;

}
