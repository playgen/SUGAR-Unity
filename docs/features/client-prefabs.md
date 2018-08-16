---
uid: client-prefabs
---

# Unity Client Prefabs

There are a number of prefabs included within the Unity package. Example usage can be viewed in the demo scene which can be found at: Assets/SUGAR/Example/Scene.unity

After setting up a game using the [quick start guide](../tutorials/quick-start.md), all prefabs can be seen working by running the demo scene and entering the shortcuts to display each.

Each of the prefabs can be activated using the SUGAR prefab provided (which must be added to the scene to use SUGAR). The SUGAR prefab has its own canvas which is rendered above the main Canvas which contains each of the panel prefabs. Each panel can be activated using their display functions, examples of which can be seen in TestImplementation.cs.

## Panel Prefabs

Panel Prefabs provided:
- AccountPanel
- EvaluationPanel
- EvaluationPopup
- FriendsListPanel
- GroupMembersPanel 
- LeaderboardListPanel
- LeaderboardPanel
- UserGroupsPanel

There are 2 variations provided for each of the panels, one for landscape and one for portrait. The provided SUGAR prefab uses the landscape and portrait prefabs provided and will always display the correct interface for the current game resolution.

### Changing SUGAR Panels
The example prefabs provided are all flexible and can be changed to better suit your game. The Interface classes found at Assets/SUGAR/Example/Scripts control how the data is handled and shown in the panel, these may need to be edited to work for any changes made to the layout.

If an interface is not used, such as because the feature is not going to be implemented or because it is for an aspect ratio that will not be used by the game, it can be safely removed from the project and the reference removed from the SUGAR prefab. SUGAR will only try to display interfaces when they have been provided and no errors will occur as a result of their removal.

### Creating your own SUGAR Panel
If you wish to create your own SUGAR panels, or want to adapt current UI in your game to now work with SUGAR, simply change the reference GameObject in your SUGAR prefab to a prefab that uses the corresponding Interface class found in Assets/SUGAR/Example/Scripts or uses a class derived from the corresponding base Interface class found within the PlayGen.SUGAR.Unity DLL and namespace.

## Panel Overview 
* **AccountPanel**

    Demo Shortcut: Triggered at the start of the demo scene. Can also be triggered by logging out with the 'Delete' Key.

    Handles logging in to SUGAR with username and password entered, does not display if auto login is enabled and the login details are correct.

* **EvaluationPanel**

    Demo Shortcut: Hold T & Press K for Achievements. Hold T & Press J for Skills.
    
    Displays the achievements/skills for the current game and which ones have been completed. 

* **EvaluationPopup**

    Demo Shortcut: Hold T & Press A.
    
    Displays a pop-up to notify users that an achievement or skill has been completed. 

* **FriendsListPanel**

    Demo Shortcut: Hold T & Press F.
    
    Displays the users that the logged in account is friends with, allowing them to make new friendships, remove existing ones and review pending requests.

* **UserGroupsPanel**

    Demo Shortcut: Hold T & Press G.
    
    Displays the groups that the logged in account is a member of, allowing them to leave current groups, join new groups and see pending group requests.

* **GroupMembersPanel** 

    Demo Shortcut: Hold T & Press G -> select a group.
    
    Displays the members of a specific group and allows the current user to change their current relationship with any user, similar to the functionality provided by the Friends List Panel.

* **LeaderboardListPanel**

    Demo Shortcut: Hold T & Press L.
    
    Displays a list of leaderboards for the current game, allowing users to select one and see the standings.

* **LeaderboardPanel**

    Demo Shortcut: Hold T & Press L -> select a leaderboard.
    
    Displays the current standings for the selected leaderboard, allows users to filter results by:  
    - Top - the best scores for the current leaderboard
    - Nearby - the scores near to the current users score
    - Friends - the current users' friends scores
    - Group Members - the scores for all members of your primary group
    - Alliances - the scores of all groups in an alliance with your primary group