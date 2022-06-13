using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class viveTestScript : MonoBehaviour
{

	public class Objects
	{
		public Renderer[] Renderers;
		public Color[] startColors;
	}

	public GameObject hasLookLog;
	GameObject[] objs;
	[SerializeField] Objects[] objList;
	

	private string lastName = "blah";

	void Start()
	{
		objs = hasLookLog.GetComponent<viveLookLog>().objects;
		objList = new Objects[objs.Length];

		for(int i = 0; i < objs.Length; i++)
		{
			if(objs[i].GetComponentsInChildren<Renderer>().Length != 0)
			{
				objList[i] = new Objects();
				objList[i].Renderers = new Renderer[objs[i].GetComponentsInChildren<Renderer>().Length];
				objList[i].Renderers = objs[i].GetComponentsInChildren<Renderer>();
			} else
			{
				objList[i].Renderers = new Renderer[1];
				objList[i].Renderers[0] = objs[i].GetComponent<Renderer>();
			}

			
			objList[i].startColors = new Color[objList[i].Renderers.Length];
			for (int j = 0; j < objList[i].Renderers.Length; j++)
			{
				objList[i].startColors[j] = objList[i].Renderers[j].material.color;
			}
		}
		
	}

	public void Ping(string Name)
	{


		int index = -1;
		for(int i = 0; i < objs.Length; i++)
		{
			if(objs[i].gameObject.name == Name)
			{
				index = i;
				if (Name != lastName)
				{
					UnPing(lastName);
				} else
                {
					return;
                }
			}
		}
		if(index != -1)
		{
			for(int i = 0; i < objList[index].Renderers.Length; i++)
			{
				objList[index].Renderers[i].material.color = Color.yellow;
			}
			lastName = Name;
		}
	}

	public void UnPing(string Name)
	{
		int index = -1;
		for (int i = 0; i < objs.Length; i++)
		{
			if (objs[i].gameObject.name == Name)
			{
				index = i;
			}
		}
		if (index != -1)
		{
			for (int i = 0; i < objList[index].Renderers.Length; i++)
			{
				objList[index].Renderers[i].material.color = objList[index].startColors[i];
			}
		}
	}
}
