using System;
using System.Collections;
using System.Collections.Generic;

public class LAVA
{

    //Header Info
    public uint fileVersion;
    public uint oggOffset;
    public uint oggLength;
    public uint midiOffset;
    public uint midiLength;

    //The relevant files
    public byte[] oggData;
    public byte[] midiData;

    //For reading from disk
    System.IO.FileStream LAVABin;
    byte[] readBytes;
    byte[] bufferArray; //Used to pass parts of readBytes to functions like byteArrayToUint

    public void Load(string filePath) {
        //Set up data
        LAVABin = System.IO.File.OpenRead(filePath);
        readBytes = new byte[LAVABin.Length];

        //Read header
        readToUint(ref fileVersion, 0, 4);
        readToUint(ref oggOffset, 4, 4);
        readToUint(ref oggLength, 8, 4);
        readToUint(ref midiOffset, 12, 4);
        readToUint(ref midiLength, 16, 4);

        //Read ogg
        setFileArray(ref oggData, (int)oggOffset, (int)oggLength);

        //Read midi
        setFileArray(ref midiData, (int)midiOffset, (int)midiLength);

        //Close out
        LAVABin.Close();
    }

    public void Load(byte[] file)
    {
        //Read header
        for (int i = 0; i < 5; i++)
        {
            byte[] data = new byte[4];
            int count = 0;
            for (int j = i * 4; j < (i * 4) + 4; j++)
            {
                data[count++] = file[j];
            }
            switch (i)
            {
                case 0:
                    fileVersion = byteArrayToUint(data);
                    break;
                case 1:
                    oggOffset = byteArrayToUint(data);
                    break;
                case 2:
                    oggLength = byteArrayToUint(data);
                    break;
                case 3:
                    midiOffset = byteArrayToUint(data);
                    break;
                case 4:
                    midiLength = byteArrayToUint(data);
                    break;
                default:
                    break;
            }
        }

        //Read ogg
        oggData = new byte[oggLength];
        Array.Copy(file, oggOffset, oggData, 0, oggLength);

        //Read midi
        midiData = new byte[midiLength];
        Array.Copy(file, midiOffset, midiData, 0, midiLength);
    }

    void setBufferArray(int offset, int length) { //Used to pass parts of readBytes to functions like byteArrayToUint
        bufferArray = new byte[length];
        int bufferCount = 0;
        for (int i = offset; i < offset + length; i++) {
            bufferArray[bufferCount] = readBytes[i];
            bufferCount++;
        }
    }

    uint byteArrayToUint(byte[] array) { //Takes a byte array, returns equivalent uint
        if (BitConverter.IsLittleEndian) {
            Array.Reverse(array); //Shouldn't affect the given array because C# is default pass-by-value
        }
        return BitConverter.ToUInt32(array, 0);
    }

    void readToUint(ref uint readTo, int offset, int length) { //Reads length bytes from offset of LAVA file, sets ref uint to result
        LAVABin.Read(readBytes, offset, length);
        setBufferArray(offset, length);
        readTo = byteArrayToUint(bufferArray);
    }

    void setFileArray(ref byte[] array, int offset, int length) { //Reads length bytes from offset of LAVA file, sets ref array to result
        if (length <= 0) {
            //UnityEngine.Debug.LogError("Length error");
            return;
        }
        LAVABin.Position = offset;
        array = new byte[length];
        LAVABin.Read(array, 0, length);
    }

}
