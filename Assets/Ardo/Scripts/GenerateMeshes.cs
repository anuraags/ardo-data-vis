using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
public class GenerateMeshes : MonoBehaviour {

	MeshFilter _meshFilter; 
	public Material _material; 
	Matrix4x4[] _matrices; 

	// Use this for initialization
	void Start () {
		_meshFilter = GetComponent<MeshFilter> (); 	
		// We generate a single mesh for now 

		Mesh m = new Mesh (); 

		List<Vector3> verticies = new List<Vector3> (); 
		List<int> tris = new List<int> (); 
		verticies.Add (new Vector3 (0, 0));
		verticies.Add (new Vector3 (1, 0));
		verticies.Add (new Vector3 (0, 1));
		verticies.Add (new Vector3 (1, 1));
		tris.Add (0);
		tris.Add (1);
		tris.Add (3);
		tris.Add (0);
		tris.Add (3);
		tris.Add (2);
		m.SetVertices (verticies);
		m.SetIndices (tris.ToArray (), MeshTopology.Triangles, 0); 
		m.RecalculateNormals (); 
		_meshFilter.mesh = m; 


		_matrices = new Matrix4x4[100]; 
		for (int i = 0; i < _matrices.Length; i++) {
			var pos = new Vector3 (i * 1, i * 1); 
			_matrices [i] = Matrix4x4.TRS (pos, Quaternion.identity, Vector3.one); 
			//_matrices [i] = Matrix4x4.identity;
		}
		Debug.Log ("Len: " + _matrices.Length);

	}
	
	// Update is called once per frame
	void Update () {
		//Graphics.DrawMesh (_meshFilter.mesh, Matrix4x4.identity, _material, 0); 
		Graphics.DrawMeshInstanced (_meshFilter.mesh, 0, _material, _matrices); 
	}
}
