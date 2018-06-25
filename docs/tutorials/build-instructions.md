# Build Instructions

1. Open and build the PlayGen.SUGAR.Unity project.
2. Open the Unity project.
3. Click Menu/Tools/Build SUGAR Package.
4. Copy the built package from Build/SUGAR.unitypackage.
5. Import it into your project and let the magic begin.

## Testing SUGAR features in Unity
In the unity project, launch Scene.unity, after rebuilding the PlayGen.SUGAR.Unity project you can test the functionality with the following commands:

Shortcut | Test
--- | ---
T + L | GameLeaderboard.DisplayGameList
T + K | Evaluation.DisplayAchievementList
T + A | Evaluation.ForceNotification
T + S | Unity.StartSpinner
T + H | Unity.StopSpinner
T + F | UserFriend.Display
T + G | UserGroup.Display
Esc | Application.Quit

Shortcuts can be seen and changed [here](../../Unity/Assets/SUGAR/Example/Scripts/TestImplementation.cs)