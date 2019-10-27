using System.Collections;
using System.Collections.Generic;
using System.IO;
using MidiPlayerTK;

public class RyansMIDILibrary
{

    Dictionary<int, int> NoteToPuck = new Dictionary<int, int>();

    public RyansMIDILibrary() {
        InitDictionary();
    }

    enum WriteMode {
        None,
        MIDITicks,
        Milliseconds
    }

    void InitDictionary() {

        if (!NoteToPuck.ContainsKey(60)) {
            NoteToPuck.Add(48, 3); //C3
            NoteToPuck.Add(50, 5); //D3
            NoteToPuck.Add(52, 4); //E3
            NoteToPuck.Add(53, 1); //F3
            NoteToPuck.Add(55, 0); //G3
            NoteToPuck.Add(57, 2); //A3

            NoteToPuck.Add(60, 3); //C4
            NoteToPuck.Add(62, 5); //D4
            NoteToPuck.Add(64, 4); //E4
            NoteToPuck.Add(65, 1); //F4
            NoteToPuck.Add(67, 0); //G4
            NoteToPuck.Add(69, 2); //A4

            NoteToPuck.Add(72, 3); //C5
            NoteToPuck.Add(74, 5); //D5
            NoteToPuck.Add(76, 4); //E5
            NoteToPuck.Add(77, 1); //F5
            NoteToPuck.Add(79, 0); //G5
            NoteToPuck.Add(81, 2); //A5
        }

    }

    MidiLoad midi;
    Song track;

    public Song LoadMIDI(string filePath) {

        midi = new MidiLoad();
        midi.MPTK_Load(filePath, true);

        Load();

        WriteLog(WriteMode.Milliseconds, filePath + ".txt");

        return track;

    }

    public Song LoadMIDI(byte[] file) {

        midi = new MidiLoad();
        midi.MPTK_Load(file);

        Load();

        //WriteLog(WriteMode.TotalTime, "C:/Users/Nuc/Documents/Calling All Stars.midi.txt");

        return track;

    }

    Song Load() { //Call this after your preferred MPTK_Load overload

        //Set up the track list item we're going to be returning
        track = new Song();
        for (int i = 0; i < 3; i++) {
            track.difficulties[i] = new NotesForDifficulty();
            track.difficulties[i].notes = new List<Note>();
        }

        //TODO: Handle tempo changes
        int tempo = 50000;
        if (midi.midifile.Events.GetTrackEvents(0).Count == 0) {
            track.tempoEvents.Add(new SetTempoEvent(tempo, 0, 0));
        }
        else {
            int first = 0;
            foreach (var msg in midi.midifile.Events.GetTrackEvents(0)) {
                if (msg.GetType().ToString() == "NAudio.Midi.TempoEvent") {
                    if (first == 0) {
                        midi.ChangeTempo(((NAudio.Midi.TempoEvent)msg).Tempo);
                        track.tempoEvents.Add(new SetTempoEvent(((NAudio.Midi.TempoEvent)msg).Tempo, (int)msg.AbsoluteTime, msg.AbsoluteTime * midi.TickLengthMs));
                        first++;
                    }
                    else {
                        track.tempoEvents.Add(new SetTempoEvent(((NAudio.Midi.TempoEvent)msg).Tempo, (int)msg.AbsoluteTime, track.tempoEvents[track.tempoEvents.Count - 1].absoluteMillisecondTime + ((msg.AbsoluteTime - track.tempoEvents[track.tempoEvents.Count - 1].absoluteMidiTickTime) * midi.TickLengthMs)));
                        midi.ChangeTempo(((NAudio.Midi.TempoEvent)msg).Tempo);
                    }
                }
            }
        }

        //Convert midi data to track list item
        for (int i = 0; i < 3; i++) {
            int currentTempoChangeIndex = 0;
            midi.ChangeTempo(track.tempoEvents[currentTempoChangeIndex].tempo);
            foreach (var msg in midi.midifile.Events.GetTrackEvents(i + 2)) {
                if (msg.GetType().ToString() == "NAudio.Midi.NoteOnEvent" && ((NAudio.Midi.NoteOnEvent)msg).Velocity != 0) {
                    while (currentTempoChangeIndex + 1 < track.tempoEvents.Count && track.tempoEvents[currentTempoChangeIndex + 1].absoluteMidiTickTime <= msg.AbsoluteTime) {
                        currentTempoChangeIndex++;
                        midi.ChangeTempo(track.tempoEvents[currentTempoChangeIndex].tempo);
                    }
                    track.difficulties[i].notes.Add(new Note((uint)(track.tempoEvents[currentTempoChangeIndex].absoluteMillisecondTime + ((msg.AbsoluteTime - track.tempoEvents[currentTempoChangeIndex].absoluteMidiTickTime) * midi.TickLengthMs)), (uint)NoteToPuck[((NAudio.Midi.NoteOnEvent)msg).NoteNumber]));
                }
            }
        }

        return track;

    }

    void WriteLog(WriteMode write, string writePath) {
        if (write == WriteMode.MIDITicks) {
            StreamWriter writer = new StreamWriter(writePath);
            for (int i = 0; i < midi.midifile.Tracks; i++) {
                writer.WriteLine("Track " + i);
                if (i < 2) {
                    foreach (var msg in midi.midifile.Events.GetTrackEvents(i)) {
                        writer.WriteLine(msg.ToString());
                    }
                }
                else {
                    foreach (var msg in midi.midifile.Events.GetTrackEvents(i)) {
                        if (msg.GetType().ToString() == "NAudio.Midi.NoteOnEvent" && ((NAudio.Midi.NoteOnEvent)msg).Velocity != 0) {
                            writer.WriteLine(msg.ToString());
                        }
                    }
                }
                writer.WriteLine();
            }
            writer.Close();
        }
        else if (write == WriteMode.Milliseconds) {
            StreamWriter writer = new StreamWriter(writePath);
            for (int i = 0; i < midi.midifile.Tracks; i++) {
                writer.WriteLine("Track " + i);
                if (i < 2) {
                    foreach (var msg in midi.midifile.Events.GetTrackEvents(i)) {
                        writer.WriteLine(msg.ToString());
                    }
                }
                else {
                    foreach (var msg in track.difficulties[i - 2].notes) {
                        writer.WriteLine("Time (ms): " + msg.time + ", Pad: " + msg.pad);
                    }
                }
                writer.WriteLine();
            }
            writer.Close();
        }
    }


    /*
    //Can cast to subclass; make sure you know it is that first though
    if (e.GetType().ToString() == "Midi.Events.ChannelEvents.ControllerEvent") {
        Debug.Log(((Midi.Events.ChannelEvents.ControllerEvent)e).parameter_1);
    }
    */
}
