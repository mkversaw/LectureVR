using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class viveVisionCone : MonoBehaviour
{
    private int numRays = 30; // needs to be a divisor of 360, i.e. 30, 60, 120, but not 181
    public List<RaycastHit> generateCone()
    {
        List<RaycastHit> hitList = new List<RaycastHit>(); // not *that* kind of hitlist !
        float subDegree = 360 / numRays;
        for (int i = 0; i < numRays; i++) // generate a cone line by line
        {
            float rayRadian = ( (subDegree * i) * Mathf.PI) / 180; // degree to radians

            Vector3 dir = 5 * transform.right * Mathf.Cos(rayRadian) + 5 * transform.up * Mathf.Sin(rayRadian) + 10*transform.forward; 

            RaycastHit hit;
            if (Physics.Raycast(Camera.main.transform.position, dir, out hit))
            {
                hitList.Add(hit);
            }
            
        }
        return hitList;
    }

    /// <summary>
    ///  Generates a cone of raycasts as a rough estimate of the peripheral vision
    /// </summary>
    public string generateConeString()
    {
        string hitList = "\"";
        float subDegree = 360 / numRays;
        for (int i = 0; i < numRays; i++)
        {
            float rayRadian = ((subDegree * i) * Mathf.PI) / 180; // degrees to radians

            // adjacent = cos(theta) * hypotenuse
            // opposite = sin(theta) * hypotenuse
            // scale the adjacent and opposite by ~half of height for a relatively good peripheral vision cone
            Vector3 dir = 5 * transform.right * Mathf.Cos(rayRadian) + 5 * transform.up * Mathf.Sin(rayRadian) + 10 * transform.forward;

            RaycastHit hit;
            if (Physics.Raycast(Camera.main.transform.position, dir, out hit))
            {
                string hitPosition = hit.point.ToString();
                string hitObjectName = hit.collider.gameObject.name.ToString();
                hitList += hitObjectName + "|" + hitPosition + "&";
            }
        }

        if (hitList[hitList.Length - 1] == '&') // remove an extra comma if one occurs
        {
            hitList.Remove(hitList.Length - 1, 1);
        }

        hitList += "\"";

        return hitList;
    }

    /// <summary>
    ///  Draws debug rays to make the cone visible
    /// </summary>
    void debugCone()
    {
            float subDegree = 360 / numRays;
            for (int i = 0; i < numRays; i++)
            {
                float rayRadian = ((subDegree * i) * Mathf.PI) / 180; // degree to radians

                Vector3 dir = 5 * transform.right * Mathf.Cos(rayRadian) + 5 * transform.up * Mathf.Sin(rayRadian) + 10 * transform.forward;

                Debug.DrawRay(Camera.main.transform.position, dir, Color.blue);

            }
            Vector3 forward = transform.TransformDirection(Vector3.forward) * 10; // center ray
            Debug.DrawRay(Camera.main.transform.position, forward, Color.red);
    }
}
