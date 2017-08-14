using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif


[RequireComponent(typeof(Camera)), ExecuteInEditMode]
public class ProjectionUVGenerator : MonoBehaviour
{
    public int width = 1920;
    public int height = 1080;
    public Rect cameraPixelRect = new Rect(0, 0, 1920, 1080);
    public MeshFilter targetObject;
    public int uvChannel = 1;
    public Material materialForEditor;

    new Camera camera { get { if (_c == null) _c = GetComponent<Camera>(); return _c; } }
    Camera _c;
    
    RenderTexture outputToSpout;


    // Update is called once per frame
    void Update()
    {
        camera.pixelRect = cameraPixelRect;
    }

    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        if (outputToSpout == null || outputToSpout.width != width || outputToSpout.height != height)
            outputToSpout = new RenderTexture(source.width, source.height, 0);
        Graphics.Blit(source, outputToSpout);
        Graphics.Blit(source, destination);
    }

#if UNITY_EDITOR
    void GenerateUV()
    {
        var targetMesh = targetObject.sharedMesh;
        var vertices = targetMesh.vertices.Select(v =>
        {
            var point = targetObject.transform.TransformPoint(v);
            return point;
        }).ToArray();
        var uvList = vertices.Select(v => camera.WorldToViewportPoint(v)).ToList();
        var newMesh = Instantiate(targetMesh);
        newMesh.SetUVs(uvChannel, uvList);

        var path = AssetDatabase.GetAssetPath(targetMesh);
        var directoryPath = System.IO.Path.GetDirectoryName(path);
        if (!AssetDatabase.IsValidFolder(directoryPath))
            directoryPath = "Assets";
        path = System.IO.Path.Combine(directoryPath, targetMesh.name + string.Format("_ProjectedUV{0}.asset", uvChannel));
		AssetDatabase.CreateAsset(newMesh, path);
		AssetDatabase.SaveAssets ();
		AssetDatabase.Refresh();

        Selection.activeObject = newMesh;

        var go = new GameObject(name + " UV Editor");
        go.AddComponent<MeshFilter>().sharedMesh = newMesh;
        go.AddComponent<MeshRenderer>().sharedMaterial = materialForEditor;

        var editor = go.AddComponent<ProjectionUVEditor>();
        editor.projectionCam = camera;
		editor.uvChannel = uvChannel;
		newMesh.GetUVs(uvChannel, editor.editUVs);

    }

    [MenuItem("Custom/Projection/Generate UV")]
    public static void GenerateUVMenu()
    {
        var generator = Selection.activeGameObject.GetComponent<ProjectionUVGenerator>();
        generator.GenerateUV();
    }
#endif
}
