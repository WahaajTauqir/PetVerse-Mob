using UnityEngine;
using UnityEditor;

public class FbxAnimationPathCleaner : EditorWindow
{
    private Object fbxAsset;
    private string armatureName = "RootBone";

    [MenuItem("Tools/FPX Animation Path Cleaner")]
    public static void ShowWindow()
    {
        GetWindow<FbxAnimationPathCleaner>("FBX Animation Path Cleaner");
    }

    void OnGUI()
    {
        GUILayout.Label("Select FBX Asset", EditorStyles.boldLabel);
        fbxAsset = EditorGUILayout.ObjectField("FBX File", fbxAsset, typeof(Object), false);
        armatureName = EditorGUILayout.TextField("Armature Name", armatureName);

        if (fbxAsset != null && GUILayout.Button("Clean Animations Path"))
        {
            RemoveArmaturePrefix(fbxAsset, armatureName);
        }
    }

    void RemoveArmaturePrefix(Object fbxObj, string armatureName)
    {
        string assetPath = AssetDatabase.GetAssetPath(fbxObj);
        var allAssets = AssetDatabase.LoadAllAssetsAtPath(assetPath);

        int clipsRenamed = 0;
        string prefix = armatureName + "|";

        foreach (var asset in allAssets)
        {
            if (asset is AnimationClip clip)
            {
                string oldName = clip.name;
                if (oldName.Contains(prefix))
                {
                    string newName = oldName.Replace(prefix, "");
                    Undo.RecordObject(clip, "Rename AnimationClip");
                    clip.name = newName;
                    clipsRenamed++;
                    EditorUtility.SetDirty(clip);
                }
            }
        }
        AssetDatabase.SaveAssets();
        Debug.Log($"Renamed {clipsRenamed} AnimationClips.");
    }
}