using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ProceduralGrid : MonoBehaviour {
	// Grids are "<16", "16-25", "25-35", "35-45", "45+"
	private const int NUM_COLUMNS = 5;

	public float columnPadding = 0.1f;
	public float squaresPadding = 0.02f;
	public float gridWidth = 60.0f;
	public int numSquaresPerRow = 15;
	public Font textFont;

	public float padding;
	public float meanAge, stdDevAge;
	public int numPeople;
	public MeshFilter gridMesh = null;
	// Use this for initialization
	private List<int> numPeoplePerAgeGroup;
//	private int peakNumberOfPeople;
//	private int maxAge = 100;

	private float columnWidth;
	private float squareWidth;

	void Start () {
		Random.InitState (42);
		numPeoplePerAgeGroup = new List<int> (NUM_COLUMNS);
		for (int i = 0; i < NUM_COLUMNS; i++) {
			numPeoplePerAgeGroup.Add (0);
		}
		InitPeople ();

		columnWidth = gridWidth / NUM_COLUMNS;
		float columnNonPaddingWidth = columnWidth - 2.0f * columnPadding;
		//float gridCellWidth = columnNonPaddingWidth / numSquaresPerRow;
		squareWidth = (columnNonPaddingWidth - 2.0f * squaresPadding * (numSquaresPerRow - 1)) / numSquaresPerRow;
		//DrawDebugLines ();
		GenerateMesh ();
		GenerateText ();
	}

	void InitPeople() {
//		peakNumberOfPeople = 0;
		for (int i = 0; i < numPeople; i++) {
			float randomNormal = GenerateRandomNormalValue (meanAge, stdDevAge);
			int roundedAge = (int)Mathf.Round (randomNormal);
			if (roundedAge > 0 && roundedAge < 16) {
				numPeoplePerAgeGroup [0]++;
			} else if (roundedAge < 25) {
				numPeoplePerAgeGroup [1]++;
			} else if (roundedAge < 35) {
				numPeoplePerAgeGroup [2]++;
			} else if (roundedAge < 45) {
				numPeoplePerAgeGroup [3]++;
			} else {
				numPeoplePerAgeGroup [4]++;
			}
		}
	}

	void DrawDebugLines() {
		// "<16"
		print(gridWidth);
		float lineXPos = -gridWidth / 2.0f;

		DrawLine(new Vector3(lineXPos, 0, 0), new Vector3(lineXPos, 100.0f, 0), new Color(1, 0, 0), new Color(1, 0, 0));
		DrawCellLines (lineXPos);
		lineXPos += columnWidth;
		DrawLine(new Vector3(lineXPos, 0, 0), new Vector3(lineXPos, 100.0f, 0), new Color(1, 0, 0), new Color(1, 0, 0));
		DrawCellLines (lineXPos);
		lineXPos += columnWidth;
		DrawLine(new Vector3(lineXPos, 0, 0), new Vector3(lineXPos, 100.0f, 0), new Color(1, 0, 0), new Color(1, 0, 0));
		DrawCellLines (lineXPos);
		lineXPos += columnWidth;
		DrawLine(new Vector3(lineXPos, 0, 0), new Vector3(lineXPos, 100.0f, 0), new Color(1, 0, 0), new Color(1, 0, 0));
		DrawCellLines (lineXPos);
		lineXPos += columnWidth;
		DrawLine(new Vector3(lineXPos, 0, 0), new Vector3(lineXPos, 100.0f, 0), new Color(1, 0, 0), new Color(1, 0, 0));
		DrawCellLines (lineXPos);
		lineXPos += columnWidth;
		DrawLine(new Vector3(lineXPos, 0, 0), new Vector3(lineXPos, 100.0f, 0), new Color(1, 0, 0), new Color(1, 0, 0));
	}

	void DrawCellLines(float columnStart) {
		float lineXPos = columnStart + columnPadding + squareWidth + squaresPadding;
		for (int i = 0; i < (numSquaresPerRow - 1); i++) {
			DrawLine(new Vector3(lineXPos, 0, 0), new Vector3(lineXPos, 100.0f, 0), new Color(0, 1, 0), new Color(0, 1, 0));
			lineXPos += 2.0f * squaresPadding + squareWidth;
		}

	}

	void DrawLine(Vector3 start, Vector3 end, Color startColor, Color endColor, float duration = 0.2f)
	{
		GameObject line = new GameObject ("DebugLine");
		line.transform.parent = this.transform;
		line.AddComponent<LineRenderer>();
		LineRenderer lr = line.GetComponent<LineRenderer>();
		lr.material = new Material(Shader.Find("Particles/Alpha Blended Premultiply"));
		lr.startColor = startColor;
		lr.endColor = endColor;
		lr.useWorldSpace = false;
		lr.startWidth = 0.1f;
		lr.endWidth = 0.1f;
		lr.SetPosition(0, start);
		lr.SetPosition(1, end);
		lr.transform.localPosition = Vector3.zero;
		lr.transform.localRotation = Quaternion.identity;
		//GameObject.Destroy(myLine, duration);
	}

	void GenerateText() {
		GameObject firstColumnText = new GameObject ("FirstColumnLabel");
		firstColumnText.transform.parent = this.transform;
			
		Text firstLabel = firstColumnText.AddComponent<Text> ();
		firstLabel.alignment = TextAnchor.MiddleCenter;
		firstLabel.text = "< 16 Years old";
		firstLabel.font = textFont;

		firstColumnText.transform.localPosition = new Vector3 (-gridWidth / 2.0f + columnWidth / 2.0f, -5.0f, 0.0f);
	}


	float GenerateRandomNormalValue(float mean, float stdDev) {
		float u1 = 1.0f - Random.value; //uniform(0,1] random doubles
		float u2 = 1.0f - Random.value;
		float randStdNormal = Mathf.Sqrt (-2.0f * Mathf.Log (u1)) * Mathf.Sin (2.0f * Mathf.PI * u2); //random normal(0,1)
		return mean + stdDev * randStdNormal;
	}
		
	int AddCellsForColumn(int startVertexIndex, int numPeople, float startPosX, List<Vector3> verts, List<int> tris) {
		int row = 0;
		int col = 0;
		int vertexIndex = startVertexIndex;
		for (int i = 0; i < numPeople; i++) {
			float cellXPos = col * (squareWidth + squaresPadding);
			float cellYPos = row * (squareWidth + squaresPadding);
			verts.Add (new Vector3 (startPosX + cellXPos, cellYPos));
			vertexIndex++;
			verts.Add (new Vector3 (startPosX + cellXPos, cellYPos + squareWidth));
			vertexIndex++;
			verts.Add (new Vector3 (startPosX + cellXPos + squareWidth, cellYPos));
			vertexIndex++;
			verts.Add (new Vector3 (startPosX + cellXPos + squareWidth, cellYPos + squareWidth));
			vertexIndex++;

			tris.Add (vertexIndex - 3);
			tris.Add (vertexIndex - 2);
			tris.Add (vertexIndex - 4);

			tris.Add (vertexIndex - 1);
			tris.Add (vertexIndex - 2);
			tris.Add (vertexIndex - 3);
			col++;
			if (col == numSquaresPerRow) {
				row++;
				col = 0;
			}
		}
		return vertexIndex;
	}

	void GenerateMesh() {
		float start_time = Time.time;

		List<Vector3> verts = new List<Vector3>();
		List<int> tris = new List<int>();
		List<Color> colors = new List<Color>();

		float posX = (-gridWidth / 2.0f) + columnPadding;
		int vertexIndex = 0;
		for (int i = 0; i < NUM_COLUMNS; i++) {
			vertexIndex = AddCellsForColumn (vertexIndex, numPeoplePerAgeGroup [i], posX, verts, tris);
			posX += columnWidth;
		}

		Vector3[] flattenedVertices = new Vector3[verts.Count];
		verts.CopyTo (flattenedVertices);

		Color[] flattenedColors = new Color[colors.Count];
		colors.CopyTo (flattenedColors);
		// Generate the mesh object.
		Mesh ret = new Mesh();
		ret.vertices = flattenedVertices;
		ret.triangles = tris.ToArray();
		ret.colors = flattenedColors;
		//ret.uv = uvs.ToArray();

		// Assign the mesh object and update it.
		ret.RecalculateBounds();
		ret.RecalculateNormals();
		gridMesh.mesh = ret;
	}


	// Update is called once per frame
	void Update () {
		
	}
}
