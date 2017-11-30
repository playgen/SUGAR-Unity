# Build Instructions

Follow these steps to generate a SUGAR Unity client unitypackage

1. Open PlayGen.Sugar.Unity in Visual Studio and build it.
2. In the build output folder: Unity/Assets/SUGAR/bin/.. delete UnityEngine.dll.
3. From the Tools menu, click Generate Package.
4. Check Unity's Console window for the unity package build location or any errors.

# Documentation

API Documentation is built by DocFX using the tripple slash code comments in the source code and additional settings in the docs/ folder.

To generate the documentation, run docs/build.bat