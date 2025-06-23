#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

public class PauseAssetDatabase : EditorWindow
{
	[MenuItem("Tools/Asset Database/Pause")]
    public static void PauseDatabase()
	{
		Debug.Log("Pausing AssetDatabase. Assets won't be able to be imported until it's unpaused.");
		AssetDatabase.StartAssetEditing();
	}

	[MenuItem("Tools/Asset Database/Unpause")]
	public static void UnpauseDatabase()
	{
		Debug.Log("Unpausing AssetDatabase. Assets can be imported again!");
		AssetDatabase.StopAssetEditing();
	}
}
#endif