using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ProceduralGeneration : MonoBehaviour
{

	public int width = 10;
	public float spacing = 1f;
	public float maxHeight = 3f;
	public MeshFilter terrainMesh = null;
	public float mean = 50.0f;
	public float stdDev = 10.0f;

	void Start ()
	{
		List<float> ages = new List<float> ();
		for (int i = 0; i < 100; i++) {
			ages.Add (0.0f);
		}
		Random.InitState (42);
		for (int i = 0; i < 5000; i++) {
			float u1 = 1.0f - Random.value; //uniform(0,1] random doubles
			float u2 = 1.0f - Random.value;
			float randStdNormal = Mathf.Sqrt (-2.0f * Mathf.Log (u1)) * Mathf.Sin (2.0f * Mathf.PI * u2); //random normal(0,1)
			float randNormal = mean + stdDev * randStdNormal;
			int ageIndex = (int) Mathf.Round (randNormal);
			if (ageIndex >= 0 && ageIndex < 100) {
				//ages [ageIndex]++;
				GameObject go = GameObject.CreatePrimitive(PrimitiveType.Cube);
				go.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
				go.transform.SetParent(this.transform);
				go.transform.localPosition = new Vector3(ageIndex - 50, ages[ageIndex] / 4, 0);
				ages [ageIndex] += 0.25f;
			}
		}
		for(int x=0; x < 100; x++){

		}

//		if (terrainMesh == null)
//		{
//			Debug.LogError("ProceduralTerrain requires its target terrainMesh to be assigned.");
//		}
//
//		//GenerateMesh();
//		for(int x=0; x < 5000; x++){
//			GameObject go = GameObject.CreatePrimitive(PrimitiveType.Cube);
//			go.transform.localScale = new Vector3(1.0f, 2.0f, 0.01f);
//			go.transform.SetParent(this.transform);
//			go.transform.localPosition = new Vector3(x*2, 0, 0);
//		}
	}

	void GenerateMesh ()
	{
		float start_time = Time.time;

		List<Vector3[]> verts = new List<Vector3[]>();
		List<int> tris = new List<int>();
		List<Vector2> uvs = new List<Vector2>();

		// Generate everything.
		for (int z = 0; z < width; z++)
		{
			verts.Add(new Vector3[width]);
			for (int x = 0; x < width; x++)
			{
				Vector3 current_point = new Vector3();
				current_point.x = (x * spacing);
				current_point.z = z * spacing;

				current_point.y = GetHeight(current_point.x, current_point.z);

				verts[z][x] = current_point;
				uvs.Add(new Vector2(x,z)); // TODO Add a variable to scale UVs.

				// TODO The edges of the grid aren't right here, but as long as we're not wrapping back and making underside faces it should be okay.

				// Don't generate a triangle if it would be out of bounds.
				if (x-1 <= 0 || z <= 0 || x >= width)
				{
					continue;
				}
				// Generate the triangle north of you.
				tris.Add(x + z*width);
				tris.Add(x + (z-1)*width);
				tris.Add((x-1) + (z-1)*width);

				// TODO Generate the triangle northwest of you.
			}
		}

		// Only generate one triangle.
		// TODO Generate a grid of triangles.
		tris.Add(0);
		tris.Add(1);
		tris.Add(width);

		// Unfold the 2d array of verticies into a 1d array.
		Vector3[] unfolded_verts = new Vector3[width*width];
		int i = 0;
		foreach (Vector3[] v in verts)
		{
			v.CopyTo(unfolded_verts, i * width);
			i++;
		}

		// Generate the mesh object.
		Mesh ret = new Mesh();
		ret.vertices = unfolded_verts;
		ret.triangles = tris.ToArray();
		ret.uv = uvs.ToArray();

		// Assign the mesh object and update it.
		ret.RecalculateBounds();
		ret.RecalculateNormals();
		terrainMesh.mesh = ret;

		float diff = Time.time - start_time;
		Debug.Log("ProceduralTerrain was generated in " + diff + " seconds.");
	}

	// Return the terrain height at the given coordinates.
	// TODO Currently it only makes a single peak of max_height at the center,
	// we should replace it with something fancy like multi-layered perlin noise sampling.
	float GetHeight (float x_coor, float z_coor)
	{
		float y_coor = Mathf.Min (0, maxHeight - Vector2.Distance (Vector2.zero, new Vector2 (x_coor, z_coor)));
		return y_coor;
	}
}
