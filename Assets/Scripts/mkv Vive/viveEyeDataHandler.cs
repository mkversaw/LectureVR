using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class viveEyeDataHandler : MonoBehaviour
{
    [SerializeField] private Bounds screenBoundary;
    [SerializeField] private string filepath;

    private Vector3 upperLeft;
    private Vector3 bottomRight;

    private void Start()
    {
        getCorners();

    }

    private void make2DArray()
    {
        //int arraySize = ((int)Mathf.Abs(upperLeft.x) * 100) * ((int)Mathf.Abs(upperLeft.x) * 100);


    }

    private void getCorners()
    {
         upperLeft = new Vector3(screenBoundary.max.x, screenBoundary.max.y, 0.0f);
         bottomRight = new Vector3(screenBoundary.min.x, screenBoundary.min.y, 0.0f);
    }
}
