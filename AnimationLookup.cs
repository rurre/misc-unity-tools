/// AnimationLookup.cs
/// By Pumkin 
/// https://github.com/rurre/misc-unity-tools
/// Lets you see what animations change a property of your choice in an animator.

#if UNITY_EDITOR
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Pumkin
{
    public class AnimationLookup : EditorWindow
    {
        enum SearchType
        {
            Blendshape,
            EnableState,
            MaterialProperty,
            ComponentProperty,
        }

        SearchType searchType = SearchType.ComponentProperty;
		Transform avatar;
        RuntimeAnimatorController runtimeAnimator;
        string search;
        Component component;
        string path;
        Vector2 scroll = Vector2.zero;
        string fieldName = "Property Name";

        List<AnimationClip> results = new List<AnimationClip>();

        [MenuItem("Tools/Pumkin/Animation Lookup")]
        public static void ShowWindow()
        {
            var window = GetWindow<AnimationLookup>();
            window.titleContent = new GUIContent("Animation Lookup");
            window.Show();
        }

        void OnGUI()
        {
            EditorGUILayout.Space();

			avatar = EditorGUILayout.ObjectField("Avatar", avatar, typeof(Transform), true) as Transform;
            runtimeAnimator = EditorGUILayout.ObjectField("Animation Controller", runtimeAnimator, typeof(RuntimeAnimatorController), true) as RuntimeAnimatorController;

            EditorGUILayout.Space();

            component = EditorGUILayout.ObjectField("Component", component, typeof(Component), true) as Component;
            using(var changeScope = new EditorGUI.ChangeCheckScope())
            {
                searchType = (SearchType)EditorGUILayout.EnumPopup("Search Type", searchType);
                if(changeScope.changed)
                {
                    switch(searchType)
                    {
                        case SearchType.Blendshape:
                            fieldName = "Blendshape Name";
                            break;
                        case SearchType.ComponentProperty:
                            fieldName = "Property Name";
                            break;
                        case SearchType.MaterialProperty:
                            fieldName = "Material Property";
                            break;
                        default:
                        case SearchType.EnableState:
                            break;
                    }
                }
            }

            if(searchType != SearchType.EnableState)
                search = EditorGUILayout.TextField(fieldName, search);

            if(searchType == SearchType.ComponentProperty)
                EditorGUILayout.HelpBox(new GUIContent("To find the name of a property shift+right click a field in the inspector."));

            EditorGUILayout.Space();
            using(new EditorGUI.DisabledGroupScope(!runtimeAnimator || string.IsNullOrWhiteSpace(search) || !component))
            {
                if(GUILayout.Button("Search"))
                {
                    PerformSearch();
                }
            }

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Results:");
            using(new EditorGUILayout.ScrollViewScope(scroll))
                foreach(var clip in results)
                    EditorGUILayout.ObjectField(clip, typeof(AnimationClip), true);
        }

        void PerformSearch()
        {
            if(!IsDeepChildOf(avatar.transform, component.transform))
            {
                Debug.LogError($"{component.transform.name} is not a child of {avatar.transform.name}");
                return;
            }

            string prop = null;
            switch(searchType)
            {
                case SearchType.Blendshape:
                    prop = "blendShape." + search;
                    break;
                case SearchType.EnableState:
                    prop = "m_Enabled";
                    break;
                case SearchType.ComponentProperty:
                    prop = search;
                    break;
                case SearchType.MaterialProperty:
                    prop = "material." + search;
                    break;
                default:
                    break;
            }

            if(prop == null)
            {
                Debug.LogError("Invalid Search Type selected.");
                return;
            }

            results = FindUsages(prop);
        }

        bool IsDeepChildOf(Transform parent, Transform obj)
        {
            var trans = parent.GetComponentsInChildren<Transform>(true);
            return trans.Any(t => t == obj);
        }

        List<AnimationClip> FindUsages(string propertyName)
        {
            var clips = new List<AnimationClip>();
            path = AnimationUtility.CalculateTransformPath(component.transform, avatar.transform);
            if(runtimeAnimator && runtimeAnimator.animationClips != null)
                foreach(var clip in runtimeAnimator.animationClips)
                    if(AnimationUtility.GetCurveBindings(clip).Any(c => c.path == path && c.propertyName == propertyName))
                        clips.Add(clip);
            return clips;
        }
    }
}
#endif