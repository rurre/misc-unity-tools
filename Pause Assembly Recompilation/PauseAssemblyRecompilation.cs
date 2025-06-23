#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

public class PauseAssemblyRecompilation : EditorWindow
{
	[MenuItem("Tools/Assembly Recompilation/Pause")]
    public static void PauseDatabase()
	{
		Debug.Log("Pausing Assembly Recompilation. Script changes will not trigger recompiles.");
		AssetDatabase.StartAssetEditing();
	}

	[MenuItem("Tools/Assembly Recompilation/Unpause")]
	public static void UnpauseDatabase()
	{
		Debug.Log("Unpausing Assembly Recompilation. Scripts can take forever to compile again!");
		AssetDatabase.StopAssetEditing();
	}
}
#endif