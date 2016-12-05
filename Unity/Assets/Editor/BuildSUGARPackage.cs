﻿using UnityEngine;
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
            var directory = new[] { "Assets/SUGAR", "Assets/Plugins", "Assets/StreamingAssets" };

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
