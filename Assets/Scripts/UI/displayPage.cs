using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class displayPage : MonoBehaviour
{
	
	private SpriteRenderer sprtrend;
	
	private float visible = 0f;
	private float changeSpeed = 0.1f;
	
    void Start(){
        sprtrend = this.GetComponent<SpriteRenderer>();
    }

	void Update(){
		
		visible = sprtrend.color.a;
		
	}

	private void ChangeColor(float delta){
		
		sprtrend.color = new Color(1f, 1f, 1f, sprtrend.color.a + delta);
		
	}

	public void ToggleVisibility(){
		
		if(visible > 0.5)
			StartCoroutine(FadeOut());
		
		else
			StartCoroutine(FadeIn());
		
	}
	
	public void TurnOff(){ StartCoroutine(FadeOut()); }
	
	public void TurnOn(){ StartCoroutine(FadeIn()); }

    IEnumerator FadeOut(){
		
		while(visible > 0){
			ChangeColor(-changeSpeed);
			yield return null;
		}
		
	}
	
	IEnumerator FadeIn(){
		
		while(visible < 1){
			ChangeColor(changeSpeed);
			yield return null;
		}
		
	}
}
