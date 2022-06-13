//========= Copyright 2018, HTC Corporation. All rights reserved. ===========
using System;
using UnityEngine;

namespace ViveSR
{
    namespace anipal
    {
        namespace Eye
        {
            public class viveGUI : MonoBehaviour
            {
                private void OnGUI() // ! This is only visible in the game view 
                {
                    if (GUILayout.Button("Launch Calibration"))
                    {
                        SRanipal_Eye_API.LaunchEyeCalibration(IntPtr.Zero);
                    }
                }
            }
        }
    }
}
