using UnityEditor;

namespace Fancy.Editor
{

public static class DomainReloader
{
	[MenuItem("РАЗРАБ ПОПРАВЬ!/Включить и выключить")]
	public static void RequestDomainReload()
	{
		EditorUtility.RequestScriptReload();
	}
}

}