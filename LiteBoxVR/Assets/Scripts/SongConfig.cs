using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Song Config", menuName = "Song Config")]
public class SongConfig : ScriptableObject
{
    [System.Serializable]
    public class SongDifficultySettings
    {
        public TextAsset TrackJson;

        public AudioClip audioClip;
 
    }

    public string SongName;

    public string ArtistName;

    public Texture AlbumCover;

    public SongDifficultySettings[] DifficultyLevels;

}
