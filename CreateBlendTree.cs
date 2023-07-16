/// CreateBlendTree.cs by Pumkin
/// https://github.com/rurre
/// Adds an option to create a new blend tree to the Right Click > Create menu in assets.

#if UNITY_EDITOR
using System.IO;
using UnityEditor;
using UnityEditor.Animations;
using UnityEditor.ProjectWindowCallback;
using UnityEngine;

namespace CreateBlendTree
{
    public class CreateBlendTree : Editor
    {
        [MenuItem("Assets/Create/BlendTree", priority = 411)]
        static void CreateNewBlendTree()
        {
            ProjectWindowUtil.StartNameEditingIfProjectWindowExists(
               0,
               CreateInstance<DoCreateBlendTree>(),
               "New BlendTree.asset",
               EditorGUIUtility.IconContent("BlendTree Icon").image as Texture2D,
               null);
        }

        class DoCreateBlendTree : EndNameEditAction
        {
            public override void Action(int instanceId, string pathName, string resourceFile)
            {
                BlendTree blendTree = new BlendTree { name = Path.GetFileNameWithoutExtension(pathName) };
                AssetDatabase.CreateAsset(blendTree, pathName);
                Selection.activeObject = blendTree;
            }
        }
    }
}
#endif
