using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Valve.VR;


public class viveResize : MonoBehaviour
{
    [SerializeField]
    private SteamVR_Input_Sources leftHand;
    [SerializeField]
    public SteamVR_Input_Sources rightHand;

    [SerializeField]
    private float height = 1.3f; // works well for ~5ft11

    [SerializeField]
    private SteamVR_Input_Sources headSet;

    /// <summary> Scales the view based on the user's height </summary>
    private void Resize()
    {
        float headHeight = Camera.main.transform.localPosition.y; // get the position of the top of their head from the vive camera
        float scale = height / headHeight; // divide the base height by the height of their head to determine a rough scale
        transform.localScale = Vector3.one * scale; // rescale the camera
    }

    public void EnableResize()
    {
        Resize();
    }

    void Update()
    {
        if (SteamVR_Input.GetStateDown("SideButton", leftHand) || SteamVR_Input.GetStateDown("SideButton", rightHand)) // check for input
        {
            Resize();
        }
    }
}
