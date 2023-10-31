using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Fancy.Editor
{

public class CreateMonoStateData : ScriptableSingleton<CreateMonoStateData>
{
	public bool createMonoStates = false;
	[System.Serializable]
	public struct StateScriptData
	{
		public string className;
		public string monoScriptPath;
	}
	public List<StateScriptData> states;
}

}