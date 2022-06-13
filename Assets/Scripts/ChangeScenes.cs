using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ChangeScenes : MonoBehaviour
{
    public void LoadScene (string destination){ StartCoroutine(TimedSceneChange(destination)); }
	
	IEnumerator TimedSceneChange(string destination){
		
		yield return new WaitForSeconds(2);
		SceneManager.LoadScene(destination);
		
	}
}
