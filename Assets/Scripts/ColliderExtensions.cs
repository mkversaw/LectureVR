using UnityEngine;

public static class ColliderExtensions
{
    public static bool IsTriggerButton(this Collider col){
		return col.tag == "Button_Obj";
	}
}
