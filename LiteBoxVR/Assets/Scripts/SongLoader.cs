using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using RuntimeAudioClipLoader;

public class SongLoader : MonoBehaviour
{

    [SerializeField] bool baked;
    [SerializeField] List<SongConfig> bakedTracks;
    public SongLibrary songLibrary;

    public void Load()
    {
        songLibrary = new SongLibrary();
        songLibrary.songs = new List<Song>();

        RyansMIDILibrary MIDI = new RyansMIDILibrary();

        if (baked) //Files are set in the inspector
        {
            foreach (SongConfig config in bakedTracks)
            {
                LAVA lavaParser = new LAVA();
                lavaParser.Load(config.songFile.bytes);
                //Debug.Log(lavaParser.oggOffset + " | " + lavaParser.oggLength + " | " + lavaParser.midiOffset + " | " + lavaParser.midiLength);

                //Parse MIDI
                Song midi = MIDI.LoadMIDI(lavaParser.midiData);
                for (int i = 0; i < 3; i++)
                {
                    config.song.difficulties[i] = midi.difficulties[i];
                }
                config.song.tempoEvents = midi.tempoEvents;

                //Parse OGG
                var audioConfig = new AudioLoaderConfig();
                audioConfig.DataStream = new MemoryStream(lavaParser.oggData);
                config.song.loader = new AudioLoader(audioConfig);
                config.song.loader.StartLoading();

                songLibrary.songs.Add(config.song);
            }
        }
        else //Files are downloaded to the persistent data path
        {

        }
    }

}
