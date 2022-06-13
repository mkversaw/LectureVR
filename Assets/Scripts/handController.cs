using System.Collections;
using System.Collections.Generic;
using UnityEngine;

enum hand{ Left, Right};


public class handController : MonoBehaviour
{
	
	[SerializeField] private Animator anim;
	
	[SerializeField] private hand side;

    // Update is called once per frame
    void Update(){
        AnimateHand();
    }
	
	void AnimateHand(){
		
		if(side == hand.Left){
			anim.SetFloat("thumb", (float)System.Convert.ToDouble(OVRInput.Get(OVRInput.RawTouch.X)));
			anim.SetFloat("index", OVRInput.Get(OVRInput.RawAxis1D.LIndexTrigger));
			anim.SetFloat("others", OVRInput.Get(OVRInput.RawAxis1D.LHandTrigger));
		} else{
			anim.SetFloat("thumb", (float)System.Convert.ToDouble(OVRInput.Get(OVRInput.RawTouch.A)));
			anim.SetFloat("index", OVRInput.Get(OVRInput.RawAxis1D.RIndexTrigger));
			anim.SetFloat("others", OVRInput.Get(OVRInput.RawAxis1D.RHandTrigger));
		}
	}
}
