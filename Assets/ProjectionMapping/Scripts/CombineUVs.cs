using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombineUVs : MonoBehaviour {
	public ProjectionUVEditor[] uvEditors;
	public Mesh combined;
	// Use this for initialization
	void Start () {
		combined = uvEditors [0].GetComponent<MeshFilter> ().mesh;
		foreach (var e in uvEditors) 
			combined.SetUVs (e.uvChannel, e.editUVs);
		
	}
}
