using System.IO;
using UnityEditor;
using UnityEngine;

public class CreateAssetBundles
{

	[MenuItem("Assets/Build AssetBundles")]
	static void BuildAllAssetBundles()
	{
		string folderName = "AssetBundles";
		string filePath = Path.Combine(Application.streamingAssetsPath, folderName);
		BuildPipeline.BuildAssetBundles(filePath, BuildAssetBundleOptions.None, BuildTarget.StandaloneWindows64);

		AssetDatabase.Refresh();
	}
}