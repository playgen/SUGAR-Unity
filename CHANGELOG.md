# SUGAR Unity Release Change Log
See below details for each release of SUGAR Unity and the notable changes made.

### Next Version
The following will be available in the next version of SUGAR Unity:
- Add Reload abstract method for BaseInterface.
- Add ActorUnityClient so make functionality for getting groups and users more easily accessible.
- Add ability to seed Groups.
- Add functionality to display Achievement and Skill progress at the same time.
- Add methods that allows evaluation progress to be gathered for any actor or actor id.
- Add methods that allows the member list to be gathered for any group or group id.
- Add public method of getting details for a leaderboard by token.
- Add public methods of getting leaderboard standings.
- Update 'success' callbacks renamed to 'onComplete'.
- Update Callbacks now triggered when calling GameData methods with no signed in user.
- Update Auto Log-in Editor tool values now saved per project (according to the Product Name) on the same machine.
- Update interfaces and unity clients related to relationships in order to better fit expected functionality and to make them more consistent with the rest of the project.
- Update to provided localization resource.
- Fix for being locked into UI when changing aspect ratio from landscape to portrait and vice versa.
- Fix for streaming assets config file being required in order to use the seeding functionality.
- Fix for invoke in ResourceUnityClient resulting in debug messages.

### 1.2.2
- Update SUGAR url.

### 1.2.1
- Update .meta files so duplicate .dlls don't cause build failure.

### 1.2.0
- Update exception logging to be more detailed.
- Update User groups set on user group client before login callback is fired.
- Update User resources set on resource client before login callback is fired.
- Fix Saved login credentials not reset when logging in with a different account.

### 1.1.3
- Add Count as an Evaluation Criteria Query Type.
- Update Docs.