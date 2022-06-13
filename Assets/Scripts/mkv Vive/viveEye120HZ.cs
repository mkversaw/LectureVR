using UnityEngine;
using ViveSR.anipal.Eye;
using System.Runtime.InteropServices;
using System.IO;
using System;
using System.Text.RegularExpressions;
using UnityEngine.SceneManagement;

/// <summary>
/// Example usage for eye tracking callback
/// Note: Callback runs on a separate thread to report at ~120hz.
/// Unity is not threadsafe and cannot call any UnityEngine api from within callback thread.
/// </summary>
public class viveEye120HZ : MonoBehaviour
{

	public int LengthOfRay = 25;
	[SerializeField] private LineRenderer GazeRayRenderer;
	[SerializeField] private Vector3 GazeDirectionCombined;

	[SerializeField] private string filepath = "D:\\Miles\\60HZData\\60data.csv";
	[SerializeField] private string callBackFilePath = "D:\\Miles\\120HZData\\120data.csv";
	[SerializeField] private string eventFilePath = "D:\\Miles\\eventData\\eventData.csv";
	[SerializeField] private string slidesFilePath = "D:\\Miles\\eventData\\slideData.csv";

	//[SerializeField] private GameObject screen;

	//[SerializeField] private GameObject objectwithheatmap;

	//[SerializeField] private GameObject objectWithCone;

	private static EyeData eyeData = new EyeData();
	private static bool eye_callback_registered = false;

	private static StreamWriter writer;
	private static StreamWriter eventWriter;
	private static StreamWriter slideWriter;
	private static StreamWriter callBackWriter;

	//private int callBackUpdateSpeed = 0;
	//private static int callBackTimeStamp2 = 0;
	//private static int callBackLastTime = 0;
	//private static int currentTime2 = 0;

	private static long callBackUpdateSpeed = 0;
	private static long callBackTimeStamp2 = 0;
	private static long callBackLastTime = 0;
	private static long currentTime2 = 0;


	private static int slide = 1;



	private void Start()
	{
		VisionDataPreamble(filepath);
		CallBackVisionDataPreamble(callBackFilePath);
		EventDataPreamble(eventFilePath);
		slidesPreamble(slidesFilePath);
	}

