using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Fancy.Editor
{
public class MeshProccessor : EditorWindow
{
	[MenuItem("Window/Fancy/Mesh Processor")]
	public static void ShowWindow()
	{
		EditorWindow.GetWindow<MeshProccessor>("Mesh Processor");
	}
	
	Mesh[] meshes = new Mesh[0];
	void OnGUI()
	{
		if(meshes.Length == 0)
		{
			GUILayout.Label("No meshes selected");
			return;
		}

		for(int i = 0; i < meshes.Length; i++)
		{
			GUILayout.Label(meshes[i].name + " submeshes: " + meshes[i].subMeshCount);
		}
		
		GUILayout.BeginHorizontal();
		GUILayout.Label("Center mesh pivot:");
		if(GUILayout.Button("X")) {
			Undo.RecordObjects(meshes, "CenterX");
			CenterSelection(Vector3.right);
		}
		if(GUILayout.Button("Y")) {
			Undo.RecordObjects(meshes, "CenterY");
			CenterSelection(Vector3.up);
		}
		if(GUILayout.Button("Z")) {
			Undo.RecordObjects(meshes, "CenterZ");
			CenterSelection(Vector3.forward);
		}
		GUILayout.EndHorizontal();

		if(GUILayout.Button("Save"))
		{
			Save();
		}

		if(GUILayout.Button("Stitch Ani"))
		{
			for(int i = 0; i < meshes.Length; i++)
			{
				Debug.Log(meshes[i].name);
				StitchAni(meshes[i]);
			}
		}
	}

	void CenterSelection(Vector3 axisMask)
	{
		for(int i = 0; i < meshes.Length; i++)
		{
			Center(meshes[i], axisMask);
		}
	}

	void Center(Mesh m, Vector3 axisMask)
	{
		var bounds = m.bounds;
		Vector3 offset = Vector3.Scale(m.bounds.center, axisMask);
		var vertices = m.vertices;
		for(int i = 0; i < m.vertexCount; i++)
		{
			vertices[i] -= offset;
		}
		m.SetVertices(vertices);
		m.RecalculateBounds();
		AssetDatabase.GetAssetPath(m);
	}

	void OnSelectionChange()
	{
		meshes = Selection.GetFiltered<Mesh>(SelectionMode.Unfiltered);
		if(meshes.Length == 0)
		{
			var filters = Selection.GetFiltered<MeshFilter>(SelectionMode.Unfiltered);
			meshes = new Mesh[filters.Length];
			for(int i = 0; i < filters.Length; i++)
			{
				meshes[i] = filters[i].sharedMesh;
			}
		}
		Repaint();
	}

	public void StitchAni(Mesh mesh)
	{
		float epsilon = 0.0001f;
		var vertices = mesh.vertices;
		var uvs = mesh.uv;
		Debug.Assert(uvs.Length == vertices.Length, "UVs and vertices count mismatch");
		for(int i = 0; i < vertices.Length; i++)
		{
			for(int j = i + 1; j < vertices.Length; j++)
			{
				var v1 = vertices[i];
				var v2 = vertices[j];
				if( (v2-v1).magnitude <= epsilon && (uvs[i] - uvs[j]).magnitude >= epsilon)
				{
					Debug.Log("Pair: " + v1 + " : " + v2);
					Debug.Log(uvs[i] + " : " + uvs[j]);
					Vector2 uv = new Vector2(Mathf.Min(uvs[i].x, uvs[j].x), Mathf.Min(uvs[i].y, uvs[j].y));
					Debug.Log("UV: " + uv);
					uvs[i] = uv;
					uvs[j] = uv;
				}
			}
		}
		mesh.SetUVs(0, uvs);
	}

	void Save()
	{
		string path = EditorUtility.SaveFolderPanel("Where to save assets", "", "");
		MeshRenderer[] renderers = Selection.GetFiltered<MeshRenderer>(SelectionMode.Unfiltered);

		if (path.StartsWith(Application.dataPath)) {
			path = "Assets" + path.Substring(Application.dataPath.Length);
		}
		for(int i = 0; i < renderers.Length; i++)
		{
			//var go = Object.Instantiate<GameObject>(renderers[i].gameObject);
			var go = renderers[i].gameObject;
			Mesh mesh = go.GetComponent<MeshFilter>().sharedMesh;
			mesh.name = go.name;
			mesh = Object.Instantiate<Mesh>(mesh);
			mesh.name = go.name;
			go.GetComponent<MeshFilter>().sharedMesh = mesh;
			string p = AssetDatabase.GenerateUniqueAssetPath(path + "/" + go.name + ".prefab");
			PrefabUtility.SaveAsPrefabAssetAndConnect(go, p, InteractionMode.AutomatedAction);
			//AssetDatabase.CreateAsset(go, p);
			AssetDatabase.AddObjectToAsset(mesh, p);

			go = PrefabUtility.LoadPrefabContents(p);
			go.GetComponent<MeshFilter>().sharedMesh = mesh;
			PrefabUtility.SaveAsPrefabAsset(go, p);
			PrefabUtility.UnloadPrefabContents(go);
		}
		AssetDatabase.SaveAssets();
	}
}
}