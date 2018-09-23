using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class Test : MonoBehaviour
{
	public void BuildSphere()
    {
        float rad = 0.25f;
        float gridRad = rad * 1.25f;
        int cells = 50;

        GetComponent<MeshFilter>().sharedMesh = MarchingCubesBasicPort.BuildUnityMesh(-Vector3.one* gridRad, 2 * gridRad, cells, (Vector3 pos) =>
        {
            return SDFOps.Sphere(pos, Vector3.zero, rad);
        });
    }

    public void BuildSpherePair()
    {
        float rad = 0.25f;
        float gridRad = rad * 1.25f;
        int cells = 50;

        GetComponent<MeshFilter>().sharedMesh = MarchingCubes.BuildUnityMesh(-Vector3.one * gridRad, 2 * gridRad, cells, (Vector3 pos) =>
        {
            return SDFOps.Union(SDFOps.Sphere(pos, new Vector3(0, 0, -rad * 0.5f), rad * 0.65f), SDFOps.Sphere(pos, new Vector3(0, 0, rad * 0.5f), rad * 0.65f));
        });
    }

    public void BuildBox()
    {
        Vector3 halfExt = new Vector3(0.5f, 0.4f, 0.25f);
        float gridRad = Mathf.Max(halfExt.x,halfExt.y,halfExt.z)*1.25f;
        int cells = 50;

        GetComponent<MeshFilter>().sharedMesh = MarchingCubes.BuildUnityMesh(-Vector3.one * gridRad, 2 * gridRad, cells, (Vector3 pos) =>
        {
            return SDFOps.Box(pos, Vector3.zero, halfExt);
        });
    }

    public void BuildDumbel()
    {
        float gridRad = 1;
        int cells = 50;

        GetComponent<MeshFilter>().sharedMesh = MarchingCubes.BuildUnityMesh(-Vector3.one * gridRad, 2 * gridRad, cells, (Vector3 pos) =>
        {
            float d = SDFOps.Box(pos, Vector3.zero, new Vector3(0.5f,0.05f, 0.05f));
            d = SDFOps.Blend(d, SDFOps.Sphere(pos, new Vector3(-0.5f, 0.0f, 0.0f), 0.25f), 0.2f);
            d = SDFOps.Blend(d, SDFOps.Sphere(pos, new Vector3(0.5f, 0.0f, 0.0f), 0.25f), 0.2f);
            return d;
        });
    }


}

#if UNITY_EDITOR
[CustomEditor(typeof(Test))]
public class TestEditor : Editor
{
    public override void OnInspectorGUI()
    {

        base.OnInspectorGUI();

        Test t = (Test)target;
        if (GUILayout.Button("Build Sphere"))
            t.BuildSphere();
        if (GUILayout.Button("Build Sphere Pair"))
            t.BuildSpherePair();
        if (GUILayout.Button("Build Box"))
            t.BuildBox();
        if (GUILayout.Button("Build Dumbell"))
            t.BuildDumbel();
    }
}
#endif