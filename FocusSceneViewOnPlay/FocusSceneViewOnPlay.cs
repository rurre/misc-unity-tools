/// FocusSceneViewOnPlay.cs by Pumkin
/// https://github.com/rurre/misc-unity-tools
/// Automatically focuses scene when going into play mode, unless uploading an avatar to VRChat. Also moves the main camera to the scene camera once, when entering play mode.
/// Put anywhere in your project and it should just work(tm)

#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace Pumkin
{
	public class FocusSceneViewOnPlay
	{
		[InitializeOnEnterPlayMode]
		public static void FocusSceneView()
		{
			EditorApplication.delayCall += () =>
			{
				if(GameObject.Find("VRCSDK"))
					return;

				EditorWindow.FocusWindowIfItsOpen(typeof(SceneView));

				// If you want the camera to no longer snap to scene camera delete from here
				Camera cam = Camera.main;
				if(!cam)
					return;

				EditorApplication.delayCall += () =>
				{
					cam.nearClipPlane = 0.01f;
					Camera sceneCam = SceneView.lastActiveSceneView.camera; 
					Camera.main.transform.SetPositionAndRotation(sceneCam.transform.position, sceneCam.transform.rotation);					
				};
        // Delete until here
			};
		}
	}
}
#endif
