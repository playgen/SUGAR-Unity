# Quick Start

**Note: Please ensure you have created a SUGAR account before going through the following steps.**

See [creating accounts](creating-accounts.md) for more details.

## Import the SUGAR Unity Client
1. Downlaod the SUGAR Unity package from the Unity Asset Store [here](https://assetstore.unity.com/packages/tools/network/sugar-social-gamification-107078).  
OR  
If you have downloaded the source code, follow the [build instructions](../development/build-instructions.md).

## Add SUGAR to your game
Add the 'SUGAR' prefab, found at SUGAR/Prefabs/SUGAR, into your starting scene. All of the interfaces referenced on the Unity Clients on this object can be found at SUGAR/Example/Prefabs/Landscape. 

The SUGAR Unity Manager script component on the prefab holds universal information required by other SUGAR components. Configuring this component is necessary to connect to SUGAR.
- Base Address - web address of SUGAR server (e.g. ``http://localhost:59400/`` or ``http://www.mysugarserver.com``). This is overwritten by the value set inside the config.json file (found in "Assets/StreamingAssets").
- Game Token - name of the <xref:game> used for database lookup.
- Game Id - database row Id of the <xref:game>, returned from token lookup.
- Source - [Account Source](../features/accountsources.md) to log in to. Use "SUGAR" as default to get started quickly or if you don't need your own account source.

## Creating your Game in SUGAR
New games can be created through the admin panel or through the C# API, but the simplest is to use a game seed through Unity. The seed file allows you to define the name, achievements, leaderboards and skills at the same time. 

### Create Game Seed File  
1. Open the 'Edit Game Seed' tool by clicking Tools/SUGAR/Edit Game Seed.  
2. Create a new game seed by clicking the 'Create Game Seed' button.  
3. Fill in the 'Name' field with the name of the game you wish to seed.  
4. Save this basic Seed file by clicking the 'Save' button. If you do not change the selected file, this will overwrite the provided 'GameSeed' file.
5. If you wish to set up the achievements, leaderboards and skills for your game now, go to the guide on [Seeding](../features/seeding.md) for further details.  

### Seed Game  
1. Open the 'Seed Game' tool by clicking Tools/SUGAR/Seed Game.  
2. Fill in the provided Username and Password fields with your SUGAR details.  
3. If you did not overwrite the provided 'GameSeed' file, change the 'Game Seed File' field to use the file you created during step 2.  
4. Click the 'Sign-in and Seed' button to add the game to the platform. This step will fail if the seed file is invalid or you provide invalid user details.  
5. Check that the 'Game Token' and 'Game Id' fields on the SUGAR object in your starting scene has been edited to match the details of the game you just created.  

## Using SUGAR in your game
Once you have set up your game in SUGAR, you are now ready to log in and start using SUGAR features in your game. SUGAR functionality is controlled through the [SUGARManager](../features/sugarmanager.md). 

Before using SUGAR functionality during your game, a user should be logged in.

### Logging in to SUGAR
By default when the SUGAR gameobject is present in the scene and no user is logged in, the login panel will be shown to the user. In the example scene this will happen as soon as the project is run. Alternatively you can set auto login values (for testing purposes).

### Setting Auto Log in values (optional)
1. Open the 'Set Auto Log-in Values' tool by clicking Tools/SUGAR/Set Auto Log-in Values.  
2. Fill in the details you want to use to automatically sign in when testing in Unity.  
3. This feature is disabled if either 'Auto Log-in' within the tool or 'Allow Auto Login' in 'Account Unity Client' on the 'SUGAR' prefab is not checked.  

## Testing SUGAR features in Unity Project
In the unity project, open *Unity/Assets/SUGAR/Example/Scene.unity*, after rebuilding the PlayGen.SUGAR.Unity project you can test the functionality with the following commands:

Shortcut | Test
--- | ---
T + L | GameLeaderboard.DisplayGameList
T + K | Evaluation.DisplayAchievementList
T + A | Evaluation.ForceNotification
T + S | Unity.StartSpinner
T + H | Unity.StopSpinner
T + F | UserFriend.Display
T + G | UserGroup.Display
Escape | Application.Quit
Delete | Account.Logout

Shortcuts can be seen and changed in *Unity/Assets/SUGAR/Example/Scripts/TestImplementation.cs*.