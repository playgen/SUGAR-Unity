using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;

namespace PlayGen.SUGAR.Unity
{
	public static class BuildSUGARPackage
	{
		[MenuItem("Tools/Build SUGAR Package")]
		public static void Build()
		{
			var rootDir = Directory.GetParent(Application.dataPath).Parent.FullName;
			var packageFile = rootDir + "/Build/SUGAR.unitypackage";
			var directory = new[]
			{
				"Assets/Editor/SUGAR",
				"Assets/StreamingAssets",
				"Assets/SUGAR"
			};

			var packageAssetPaths = new List<string>();

			foreach (var assetPath in AssetDatabase.GetAllAssetPaths())
			{
				if (directory.Any(dir => assetPath.StartsWith(dir))
					&& File.Exists(assetPath))   // is file?
					
				{
					packageAssetPaths.Add(assetPath);
				}
			}

			var packageDir = Path.GetDirectoryName(packageFile);
			if (!Directory.Exists(packageDir))
			{
				Directory.CreateDirectory(packageDir);
			}

			AssetDatabase.ExportPackage(packageAssetPaths.ToArray(), packageFile);

			Debug.Log("Exported package to: \"" + packageFile + "\"");
		}
	}
} 
