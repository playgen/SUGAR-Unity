# Build Instructions

Can be found [here](docs/tutorials/build-instructions.md)

# Documentation

[API Documentation](docs/index.md) is built by DocFX using the tripple slash code comments in the source code and additional settings in the docs/ folder.

To generate the documentation, run docs/build.bat

# SUGAR Version
Commit hash: 8052283f57db7657b3a905468fcd3cc5c1839b52

# Testing SUGAR features in Unity Project
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