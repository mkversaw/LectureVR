using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class sceneManager : MonoBehaviour
{
    public void exitScene(){ StartCoroutine(TimedExit()); }
	
	IEnumerator TimedExit(){
		
		yield return new WaitForSeconds(2);
		Application.Quit();
		
	}
}
