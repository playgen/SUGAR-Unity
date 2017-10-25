# Build Instructions

Follow these steps to generate a SUGAR Unity client unitypackage

1. Open PlayGen.Sugar.Unity in Visual Studio and build it.
2. In the build output folder: Unity/Assets/SUGAR/bin/.. delete UnityEngine.dll.
3. Open the Unity project in Unity and set PlayGen.SUGAR.Unity.Editor.dll to be editor only.
4. From the Tools menu, click Generate Package.
5. Check Unity's Console window for the unity package build location or any errors.