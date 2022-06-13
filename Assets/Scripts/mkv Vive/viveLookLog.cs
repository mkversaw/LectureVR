using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class viveLookLog : MonoBehaviour
{
	[System.Serializable]
	public class obj_and_counter
	{
		public int hit_counter = 0;
		public GameObject obj = null;
	}


	public GameObject[] objects;

	[SerializeField] obj_and_counter[] objectHits;

	//[SerializeField] GameObject objectwithhighlight;

	private string lastName = "blah";

	void Start()
	{
		objectHits = new obj_and_counter[objects.Length]; // update the size of the objectHits array on start
		for(int i = 0; i < objects.Length; i++)
		{
			objectHits[i] = new obj_and_counter();
			objectHits[i].obj = objects[i];
        }
    }

	public void updateList(RaycastHit hit, string name) // gets passed a gameobject.name (string)
	{
		if(name == lastName) // ignore immediate repeats
        {
			return; 
        }

		for(int i = 0; i < objects.Length; i++)
		{
			if(name == objects[i].gameObject.name) // if name of object that the ray hit is in the list of objects were keeping track of
			{
				objectHits[i].hit_counter += 1; // then increment its counter by 1
				lastName = name;
			}
		}
		// objectwithhighlight.GetComponent<viveTestScript>().Ping(name);
	}
}
