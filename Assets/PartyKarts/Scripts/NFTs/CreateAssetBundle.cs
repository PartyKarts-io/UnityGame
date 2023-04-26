#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

public class CreateAssetBundle : MonoBehaviour
{
    [MenuItem("Assets/Build AssetBundles")]
    static void BuildBundles()
    {
        string assetBundleDirectory = "Assets/AssetBundles";

        if (!System.IO.Directory.Exists(assetBundleDirectory))
        {
            System.IO.Directory.CreateDirectory(assetBundleDirectory);
        }

        BuildPipeline
            .BuildAssetBundles(assetBundleDirectory,
            BuildAssetBundleOptions.None,
            BuildTarget.WebGL);
    }
}
#endif