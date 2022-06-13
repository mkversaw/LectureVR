using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Assertions;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;
using System.Text.RegularExpressions;

namespace ViveSR
{
    namespace anipal
    {
        namespace Eye
        {
            public class viveEyeTrack : MonoBehaviour
            {
                public int LengthOfRay = 25;
                [SerializeField] private LineRenderer GazeRayRenderer;
                private static EyeData eyeData = new EyeData();
                [SerializeField] private Vector3 GazeDirectionCombined;
                [SerializeField] private string filepath;
                private bool eye_callback_registered = false;
                private float startTime;
                private float currentTime = 0;
                private bool eyesFailed;

                [SerializeField] private GameObject screen;

                [SerializeField] private GameObject objectwithheatmap;

                [SerializeField] private GameObject objectwithlog;

                [SerializeField] private GameObject objectWithCone;

                private void Start()
                {
                    startTime = Time.time;
                    VisionDataPreamble();
                    if (!SRanipal_Eye_Framework.Instance.EnableEye)
                    {

                        enabled = false;
                        return;
                    }
                    Assert.IsNotNull(GazeRayRenderer);

                }

                private void Update()
                {
                    currentTime += Time.deltaTime; // always increment the time!
                    if (SRanipal_Eye_Framework.Status != SRanipal_Eye_Framework.FrameworkStatus.WORKING && SRanipal_Eye_Framework.Status != SRanipal_Eye_Framework.FrameworkStatus.NOT_SUPPORT)
                    {
                        VisionDataWriter2(false);
                        return;
                    }

                    if (SRanipal_Eye_Framework.Instance.EnableEyeDataCallback == true && eye_callback_registered == false)
                    {
                        SRanipal_Eye.WrapperRegisterEyeDataCallback(Marshal.GetFunctionPointerForDelegate((SRanipal_Eye.CallbackBasic)EyeCallback));
                        eye_callback_registered = true;
                    }
                    else if (SRanipal_Eye_Framework.Instance.EnableEyeDataCallback == false && eye_callback_registered == true)
                    {
                        SRanipal_Eye.WrapperUnRegisterEyeDataCallback(Marshal.GetFunctionPointerForDelegate((SRanipal_Eye.CallbackBasic)EyeCallback));
                        eye_callback_registered = false;
                    }

                    Vector3 GazeOriginCombinedLocal, GazeDirectionCombinedLocal;

                    if (eye_callback_registered)
                    {
                        if (SRanipal_Eye.GetGazeRay(GazeIndex.COMBINE, out GazeOriginCombinedLocal, out GazeDirectionCombinedLocal, eyeData)) { }
                        else if (SRanipal_Eye.GetGazeRay(GazeIndex.LEFT, out GazeOriginCombinedLocal, out GazeDirectionCombinedLocal, eyeData)) { }
                        else if (SRanipal_Eye.GetGazeRay(GazeIndex.RIGHT, out GazeOriginCombinedLocal, out GazeDirectionCombinedLocal, eyeData)) { }
                        else
                        {
                            VisionDataWriter2(false);
                            return;
                        }
                    }
                    else
                    {
                        if (SRanipal_Eye.GetGazeRay(GazeIndex.COMBINE, out GazeOriginCombinedLocal, out GazeDirectionCombinedLocal)) { }
                        else if (SRanipal_Eye.GetGazeRay(GazeIndex.LEFT, out GazeOriginCombinedLocal, out GazeDirectionCombinedLocal)) { }
                        else if (SRanipal_Eye.GetGazeRay(GazeIndex.RIGHT, out GazeOriginCombinedLocal, out GazeDirectionCombinedLocal)) { }
                        else
                        {
                            VisionDataWriter2(false);
                            return;
                        }
                    }
                     
                    GazeDirectionCombined = Camera.main.transform.TransformDirection(GazeDirectionCombinedLocal);
                    GazeRayRenderer.SetPosition(0, Camera.main.transform.position - Camera.main.transform.up * 0.05f);
                    GazeRayRenderer.SetPosition(1, Camera.main.transform.position + GazeDirectionCombined * LengthOfRay);

                    VisionDataWriter2(true);
                }
                private void Release()
                {
                    if (eye_callback_registered == true)
                    {
                        SRanipal_Eye.WrapperUnRegisterEyeDataCallback(Marshal.GetFunctionPointerForDelegate((SRanipal_Eye.CallbackBasic)EyeCallback));
                        eye_callback_registered = false;
                    }
                }
                private static void EyeCallback(ref EyeData eye_data)
                {
                    eyeData = eye_data;
                }

