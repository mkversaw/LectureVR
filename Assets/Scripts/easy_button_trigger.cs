using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class easy_button_trigger : MonoBehaviour
{
    [SerializeField] private Animator buttonAnimator;
	
	[SerializeField] private UnityEvent onButtonPressed;
	
	[SerializeField] private Transform[] InteractableObjects;
	
	[SerializeField] private Transform center;
	[SerializeField] private float rad = 0.15f;
	
	private bool pressing = false;
	
	private float waitTime = 24;
	private float curTime = 0;
	
	void Update(){
		
		if(curTime >= 0)
			curTime--;
		
		bool objectsInRange = false;
		
		foreach(var obj in InteractableObjects){
			
			if(Vector3.Distance(center.position, obj.position) < 0.15f){
				objectsInRange = true;
			}
			
		}
		
		if(!pressing && objectsInRange && curTime <= 0){
			Debug.Log("ButtonReleased - " + pressing);
			buttonAnimator.SetTrigger("ButtonPressed");
			onButtonPressed?.Invoke();
			pressing = true;
			
		}else if(pressing && !objectsInRange){
			Debug.Log("ButtonReleased - " + pressing);
			buttonAnimator.SetTrigger("ButtonReleased");
			pressing = false;
			curTime = waitTime;
		}
	}
}
