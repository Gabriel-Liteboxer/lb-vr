using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleJSON;

public class JsonDecoder : MonoBehaviour
{
    [System.Serializable]
    public class NoteData
    {
        public int pad;

        public int time;

    } 

    public NoteData[] GetNoteDataFromJson(TextAsset SongJson)
    {
        NoteData[] newNoteData;

        SimpleJSON.JSONNode data = SimpleJSON.JSON.Parse(SongJson.text);
        //Debug.Log("Tempo " + data["tempo"]);

        //SongTempo = data["tempo"];

        newNoteData = new NoteData[data["tracks"].Count];

        //Debug.Log("Tracks Count " + data["tracks"].Count);

        for (int i = 0; i < data["tracks"].Count; i++)
        {
            newNoteData[i] = new NoteData();

            newNoteData[i].pad = data["tracks"][i]["p"];

            newNoteData[i].time = data["tracks"][i]["t"];

            //Debug.Log("note pad " + i + ": " + newNoteData[i].pad);

            //Debug.Log("note time " + i + ": " + newNoteData[i].time);

        }

        return newNoteData;

    }
}
