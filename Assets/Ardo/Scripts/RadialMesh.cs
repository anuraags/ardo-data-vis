using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ring{
	public Mesh Mesh; 
	public Matrix4x4[] Matrices; 
	public Vector4[] Colors; 
	public float RotationOffset;
	public float RotationDirection; 

	float _radius; 
	int _units; 

	public Ring(float radius, float height, float arcWidth, int units, float rotationDirection){
		_radius = radius; 
		_units = units; 
		RotationDirection = rotationDirection; 
		Mesh = GenerateRadialSquare (radius, height, arcWidth); 
	}

	public void Update(){
		UpdateRingTopology (_radius, _units, RotationOffset); 
	}

	void SetRingColors(){
		for (int i = 0; i < Colors.Length; i++) {
			Colors [i].x = Random.Range (0.0f, 1.0f); 
			Colors [i].y = Random.Range (0.0f, 1.0f); 
			Colors [i].z = Random.Range (0.0f, 1.0f); 
		}
	}


	void UpdateRingTopology(float radius, int units, float rotationOffset){

		if (Matrices == null || Matrices.Length != units) {
			Matrices = new Matrix4x4[units]; 
			Colors = new Vector4[units];

			for (int i = 0; i < Matrices.Length; i++) {
				Matrices [i] = Matrix4x4.identity; 
				Colors [i] = new Vector4 (1, (i%2==0)?0.0f:1.0f, 1, 1); 
			}
		}

		float thetaDelta = (2 * Mathf.PI) /units;

		for (int i = 0; i < Matrices.Length; i++) {
			var theta = thetaDelta * i + rotationOffset; 
			var x = radius * Mathf.Cos (theta); 
			var y = radius * Mathf.Sin (theta); 
			var pos = new Vector3 (x,y); 
			var rotation = Quaternion.Euler (0, 0, (theta / Mathf.PI) * 180 - 90); 
			Matrices [i].SetTRS (pos, rotation, Vector3.one); 
		}
	}

	static Mesh GenerateRadialSquare(float centreRadius, float height, float arcTheta){

		float outerRadius = centreRadius + height / 2.0f; 
		float innerRadius = centreRadius - height / 2.0f; 
		float outerChordLength = ChordLength (outerRadius, arcTheta); 
		float innerChordLength = ChordLength (innerRadius, arcTheta); 

		Mesh m = new Mesh (); 

		List<Vector3> verticies = new List<Vector3> (); 
		List<int> tris = new List<int> (); 
		verticies.Add (new Vector3 (-innerChordLength/2.0f, -height/2.0f));
		verticies.Add (new Vector3 (innerChordLength/2.0f, -height/2.0f));
		verticies.Add (new Vector3 (outerChordLength/2.0f, height/2.0f));
		verticies.Add (new Vector3 (-outerChordLength/2.0f, height/2.0f));
		tris.Add (0);
		tris.Add (3);
		tris.Add (2);
		tris.Add (0);
		tris.Add (2);
		tris.Add (1);
		m.SetVertices (verticies);
		m.SetIndices (tris.ToArray (), MeshTopology.Triangles, 0); 
		m.RecalculateNormals ();
		return m; 
	}

	static float ChordLength(float radius, float theta){
		return 2 * radius * Mathf.Sin (theta / 2.0f); 
	}

}

public class RadialMesh : MonoBehaviour
{
	public Material _material; 
	public float _animationSpeed = 0.1f; 

	Ring[] _rings; 

	private MaterialPropertyBlock block;
	private int colorID;

	void Start(){
		block = new MaterialPropertyBlock();
		colorID = Shader.PropertyToID("_Color");
		_rings = new Ring[100]; 
		for (int i = 0; i < _rings.Length; i++) {
			_rings [i] = new Ring (5 + i*2, 1.8f, Mathf.PI/30.0f, 10,Random.Range(-1.0f,1.0f)); 
		}

	}

	void FixedUpdate(){
		var animationSpeed = _animationSpeed * Mathf.Sin (Time.fixedTime); 
		for (int i = 0; i < _rings.Length; i++) {
			_rings[i].RotationOffset += _rings[i].RotationDirection * animationSpeed * Time.fixedDeltaTime; 
		}
	}

	void Update(){
		for (int i = 0; i < _rings.Length; i++) {
			var ring = _rings [i]; 
			ring.Update (); 
			block.SetVectorArray (colorID, ring.Colors); 
			Graphics.DrawMeshInstanced (ring.Mesh, 0, _material, ring.Matrices,ring.Matrices.Length,block); 
		}
	}		

}

