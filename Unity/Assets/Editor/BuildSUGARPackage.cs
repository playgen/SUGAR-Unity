using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;

namespace SUGAR.Unity
{
    public static class BuildSUGARPackage
    {
        [MenuItem("Tools/Build SUGAR Package")]
        public static void Build()
        {
            var rootDir = Directory.GetParent(Application.dataPath).Parent.FullName;
            var packageFile = rootDir + "/Build/SUGAR.Unity.unitypackage";
            var directory = "Assets/SUGAR";

            var packageAssetPaths = new List<string>();

            foreach (var assetPath in AssetDatabase.GetAllAssetPaths())
            {
                if (assetPath.StartsWith(directory)
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
