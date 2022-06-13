// Alan Zucconi
// www.alanzucconi.com
using UnityEngine;
using System.Collections;
using System.IO;

public class viveHeatMap : MonoBehaviour
{
	public Vector4[] positions;
	public Vector4[] properties;
	public Material material;
	[SerializeField] private GameObject quad_pos;
	private Vector3 upperLeft;
	private Vector3 bottomRight;
	public float max_x;
	public float max_y;
	public float min_x;
	public float min_y;
	private float z;
	private int counter = 0;

	public int count = 200;

	void Start()
	{
		positions = new Vector4[count]; // the positions (x,y,z)
		properties = new Vector4[count]; // the properties (x = radius, y = intensity)

		getRange();
		makePosition();
	}

	/// <summary>
	/// Update the heatmap material with any changes made
	/// </summary>
	void Update()
	{
		material.SetInt("_Points_Length", count);
		material.SetVectorArray("_Points", positions);
		material.SetVectorArray("_Properties", properties);
	}

	/// <summary>
	/// Obtains the corners of the screen so we know the range to draw the heatmap on
	/// </summary>
	void getRange()
	{
		Bounds quad_bounds = quad_pos.GetComponent<Renderer>().bounds;
		max_x = quad_bounds.max.x;
		max_y = quad_bounds.max.y;
		min_x = quad_bounds.min.x;
		min_y = quad_bounds.min.y;
		z = quad_bounds.max.z;
	}

	/// <summary>
	/// Establish all the points that are going to exist on the heatmap, and fill them with the base values
	/// </summary>
	void makePosition()
	{
		for (float i = min_x; i < max_x; i += 0.15f)
		{
			for(float j = min_y; j < max_y; j += 0.15f)
			{
				positions[counter] = new Vector4(i,j,z, 0); 
				properties[counter] = new Vector4(0.15f, 0.3f); // 0.15 and 0.3 are optimal starting values for the radius and intensity
				counter++;
			}
		}
	}

	/// <summary>
	/// Given a specific point, finds the closest point to it in the heatmap array, then updates its heat value
	/// </summary>
	/// <param name="x"></param>
	/// <param name="y"></param>
	public void updatePoint(float x, float y)
    {
		// find the nearest neighbor (brute force)
		float min = 100f;
		int min_index = 100;
		for(int i = 0; i < counter; i++)
        {
			if(Dist(positions[i].x,x,positions[i].y,y) < min)
            {
				min = Dist(positions[i].x, x, positions[i].y, y);
				min_index = i;
			}
        }

		properties[min_index].y += 0.05f;
	}

	/// <summary>
	/// Get the distance between two points using the distance formula
	/// </summary>
	/// <param name="x1"></param>
	/// <param name="x2"></param>
	/// <param name="y1"></param>
	/// <param name="y2"></param>
	/// <returns></returns>
	float Dist(float x1, float x2, float y1, float y2)
	{
		return Mathf.Sqrt((x1 - x2) * (x1 - x2) +
					(y1 - y2) * (y1 - y2)
		);
	}
}