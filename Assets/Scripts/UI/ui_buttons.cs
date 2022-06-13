using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Valve.VR;

public class ui_buttons : MonoBehaviour
{

	[SerializeField] private UnityEvent onButtonPressed;
	
	[Space(10)]
	
	[Tooltip("0 should be the right hand, 1 should be the left")]
	[SerializeField] private Transform[] InteractableObjects;
	
	[Space(10)]
	
	[Range(0.1f, 2f)]
	[SerializeField] private float pressRad = 0.15f;
	
	[Space(10)]
	
	[SerializeField] private Sprite defaultSprite;
	[SerializeField] private Sprite hoverSprite;
	[SerializeField] private Color clickColor;
	
	[Space(10)]
	[SerializeField] private bool buttonOn = true;



	private SpriteRenderer sprtrend;

	[SerializeField]
	private SteamVR_Action_Vibration hapticAction;
	

	private Vector3 center;
	
	private bool toggled = false;
	private bool pressing;
	
	void Start(){
		
		sprtrend = this.GetComponent<SpriteRenderer>();
		
		center = this.transform.position;
		
	}
	
	void Update(){
		center = this.transform.position; // temp?
		if (buttonOn){
			// whether the right or left hands are present
			bool RtPresent = false;
			bool LfPresent = false;
			
			// whether the objects are in range
			bool objectsInRange = false;
			
			for(int i = 0; i < InteractableObjects.Length; i++){
				
				if(Vector3.Distance(center, InteractableObjects[i].position) < pressRad){
					objectsInRange = true;
					
					if(i == 0)
						RtPresent = true;
					if(i == 1)
						LfPresent = true;
				
				}
				
			}
			
			// handle button presses
			if(objectsInRange && !pressing){
				pressing = true;
				onButtonPressed?.Invoke();
				
				toggled = !toggled;
				
				StartCoroutine(ResetButton());

				// do the controller vibration
				if (RtPresent)
				{
					//StartCoroutine(VibrateController(OVRInput.Controller.RTouch));
					StartCoroutine(VibrateControllerVive(SteamVR_Input_Sources.RightHand));
				}

				if (LfPresent)
				{
					//StartCoroutine(VibrateController(OVRInput.Controller.LTouch));
					StartCoroutine(VibrateControllerVive(SteamVR_Input_Sources.LeftHand));
				}
				
				
			} else if(!objectsInRange && pressing){
				pressing = false;
				
			}
			
			// handle changing sprites
			if(toggled && sprtrend.sprite != hoverSprite){
				sprtrend.sprite = hoverSprite;
				
			} else if(!toggled && sprtrend.sprite != defaultSprite){
				sprtrend.sprite = defaultSprite;
				
			}
		}
	}
	
	public void SetDefaultState(){
		toggled = false;
	}
	
	public void TurnOn(){
		buttonOn = true;
	}
	
	public void TurnOff(){
		buttonOn = false;
	}
	
	IEnumerator ResetButton(){
		
		yield return new WaitForSecondsRealtime(1);
		
		pressing = false;

		
	}
	
	/*IEnumerator VibrateController(OVRInput.Controller c){ // This is for the Oculus
		
		OVRInput.SetControllerVibration(1f, 1f, c);
		
		yield return new WaitForSecondsRealtime(0.2f);
		
		OVRInput.SetControllerVibration(0, 0, c);
		
	}*/

	IEnumerator VibrateControllerVive(SteamVR_Input_Sources source) // This is for the Vive
    {
		hapticAction.Execute(0f, 0.1f, 30f, 1f, source); // (when to start the vibration, duration of vibration, frequency of vibration, strength of vibration, which controller to vibrate)

		yield return new WaitForSecondsRealtime(0.2f);
	}

}

