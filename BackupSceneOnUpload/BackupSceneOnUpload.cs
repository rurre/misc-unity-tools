/// SaveAvatarSceneBackupOnUpload.cs by Pumkin
/// https://github.com/rurre/misc-unity-tools
/// Basic scene backing up system. Creates a backup of your scene when uploading an Avatar to VRC.

#if UNITY_EDITOR && VRC_SDK_VRCSDK3
using System;
using System.IO;
using UnityEditor;
using UnityEngine;
using VRC.SDKBase.Editor.BuildPipeline;

public class SaveAvatarSceneBackupOnUpload : IVRCSDKPreprocessAvatarCallback
{
    const string backupFolderName = "Scene Backups";
    const string dateTimeFormat = "yyyy-mm-dd hh-mm";
    const string sceneNameFormat = "{sceneName} {dateTime}";

    public int callbackOrder => int.MaxValue;

    public static bool BackupsEnabled
    {
        get { return EditorPrefs.GetBool("Pumkin_SaveSceneBackupsOnUpload", true); }
        set { EditorPrefs.SetBool("Pumkin_SaveSceneBackupsOnUpload", value); }
    }

    [MenuItem("Tools/Pumkin/Backup Scene on Upload", true)]
    public static bool ToggleBackupsEnabledValidation()
    {
        Menu.SetChecked("Tools/Pumkin/Backup Scene on Upload", BackupsEnabled);
        return true;
    }

    [MenuItem("Tools/Pumkin/Backup Scene on Upload")]
    public static void ToggleBackupsEnabled()
    {
        BackupsEnabled = !BackupsEnabled;
    }


    public bool OnPreprocessAvatar(GameObject avatarGameObject)
    {
        try
        {
            string avatarScenePath = avatarGameObject.scene.path;

            string sceneDirectory = Path.GetDirectoryName(avatarScenePath);
            string sceneName = Path.GetFileNameWithoutExtension(avatarScenePath);
            string backupDirectory = $"{sceneDirectory}/{backupFolderName}";

            if(!AssetDatabase.IsValidFolder(backupDirectory))
            {
                Debug.Log($"Backup Scene On Upload: {backupDirectory} doesn't exist. Creating.");
                string createFolderResult = AssetDatabase.CreateFolder(sceneDirectory, backupFolderName);
                if(string.IsNullOrWhiteSpace(createFolderResult))
                    throw new Exception("Failed to create backup folder. Skipping backup.");
            }

            string nowString = DateTime.Now.ToString(dateTimeFormat);
            string finalSceneName = sceneNameFormat.Replace("{dateTime}", nowString).Replace("{sceneName}", sceneName);
            string finalSceneAssetPath = $"{backupDirectory}/{finalSceneName}.unity";

            Debug.Log($"Backup Scene On Upload: Creating scene backup of scene <b>{sceneName}</b> at <b>{finalSceneAssetPath}</b>");
            if(!AssetDatabase.CopyAsset(avatarScenePath, finalSceneAssetPath))
                throw new Exception("Failed to copy scene asset");
        }
        catch(Exception ex)
        {
            Debug.LogException(ex);
        }
        return true;
    }
}
#endif
