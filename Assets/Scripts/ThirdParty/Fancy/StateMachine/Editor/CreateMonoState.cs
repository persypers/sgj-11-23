using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Fancy.Editor
{

class CreateMonoState : EditorWindow {
	const string templateFileName = "MonoStateTemplate.cs.txt";
	const string persistentDataName = "CreateMonoStateData.asset";
	const string templateNamespace = "##NAMESPACE##";
	const string templateClassName = "##CLASS##";
	const string templateParentClassName = "##PARENTCLASS##";

	[MenuItem ("Window/Fancy/Create MonoState")]
	public static void ShowWindow() {
		EditorWindow.GetWindow(typeof(CreateMonoState));
	}

	public string[] states = {"NewState"};
	private string scriptOutputFolder;
	private string stateNamespace;

	private List<Type> derivedTypes = null;
	private string[] derivedTypeNames = null;
	private int derivedTypeIndex = 0;
	// this is needed for 'states' array field drawer
	private SerializedObject so;

	private void OnEnable()
	{
		so = new SerializedObject(this);

		derivedTypes = AppDomain.CurrentDomain.GetAssemblies()
			.SelectMany(assembly => assembly.GetTypes())
			.Where(type => type.IsSubclassOf(typeof(MonoStateMachine)) && !type.IsGenericType).ToList();
		derivedTypeNames = derivedTypes.Select(type => type.ToString()).ToArray();
		scriptOutputFolder = GetDerivedClassPath(derivedTypeIndex);
	}

	private void OnGUI()
	{
		{	// draw states array
			so.Update();
			SerializedProperty stringsProperty = so.FindProperty("states");
			EditorGUILayout.PropertyField(stringsProperty, true);
			so.ApplyModifiedProperties();
		}

		stateNamespace = EditorGUILayout.TextField("Namespace", stateNamespace);

		int index = EditorGUILayout.IntPopup("Base class", derivedTypeIndex, derivedTypeNames, null);
		if(index != derivedTypeIndex)
		{
			derivedTypeIndex = index;
			scriptOutputFolder = GetDerivedClassPath(index);
		}

		scriptOutputFolder = EditorGUILayout.TextField("Script output folder", scriptOutputFolder);

		if(GUILayout.Button("Create states"))
		{
			CreateStates();
		}

	}

	private string GetDerivedClassPath(int derivedClassIndex)
	{
		// Couldn't find a better way to get MonoScript from corresponding System.Type =(
		Type type = derivedTypes[derivedClassIndex];
		string fullClassName = derivedTypeNames[derivedClassIndex];
		string className = fullClassName.Substring(fullClassName.LastIndexOf('.') + 1);
		var guids = AssetDatabase.FindAssets("t:MonoScript " + className);
		string derivedTypeGuid = guids.First(guid => {
			var mono = AssetDatabase.LoadAssetAtPath<MonoScript>(AssetDatabase.GUIDToAssetPath(guid));
			return mono.GetClass() == type;
		});
		return Path.GetDirectoryName(AssetDatabase.GUIDToAssetPath(derivedTypeGuid));
	}

	private void CreateStates()
	{
		var windowScript = MonoScript.FromScriptableObject(this);
		string thisPath = AssetDatabase.GetAssetPath(windowScript);
		thisPath = Path.GetFullPath(thisPath);
		thisPath = Path.GetDirectoryName(thisPath);

		string file = File.ReadAllText(Path.Combine(thisPath, templateFileName));

		var data = CreateMonoStateData.instance;
		data.states = new List<CreateMonoStateData.StateScriptData>();

		for(int i = 0; i < states.Length; i++)
		{
			string className = states[i];
			var scriptData = new CreateMonoStateData.StateScriptData();
			scriptData.className = className;
			scriptData.monoScriptPath = CreateScriptFile(file, className, stateNamespace, derivedTypeNames[derivedTypeIndex]);
			data.states.Add(scriptData);
		}

		data.createMonoStates = true;
		AssetDatabase.Refresh();
	}

	private string CreateScriptFile(
		string fileTemplate,
		string className,
		string namespaceName,
		string parentClassName
	){
		fileTemplate = fileTemplate.Replace(templateNamespace, namespaceName);
		fileTemplate = fileTemplate.Replace(templateClassName, className);
		fileTemplate = fileTemplate.Replace(templateParentClassName, parentClassName);
		
		string filename = className + ".cs";
		string path = Path.GetFullPath(scriptOutputFolder);
		path = Path.Combine(path, filename);
		File.WriteAllText(path, fileTemplate);
		//var monoScript = AssetDatabase.LoadAssetAtPath<MonoScript>(Path.Combine(scriptOutputFolder, filename));
		return Path.Combine(scriptOutputFolder, filename);
	}

	[UnityEditor.Callbacks.DidReloadScripts]
	private static void CreateAssetWhenReady()
	{
		var data = CreateMonoStateData.instance;

		try{
			if(data.createMonoStates)
			{
				data.createMonoStates = false;
				Debug.Log("Creating new MonoState gameobjects...");
				for(int i = 0; i < data.states.Count; i++)
				{
					Debug.Log(data.states[i].className);
					MonoScript mono = AssetDatabase.LoadAssetAtPath<MonoScript>(data.states[i].monoScriptPath);
					GameObject go = new GameObject(data.states[i].className);
					go.AddComponent(mono.GetClass());
				}
				EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
			}
		} finally
		{
			if(data != null) data.createMonoStates = false;
		}
	}
}

}
