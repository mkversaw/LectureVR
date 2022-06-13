using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Valve.VR;

public class viveMenuAdjust : MonoBehaviour
{
    [SerializeField]
    private SteamVR_Input_Sources leftHand;
    [SerializeField]
    public SteamVR_Input_Sources rightHand;


    [SerializeField]
    private GameObject[] objects;

    [SerializeField]
    private float height_adjustment_factor;
    
    private Vector3[] original_position_vectors;
    private Vector3[] transforms;

    /// <summary>
    /// obtain list of the original positions, to return to later
    /// </summary>
    private void Start()
    {
        original_position_vectors = new Vector3[objects.Length];
        for (int i = 0; i < original_position_vectors.Length; i++)
        {
            original_position_vectors[i] = objects[i].transform.position;
        }
    }

    /// <summary>
    /// given a list of game objects will move them up or down by the height factor, direction dependent on value of bool: 'up'
    /// </summary>
    /// <param name="_objects"> The list of objects to move </param>
    /// <param name="up"> which direction to move the objects </param>
    private void Move(GameObject[] _objects, bool up) // 
    {
        int direction = -1;
        if(up)
        {
            direction = 1;
        }

        for (int i = 0; i < _objects.Length; i++)
        {
            Vector3 temp_pos = _objects[i].transform.position;
            temp_pos.y += direction * height_adjustment_factor;
            _objects[i].transform.position = temp_pos;
        }
    }

    /// <summary>
    /// Return the objects to their original positions
    /// </summary>
    /// <param name="_objects"></param>
    private void Reset(GameObject[] _objects)
    {
        for (int i = 0; i < original_position_vectors.Length; i++)
        {
            objects[i].transform.position = original_position_vectors[i];
        }
    }

    private void Update()
    {
        if (SteamVR_Input.GetStateDown("menuUp", leftHand) || SteamVR_Input.GetStateDown("menuUp", rightHand)) {
            Move(objects, true);
        }

        if (SteamVR_Input.GetStateDown("menuDown", leftHand) || SteamVR_Input.GetStateDown("menuDown", rightHand))
        {
            Move(objects, false);
        }

        if (SteamVR_Input.GetStateDown("Trigger", leftHand) || SteamVR_Input.GetStateDown("Trigger", rightHand))
        {
            Reset(objects);
        }
    }

    /// <summary>
    /// Prints which hand (controller) moved the menu, and which direction
    /// </summary>
    private void handDebugger()
    {
        if (SteamVR_Input.GetStateDown("menuUp", leftHand))
        {
            Debug.Log("left hand menu up");
        }

        if (SteamVR_Input.GetStateDown("menuUp", rightHand))
        {
            Debug.Log("right hand menu up");
        }

        if (SteamVR_Input.GetStateDown("menuDown", leftHand))
        {
            Debug.Log("left hand menu down");
        }

        if (SteamVR_Input.GetStateDown("menuDown", rightHand))
        {
            Debug.Log("right hand menu down");
        }
    }
}