                private void VisionDataPreamble()
                {
                    var digits = new[] { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' };
                    while (System.IO.File.Exists(filepath))
                    {

                        string[] parsedFile = filepath.Split('.'); // result should be two strings, the file name , and the file type

                        String result = Regex.Match(parsedFile[0], @"\d+$").Value; // check end of file name for a number

                        if(String.IsNullOrEmpty(result)) // if empty no number was found
                        {
                            parsedFile[0] += "1"; // add 1 as the first number
                            filepath = parsedFile[0] + "." + parsedFile[1]; // recombine the split strings to form the new filepath
                        } else
                        {
                            int numValue = Int32.Parse(result); // convert the (string) number to an int
                            numValue += 1; // increment the number by 1
                            parsedFile[0] = parsedFile[0].TrimEnd(digits); // remove the number from the end
                            parsedFile[0] += numValue.ToString(); // add the new number
                            filepath = parsedFile[0] + "." + parsedFile[1];
                        }
                        
                    }

                    string titleLine = "Time,Gaze Position,Camera Position,Head Rotation,Hit Object Name,Hit Position,Peripheral hitlist";


                    StreamWriter writer = new StreamWriter(filepath, true);
                    writer.WriteLine(titleLine);
                    writer.WriteLine();
                    writer.Close();
                }

                /// <summary>
                /// Returns the given string wrapped in quotes
                /// </summary>
                /// <param name="str"></param>
                /// <returns></returns>
                String quotifier(String str)
                {
                    return "\"" + str + "\"";
                }

                private void VisionDataWriter2(bool eyeEnabled)
                {
                    StreamWriter writer = new StreamWriter(filepath, true);
                    RaycastHit hit;
                    string time = currentTime.ToString();
                    if (!eyeEnabled)
                    {
                        writer.WriteLine(time + "," + "" + "," + "" + "," + "" + "," + "" + "," + ""); // write even if eye isnt found, but leave it all blank
                        writer.Close();
                        return; // don't need the rest
                    }

                    string eyeLook = GazeDirectionCombined.ToString();
                    string hitPosition = "";
                    string hitObjectName = "";
                    string headRotation = Camera.main.transform.rotation.ToString();
                    
                    string cameraPosition = Camera.main.transform.position.ToString();

                    if (Physics.Raycast(Camera.main.transform.position, GazeDirectionCombined, out hit))
                    {
                        objectwithlog.GetComponent<viveLookLog>().updateList(hit, hit.collider.gameObject.name);
                        if (hit.collider.gameObject.tag == "Screen") // if its the screen then update the heat map
                        {
                            objectwithheatmap.GetComponent<viveHeatMap>().updatePoint(hit.point.x, hit.point.y);
                        }

                        hitPosition = hit.point.ToString();
                        hitObjectName = hit.collider.gameObject.name.ToString();

                    }

                    string peripheryHitList = objectWithCone.GetComponent<viveVisionCone>().generateConeString(); // obtain all the objects in the peripheral vision from the cone

                    writer.WriteLine(time + "," + quotifier(eyeLook) + "," + quotifier(cameraPosition) + "," + quotifier(headRotation) + "," + hitObjectName + "," + quotifier(hitPosition) + "," + peripheryHitList);
                    writer.Close();
                }
            }
        }
    }
}