	private void Update()
	{
		if(callBackLastTime != 0)
        {
			callBackUpdateSpeed = currentTime2 - callBackLastTime;
			callBackTimeStamp2 += callBackUpdateSpeed;
		}

		// currentTime3 += Time.deltaTime;
		//Debug.Log("timeStamp: " + callBackTimeStamp2.ToString() + " ms");
		//Debug.Log("other: " + currentTime2 + "," + callBackLastTime);

		if (SRanipal_Eye_Framework.Status != SRanipal_Eye_Framework.FrameworkStatus.WORKING) return;

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

	private void OnDisable()
	{
		Release();
	}

	void OnApplicationQuit()
	{
		Release();
		writer.Close();
		callBackWriter.Close();
		eventWriter.Close();
		slideWriter.Close();
	}

	/// <summary>
	/// Release callback thread when disabled or quit
	/// </summary>
	private static void Release()
	{
		if (eye_callback_registered == true)
		{
			SRanipal_Eye.WrapperUnRegisterEyeDataCallback(Marshal.GetFunctionPointerForDelegate((SRanipal_Eye.CallbackBasic)EyeCallback));
			eye_callback_registered = false;
		}
	}

	/// <summary>
	/// Required class for IL2CPP scripting backend support
	/// </summary>
	internal class MonoPInvokeCallbackAttribute : System.Attribute
	{
		public MonoPInvokeCallbackAttribute() { }
	}

	/// <summary>
	/// Eye tracking data callback thread.
	/// Reports data at ~120hz
	/// MonoPInvokeCallback attribute required for IL2CPP scripting backend
	/// </summary>
	/// <param name="eye_data">Reference to latest eye_data</param>
	[MonoPInvokeCallback]

	private static String quotifier2(String str)
	{
		return "\"" + str + "\"";
	}

	// https//www.ncbi.nlm.nih.gov/pmc/articles/PMC7527608/
	private static void EyeCallback(ref EyeData eye_data)
	{

		eyeData = eye_data;
		callBackLastTime = currentTime2;
		currentTime2 = eyeData.timestamp;

		//Debug.Log(eyeData.timestamp + "," + eyeData.frame_sequence);

		// do stuff with eyeData...

		VerboseData dataObj = eyeData.verbose_data;
		SingleEyeData dataLeftEye = dataObj.left;
		SingleEyeData dataRightEye = dataObj.right;

		//Vector3 gaze_origin_mm_LEFT = dataLeftEye.gaze_origin_mm;
		//Vector3 gaze_direction_normalized_LEFT = dataLeftEye.gaze_direction_normalized;
		//float pupil_diameter_mm_LEFT = dataLeftEye.pupil_diameter_mm;
		//float eye_openness_LEFT = dataLeftEye.eye_openness;
		//Vector2 pupil_position_in_sensor_area_LEFT = dataLeftEye.pupil_position_in_sensor_area;


		//Vector3 gaze_origin_mm_RIGHT = dataRightEye.gaze_origin_mm;
		//Vector3 gaze_direction_normalized_RIGHT = dataRightEye.gaze_direction_normalized;
		//float pupil_diameter_mm_RIGHT = dataRightEye.pupil_diameter_mm;
		//float eye_openness_RIGHT = dataRightEye.eye_openness;
		//Vector2 pupil_position_in_sensor_area_RIGHT = dataRightEye.pupil_position_in_sensor_area;

		// String title = "LEFT: gaze origin (mm),gaze direction normalized (0 to 1),pupil diameter (mm),eye_openness,pupil position in sensor area (0 to 1),RIGHT: gaze origin (mm),gaze direction normalized (0 to 1),pupil diameter (mm),eye_openness,pupil position in sensor area (0 to 1)";

		String strData = quotifier2(dataLeftEye.gaze_origin_mm.ToString()) + "," + quotifier2(dataLeftEye.gaze_direction_normalized.ToString()) + "," + dataLeftEye.pupil_diameter_mm.ToString()
		+ "," + dataLeftEye.eye_openness.ToString() + "," + quotifier2(dataLeftEye.pupil_position_in_sensor_area.ToString()) + "," + quotifier2(dataRightEye.gaze_origin_mm.ToString())
		+ "," + quotifier2(dataRightEye.gaze_direction_normalized.ToString()) + "," + dataRightEye.pupil_diameter_mm.ToString()
		+ "," + dataRightEye.eye_openness.ToString() + "," + quotifier2(dataRightEye.pupil_position_in_sensor_area.ToString());

		//String combinedData = callBackTimeStamp2.ToString() + "," + strData;

		callBackWriter.WriteLine((callBackTimeStamp2.ToString() + "," + slide + "," + strData));
		//callBackWriter.WriteLine(combinedData);
	}

	private static void VisionDataPreamble(string filepath)
	{
		var digits = new[] { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' };

		string[] splitTemp = filepath.Split('.');
		filepath = splitTemp[0] + "_" + SceneManager.GetActiveScene().name + "." + splitTemp[1]; // add the name of the scene to the front of the file

		while (System.IO.File.Exists(filepath))
		{

			string[] parsedFile = filepath.Split('.'); // result should be two strings, the file name , and the file type

			String result = Regex.Match(parsedFile[0], @"\d+$").Value; // check end of file name for a number

			if (String.IsNullOrEmpty(result)) // if empty no number was found
			{
				parsedFile[0] += "1"; // add 1 as the first number
				filepath = parsedFile[0] + "." + parsedFile[1]; // recombine the split strings to form the new filepath
			}
			else
			{
				int numValue = Int32.Parse(result); // convert the (string) number to an int
				numValue += 1; // increment the number by 1
				parsedFile[0] = parsedFile[0].TrimEnd(digits); // remove the number from the end
				parsedFile[0] += numValue.ToString(); // add the new number
				filepath = parsedFile[0] + "." + parsedFile[1];
			}

		}

		string titleLine = "Time,Gaze Position,Camera Position,Head Rotation,Hit Object Name,Hit Position";


		writer = new StreamWriter(filepath, true);
		writer.WriteLine(titleLine);
		writer.WriteLine();
	}


	private static void CallBackVisionDataPreamble(string callBackFilePath)
	{
		var digits = new[] { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' };

		string[] splitTemp = callBackFilePath.Split('.');
		callBackFilePath = splitTemp[0] + "_" + SceneManager.GetActiveScene().name + "." + splitTemp[1]; // add the name of the scene to the front of the file

		while (System.IO.File.Exists(callBackFilePath))
		{

			string[] parsedFile = callBackFilePath.Split('.'); // result should be two strings, the file name , and the file type

			String result = Regex.Match(parsedFile[0], @"\d+$").Value; // check end of file name for a number

			if (String.IsNullOrEmpty(result)) // if empty no number was found
			{
				parsedFile[0] += "1"; // add 1 as the first number
				callBackFilePath = parsedFile[0] + "." + parsedFile[1]; // recombine the split strings to form the new filepath
			}
			else
			{
				int numValue = Int32.Parse(result); // convert the (string) number to an int
				numValue += 1; // increment the number by 1
				parsedFile[0] = parsedFile[0].TrimEnd(digits); // remove the number from the end
				parsedFile[0] += numValue.ToString(); // add the new number
				callBackFilePath = parsedFile[0] + "." + parsedFile[1];
			}

		}

		String titleLine = "Time,Slide,LEFT gaze origin (mm),LEFT gaze direction normalized,LEFT pupil diameter (mm),LEFT eye openness,LEFT pupil position,RIGHT gaze origin (mm),RIGHT gaze direction normalized,RIGHT pupil diameter (mm),RIGHT eye openness,RIGHT pupil position";


		callBackWriter = new StreamWriter(callBackFilePath, true);
		callBackWriter.WriteLine(titleLine);
		callBackWriter.WriteLine();
	}

	String quotifier(String str)
	{
		return "\"" + str + "\"";
	}

	private static void EventDataPreamble(string eventFilePath)
	{
		var digits = new[] { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' };

		string[] splitTemp = eventFilePath.Split('.');
		eventFilePath = splitTemp[0] + " " + SceneManager.GetActiveScene().name + "." + splitTemp[1]; // add the name of the scene to the front of the file

		while (System.IO.File.Exists(eventFilePath))
		{

			string[] parsedFile = eventFilePath.Split('.'); // result should be two strings, the file name , and the file type

			String result = Regex.Match(parsedFile[0], @"\d+$").Value; // check end of file name for a number

			if (String.IsNullOrEmpty(result)) // if empty no number was found
			{
				parsedFile[0] += "1"; // add 1 as the first number
				eventFilePath = parsedFile[0] + "." + parsedFile[1]; // recombine the split strings to form the new filepath
			}
			else
			{
				int numValue = Int32.Parse(result); // convert the (string) number to an int
				numValue += 1; // increment the number by 1
				parsedFile[0] = parsedFile[0].TrimEnd(digits); // remove the number from the end
				parsedFile[0] += numValue.ToString(); // add the new number
				eventFilePath = parsedFile[0] + "." + parsedFile[1];
			}

		}

		string titleLine = "Time,Event";

		eventWriter = new StreamWriter(eventFilePath, true);
		eventWriter.WriteLine(titleLine);
		eventWriter.WriteLine();
	}

	public void EventDataWriter(GameObject obj)
	{
		string time = callBackTimeStamp2.ToString();
		eventWriter.WriteLine(time + "," + obj.name);
	}

	private static void slidesPreamble(string slidesFilePath)
	{
		var digits = new[] { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' };

		string[] splitTemp = slidesFilePath.Split('.');
		slidesFilePath = splitTemp[0] + " " + SceneManager.GetActiveScene().name + "." + splitTemp[1]; // add the name of the scene to the front of the file

		while (System.IO.File.Exists(slidesFilePath))
		{

			string[] parsedFile = slidesFilePath.Split('.'); // result should be two strings, the file name , and the file type

			String result = Regex.Match(parsedFile[0], @"\d+$").Value; // check end of file name for a number

			if (String.IsNullOrEmpty(result)) // if empty no number was found
			{
				parsedFile[0] += "1"; // add 1 as the first number
				slidesFilePath = parsedFile[0] + "." + parsedFile[1]; // recombine the split strings to form the new filepath
			}
			else
			{
				int numValue = Int32.Parse(result); // convert the (string) number to an int
				numValue += 1; // increment the number by 1
				parsedFile[0] = parsedFile[0].TrimEnd(digits); // remove the number from the end
				parsedFile[0] += numValue.ToString(); // add the new number
				slidesFilePath = parsedFile[0] + "." + parsedFile[1];
			}

		}

		string titleLine = "Time,Slide";

		slideWriter = new StreamWriter(slidesFilePath, true);
		slideWriter.WriteLine(titleLine);
	}

	public void slidesDataWriter(int currSlideIndex)
	{
		slide = currSlideIndex;
		slideWriter.WriteLine(callBackTimeStamp2.ToString() + "," + currSlideIndex);
	}

	private void VisionDataWriter2(bool eyeEnabled)
	{
		RaycastHit hit;
		// string time = currentTime.ToString();
		string time = callBackTimeStamp2.ToString();
		if (!eyeEnabled)
		{
			writer.WriteLine(time + "," + "" + "," + "" + "," + "" + "," + "" + "," + ""); // write even if eye isnt found, but leave it all blank
			return; // don't need the rest
		}

		string eyeLook = GazeDirectionCombined.ToString();
		string hitPosition = "";
		string hitObjectName = "";
		string headRotation = Camera.main.transform.rotation.ToString();

		string cameraPosition = Camera.main.transform.position.ToString();

		if (Physics.Raycast(Camera.main.transform.position, GazeDirectionCombined, out hit))
		{
			/*if (hit.collider.gameObject.tag == "Screen") // if its the screen then update the heat map
			{
				objectwithheatmap.GetComponent<viveHeatMap>().updatePoint(hit.point.x, hit.point.y);
			}*/

			hitPosition = hit.point.ToString();
			hitObjectName = hit.collider.gameObject.name.ToString();

		}

		//string peripheryHitList = objectWithCone.GetComponent<viveVisionCone>().generateConeString(); // obtain all the objects in the peripheral vision from the cone

		writer.WriteLine(time + "," + quotifier(eyeLook) + "," + quotifier(cameraPosition) + "," + quotifier(headRotation) + "," + hitObjectName + "," + quotifier(hitPosition));
	}

}