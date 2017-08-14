using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ProjectionUVEditor : MonoBehaviour {

    public Camera projectionCam;
    public int uvChannel = 1;
    
    public EditorProperty editorProp;
	public List<Vector2> editUVs;
	public bool edit;

    Vector2 preTouch;
    List<Vector2> defaultUVs;

    Mesh mesh;

    // Use this for initialization
    void Start ()
    {
        defaultUVs = new List<Vector2>();
        mesh = GetComponent<MeshFilter>().mesh;
        var renderer = GetComponent<Renderer>();

		mesh.GetUVs(uvChannel, defaultUVs);
		if (editUVs.Count != defaultUVs.Count)
			mesh.GetUVs (uvChannel, editUVs);
		mesh.SetUVs(uvChannel, editUVs);
		renderer.material.SetFloat ("_Sel", uvChannel);
	}

	void OnDestroy(){
		#if UNITY_EDITOR
		if(edit)
		{
			UnityEditor.AssetDatabase.SaveAssets();
			UnityEditor.AssetDatabase.Refresh();
		}
		#endif
	}
	
	// Update is called once per frame
	void Update () {
		if (!edit)
			return;
        var pos = (Vector2)Input.mousePosition;
        var touching = (Vector2)projectionCam.ScreenToViewportPoint(pos);
        if (Input.GetMouseButtonDown(0))
        {
            preTouch = touching;
            return;
        }
        if(Input.GetMouseButton(0))
        {
            var drag = touching - preTouch;
            preTouch = touching;
            
            mesh.GetUVs(uvChannel, editUVs);
            for (var i = 0; i < editUVs.Count; i++)
            {
                var uv = editUVs[i];
                var t = Mathf.Clamp01(1f - Vector2.Distance(uv, touching) / editorProp.radius);

                editUVs[i] = uv + drag * t;
            }
            mesh.SetUVs(uvChannel, editUVs);
        }
	}

    void ResetUVs()
    {
        mesh.SetUVs(uvChannel, defaultUVs);
    }

    [System.Serializable]
    public class EditorProperty
    {
        public float radius = 0.05f;
        public int editorCurveIdx;
        public AnimationCurve[] curves;

        public float GetCurveVal(float t)
        {
            if (curves.Length < 1) return t;
            var c = curves[editorCurveIdx % curves.Length];
            return c.Evaluate(t);
        }
    }
}
