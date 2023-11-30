/// FocusSceneViewOnPlay.cs by Pumkin
/// https://github.com/rurre/misc-unity-tools
/// Automatically focuses scene when going into play mode, unless uploading an avatar to VRChat. Also moves the main camera to the scene camera once, when entering play mode.
/// Put anywhere in your project and it should just work(tm)
/// Checkboxes added by nullstalgia, Oct/Nov 2023

#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using SceneView = UnityEditor.SceneView;

namespace Pumkin
{
	public class FocusSceneViewOnPlay
	{
		public static bool focusSceneOnPlayCheckbox 
		{
			get { return EditorPrefs.GetBool("Pumkin_FocusOnPlay", true); }
			set { EditorPrefs.SetBool("Pumkin_FocusOnPlay", value); }
		}

		[MenuItem("Tools/Pumkin/Focus Scene On Play", true)]
		public static bool ToggleSceneFocusOnPlayValidation()
		{
			// Toggle action validation
			Menu.SetChecked("Tools/Pumkin/Focus Scene On Play", focusSceneOnPlayCheckbox);
			return true;
		}

		[MenuItem("Tools/Pumkin/Focus Scene On Play")]
		public static void ToggleSceneFocusOnPlay()
		{
			// Toggle the boolean variable
			focusSceneOnPlayCheckbox = !focusSceneOnPlayCheckbox;
		}
        
		public static bool moveCameraOnPlayCheckbox
		{
			get { return EditorPrefs.GetBool("Pumkin_MoveCameraOnPlay", true); }
			set { EditorPrefs.SetBool("Pumkin_MoveCameraOnPlay", value); }
		}
		
		[MenuItem("Tools/Pumkin/Move Main Camera On Play", true)]
		public static bool ToggleMoveCameraOnPlayValidation()
		{
			// Toggle action validation
			Menu.SetChecked("Tools/Pumkin/Move Main Camera On Play", moveCameraOnPlayCheckbox);
			return true;
		}
		
		[MenuItem("Tools/Pumkin/Move Main Camera On Play")]
		public static void ToggleMoveCameraOnPlay()
		{
			// Toggle the boolean variable
			moveCameraOnPlayCheckbox = !moveCameraOnPlayCheckbox;
		}

		
		[InitializeOnEnterPlayMode]
		public static void FocusSceneView()
		{
			EditorApplication.delayCall += () =>
			{
				if(GameObject.Find("VRCSDK"))
					return;
				
				if (focusSceneOnPlayCheckbox)
				{
					EditorWindow.FocusWindowIfItsOpen(typeof(SceneView));
				}

				if (moveCameraOnPlayCheckbox)
				{
					// If we're not focusing the scene view, we still need to for a moment in order 
					// to get the scene camera's position and rotation
					if (!focusSceneOnPlayCheckbox)
						EditorWindow.FocusWindowIfItsOpen(typeof(SceneView));
					
					Camera cam = Camera.main;
					if (!cam)
						return;

					EditorApplication.delayCall += () =>
					{
						cam.nearClipPlane = 0.01f;
						Transform sceneCamTransform = SceneView.lastActiveSceneView.camera.transform;
						cam.transform.SetPositionAndRotation(sceneCamTransform.position,
							sceneCamTransform.rotation);
						
						// Back to game view if we weren't focusing the scene view
						if (!focusSceneOnPlayCheckbox)
							EditorApplication.ExecuteMenuItem("Window/General/Game");
					};
				}
			};
		}
	}
}
#endif
