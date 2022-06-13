using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

public class viveFade : MonoBehaviour
{
    /// <summary>
    /// Make the screen turn black (permanently!)
    /// </summary>
    public void fadeToBlack()
    {
        SteamVR_Fade.View(Color.black, 0f);
    }
}