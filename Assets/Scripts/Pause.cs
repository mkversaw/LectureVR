using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pause : MonoBehaviour
{
	
	public static bool isPaused = false;

    private void Start()
    {
		PauseGame();
    }

    public void PauseGame(){
		
		Time.timeScale = 0;
		AudioListener.pause = true;
		
	}
	
	public void ResumeGame(){
		
		Time.timeScale = 1;
		AudioListener.pause = false;
		
	}
	
	public void TogglePause(){
		
		//Debug.Log("TogglePause");
		
		if(Time.timeScale == 0)
			ResumeGame();
		
		else
			PauseGame();
		
	}
}
