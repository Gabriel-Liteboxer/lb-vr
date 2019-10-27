using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RuntimeAudioClipLoader;

[System.Serializable]
public class SetTempoEvent
{

    public int absoluteMidiTickTime;
    public double absoluteMillisecondTime;
    public double tempo;

    public SetTempoEvent()
    {

    }

    public SetTempoEvent(double tempo, int absoluteMidiTickTime, double absoluteMillisecondTime)
    {
        this.tempo = tempo;
        this.absoluteMidiTickTime = absoluteMidiTickTime;
        this.absoluteMillisecondTime = absoluteMillisecondTime;
    }

}

[System.Serializable]
public class SongLibrary
{

    public List<Song> songs;

}

[System.Serializable]
public class Song
{

    //Info
    public int id;
    public string artist;
    public string genre;
    public string name;
    public Texture2D albumArt;
    public bool purchased;
    public string price;
    public string storeId;

    //Song data
    public NotesForDifficulty[] difficulties = new NotesForDifficulty[3];
    AudioClip[] audio = new AudioClip[3];
    public AudioLoader loader;
    bool setAudio = false;
    public AudioClip Audio(int index)
    {
        if (setAudio && loader.IsLoadingDone && audio[0] == null)
        {
            setAudio = false;
        }

        if (index < 0 || index >= audio.Length)
        {
            return null;
        }
        else if (loader != null && !loader.IsLoadingDone)
        {
            return null;
        }
        else if (loader != null && loader.IsLoadingDone && !setAudio)
        {
            for (int i = 0; i < audio.Length; i++)
            {
                audio[i] = loader.AudioClip;
            }
            setAudio = true;
            return audio[index];
        }
        else
        {
            return audio[index];
        }
    }
    public void SetAudio(int index, AudioClip clip)
    {
        audio[index] = clip;
    }
    public List<SetTempoEvent> tempoEvents = new List<SetTempoEvent>();

}

[System.Serializable]
public class NotesForDifficulty
{

    public List<Note> notes;

}

[System.Serializable]
public class Note
{

    public uint time; //In Milliseconds
    public uint pad; //See pad diagram at https://docs.google.com/document/d/1r4v7SJRSzFcRo1dd_7oMgUA57AYPTYV1Wt869ILAmMM/edit
    public uint style;

    public Note()
    {
        time = 0;
        pad = 0;
        style = 0;
    }

    public Note(uint time, uint pad)
    {
        this.time = time;
        this.pad = pad;
        style = 0;
    }

    public override string ToString()
    {
        return "Punch at " + time + "ms on pad " + pad;
    }

}