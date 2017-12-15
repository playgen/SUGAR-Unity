# Quick Start

**Note: Please ensure you have created a SUGAR account before going through the following steps.**

#### Add SUGAR  
1. Add the 'SUGAR' prefab, found at SUGAR/Prefabs/SUGAR, into your starting scene. All of the interfaces referenced in the Unity Clients on this object can be found at SUGAR/Example/Prefabs/Landscape.   

#### Create Game Seed File  
1. Open the 'Edit Game Seed' tool by clicking Tools/SUGAR/Edit Game Seed.  
2. Create a new game seed by clicking the 'Create Game Seed' button.  
3. Fill in the 'Name' field with the name of the game you wish to seed.  
4. Save this basic Seed file by clicking the 'Save' button. If you do not change the selected file, this will overwrite the provided 'GameSeed' file.
5. If you wish to set up the achievements, leaderboards and skills for your game now, go to the guide on [Seeding](seeding.md) for further details.  

#### Seed Game  
1. Open the 'Seed Game' tool by clicking Tools/SUGAR/Seed Game.  
2. Fill in the provided Username and Password fields with your SUGAR details.  
3. If you did not overwrite the provided 'GameSeed', change the 'Game Seed File' field to use the file you created during step 2.  
4. Click the 'Sign-in and Seed' button to add the game to the platform. This step will fail if the seed file is invalid or you provide invalid user details.  
5. Check that the 'Game Token' and 'Game Id' fields on the SUGAR object in your starting scene has been edited to match the details of the game you just created.  

#### Set Auto Log-in Values (optional)  
1. Open the 'Set Auto Log-in Values' tool by clicking Tools/SUGAR/Set Auto Log-in Values.  
2. Fill in the details you want to use to automatically sign in when testing in Unity.  
3. This feature is disabled if either 'Auto Log-in' within the tool or 'Allow Auto Login' in 'Account Unity Client' on the 'SUGAR' prefab is not checked.  