using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using UnityEditor;
using Debug = UnityEngine.Debug;

namespace PlayGen.SUGAR.Unity
{
	public static class BuildSUGARPackage
	{
	    private static string RootDir => Directory.GetParent(Application.dataPath).Parent.FullName;

        [MenuItem("Tools/Build/Docs and SUGAR Package")]
	    public static void BuildDocsAndSUGARPackage()
	    {
	        BuildDocs();
	        BuildSUGAR();

	    }

        [MenuItem("Tools/Build/Docs")]
	    public static void BuildDocs()
        {
            EditorUtility.DisplayProgressBar("Building docs", "...", 0);

	        Process.Start(new ProcessStartInfo
	        {
                WorkingDirectory = $"{RootDir}\\docs\\tools",
                FileName = "CMD.exe",
                Arguments = "/c START /WAIT all_and_copy.bat"
	        }).WaitForExit();

            AssetDatabase.Refresh();
            EditorUtility.ClearProgressBar();
        }

	    [MenuItem("Tools/Build/SUGAR Package")]
		public static void BuildSUGAR()
		{
		    EditorUtility.DisplayProgressBar("Building SUGAR Package", "...", 0);

            var versionPath = "Assets/SUGAR/Version.txt";            
			var packageVersion = File.ReadAllText(versionPath);            
			var packageFile = $"{RootDir}/Build/SUGAR_{packageVersion}.unitypackage";

			var directory = new[]
			{
				"Assets/Editor/SUGAR",
				"Assets/StreamingAssets",
				"Assets/SUGAR"
			};

			var ignoreExtensions = new[] 
			{
				".mdb"
			};

			var packageAssetPaths = new List<string>();

			foreach (var assetPath in AssetDatabase.GetAllAssetPaths())
			{
				if (directory.Any(dir => assetPath.StartsWith(dir))
					&& File.Exists(assetPath)
					&& !ignoreExtensions.Contains(Path.GetExtension(assetPath)))   // is file?
					
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

			Debug.Log($"Exported package to: \"{packageFile}\"");

		    EditorUtility.ClearProgressBar();
        }
	}
} 
