---
uid: client-prefabs
---

# Unity Client Prefabs

There are a number of prefabs included within the Unity package. Example usage can be viewed in the demo scene which can be found at: Assets/SUGAR/Example/Scene.unity

After setting up a game using the [quick start guide](../tutorials/quick-start.md), all prefabs can be seen working by running the demo scene and entering the shortcuts to display each.

Each of the prefabs can be activated using the SUGAR prefab provided (which must be added to the scene to use SUGAR). The SUGAR prefab has its own canvas which is rendered above the main Canvas which contains each of the panel prefabs. Each panel can be activated using their display functions, examples of which can be seen in: Assets/SUGAR/Example/Scripts/TestImplemtation.cs 

## Panel Prefabs

Panel Prefabs provided
    * AccountPanel
    * EvaluationPanel
    * EvaluationPopup
    * FriendsListPanel
    * UserGroupsPanel
    * GroupMembersPanel 
    * LeaderboardListPanel
    * LeaderboardPanel

There are 2 variations provided for each of the panels, one for landscape and one for portrait. To switch the orientation used, change the Interface references in the SUGAR prefab to the required orientation prefabs.

## Panel Overview 
* **AccountPanel**

    Handles logging in to SUGAR with username and password entered, does not display if auto login is enabled and the login details are correct.

* **EvaluationPanel**

    Demo Shortcut: Hold T & Press K.
    
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
    
    Displays the members of a specific group, allowing accounts with permissions to manage other accounts within that group.

* **LeaderboardListPanel**

    Demo Shortcut: Hold T & Press L.
    
    Displays a list of leaderboards for the current game, allowing users to select one and see the standings.

* **LeaderboardPanel**

    Demo Shortcut: Hold T & Press L -> select a leaderboard.
    
    Displays the current standings for the selected leaderboard, allows users to filter results by:  
      * Top (the best scores for the current leaderboard),  
      * Nearby (the scores near to the current users score),
      * Friends (the current users' friends scores),
      * Group Members (the scores for all members of your primary group),
      * Alliances (the scores of all groups in an alliance with your primary group).