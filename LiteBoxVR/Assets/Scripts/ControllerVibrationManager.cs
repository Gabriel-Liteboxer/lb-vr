using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControllerVibrationManager : MonoBehaviour
{
    private float VibrationFrequencyL;

    private float VibrationAmplitudeL;

    private float VibrationFrequencyR;

    private float VibrationAmplitudeR;


    public void AddVibration(bool isLeftController, float amplitude, float frequency)//adds vibration for one frame
    {

        if(isLeftController)
        {
            VibrationFrequencyL += frequency;
            VibrationAmplitudeL += amplitude;
        }
        else
        {
            VibrationFrequencyR += frequency;
            VibrationAmplitudeR += amplitude;
        }



        //locking the values so the controller doesn't over-vibrate
        if (VibrationFrequencyL > 1)
            VibrationFrequencyL = 1;

        if (VibrationAmplitudeL > 1)
            VibrationAmplitudeL = 1;

        if (VibrationFrequencyR > 1)
            VibrationFrequencyR = 1;

        if (VibrationAmplitudeR > 1)
            VibrationAmplitudeR = 1;
    }

    public void Update()
    {
        OVRInput.SetControllerVibration(VibrationFrequencyR, VibrationAmplitudeR, OVRInput.Controller.RTouch);

        OVRInput.SetControllerVibration(VibrationFrequencyL, VibrationAmplitudeL, OVRInput.Controller.LTouch);

        //resetting the values
        VibrationAmplitudeL = 0;
        VibrationFrequencyL = 0;

        VibrationAmplitudeR = 0;
        VibrationFrequencyR = 0;
    }
}
