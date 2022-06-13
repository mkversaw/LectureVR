using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlaySlides : MonoBehaviour
{
	
	[SerializeField] private UnityEvent onButtonPressed;
	
	SpriteRenderer spriteRenderer;
	
	public AudioSource speaker;
	public Animator animator;
	public GameObject eyeTrackREF;

	[HideInInspector] public List<SlideOrder> slides = new List<SlideOrder>();
	
	void Start(){
		spriteRenderer = this.GetComponent<SpriteRenderer>();
		StartCoroutine(Sequence());
	}
	
    void ChangeSprite(int x){
		spriteRenderer.sprite = slides[x].slide;
	}
	
	// the old, flawed way I was doing animation changes
	// kept for posterity
	/*IEnumerator Sequence(){
		
		// Makes an animator override controller so we can feed in our animation clips
		AnimatorOverrideController animatorOverrideController = new AnimatorOverrideController();
		animatorOverrideController.runtimeAnimatorController = animator.runtimeAnimatorController;
		
		animatorOverrideController["Nothing"] = slides[0].attachedAnim;
		animator.runtimeAnimatorController = animatorOverrideController;
		
		for (int curSlide = 0; curSlide < slides.Count; curSlide++){
			
			ChangeSprite(curSlide);
			
			speaker.clip = slides[curSlide].attachedAudio;
			speaker.Play();
			
			// Replaces the dummy animation in our Animator Controller with whatever this slide's
			// animation will be
			animatorOverrideController["Replace"] = slides[curSlide].attachedAnim;
			animator.runtimeAnimatorController = animatorOverrideController;
			animator.SetTrigger("PlayTrigger");
			
			Debug.Log("set trigger");
			
			yield return new WaitForSeconds(slides[curSlide].attachedAudio.length + 0.5f);
			
		}
	}*/
	
	IEnumerator Sequence(){
		
		// Makes an animator override controller so we can feed in our animation clips
		AnimatorOverrideController animatorOverrideController = new AnimatorOverrideController();
		animatorOverrideController.runtimeAnimatorController = animator.runtimeAnimatorController;
		
		animatorOverrideController["Buffer_1"] = slides[0].attachedAnim;
		animatorOverrideController["Buffer_2"] = slides[1].attachedAnim;
		animator.runtimeAnimatorController = animatorOverrideController;
		
		for (int curSlide = 0; curSlide < slides.Count; curSlide++){
			print("curSlide: " + (curSlide + 1));
			eyeTrackREF.GetComponent<viveEye120HZ>().slidesDataWriter(curSlide + 1);


			// updates the slide
			ChangeSprite(curSlide);
			
			// updates the audio
			speaker.clip = slides[curSlide].attachedAudio;
			speaker.Play();
			
			// updates the animation
			if(curSlide > 0)
				animator.SetTrigger("PlayTrigger");
			
			yield return new WaitForSeconds(1);
			
			// this next bit swaps out the animation 2 slides in front of the current one
			// so we can blend between the current and next slide.
			if((curSlide + 2) < slides.Count){
				if((curSlide + 1) % 3 == 1){			// when buffer 1 is in use
					animatorOverrideController["Buffer_3"] = slides[curSlide + 2].attachedAnim;
					animator.runtimeAnimatorController = animatorOverrideController;
					//Debug.Log("update buffer 3");
					
				} else if((curSlide + 1) % 3 == 2){		// when buffer 2 is in use
					animatorOverrideController["Buffer_1"] = slides[curSlide + 2].attachedAnim;
					animator.runtimeAnimatorController = animatorOverrideController;
					//Debug.Log("update buffer 1");
					
				} else if((curSlide + 1) % 3 == 0){		// when buffer 3 is in use
					animatorOverrideController["Buffer_2"] = slides[curSlide + 2].attachedAnim;
					animator.runtimeAnimatorController = animatorOverrideController;
					//Debug.Log("update buffer 2");
					
				}
			}
			
			yield return new WaitForSeconds(slides[curSlide].attachedAudio.length - 0.5f);
			
		}
		
		// exits the scene after all the bits have played
		//Debug.Log("It's over, Anakin");
		onButtonPressed?.Invoke();
	}
}