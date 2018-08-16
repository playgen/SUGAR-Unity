# Creating Accounts in Game
The Unity Client comes with functionality to create an account or login to SUGAR in your game. This is handled through an Account Panel, with template prefabs provided at Assets/SUGAR/Example/Prefabs. In order to display the Account Panel, the correct prefab must also be provided on the AccountUnityClient within the SUGAR prefab.

The login panel has a RememberMe option, which will set the IssueLoginToken flag to true in the login request, which will return a token for players to use for logging in on subsequent requests. If "Remember Me" has been selected, the interface will pre-populate user details.

The Account Panel uses the following values found on the SUGAR Prefab in scene:
- **Allow Auto Login** - This enables the use of auto-login (bypassing the need for the login panel). If this is checked, and the command line arguments for auto login have been passed, then the Account Unity Client will attempt to login with the credentials. The command line options format is ``game.exe -u [username] -p [password] -s [source] -autologin``.
- **Allow Register** - Currently this set the register button on the Account Panel should be visible. 
- **Default Source Token** - Source tokens are set to ensure the player is logging in from an allowed source. This value is always used unless it has been set within command line options for an auto log-in attempt.

In order to interact with SUGAR, users need to be logged in. It is recommended this is done as close to the start-up of the game as possible, but it can be done at any point in the game's lifecycle. To trigger displaying the Account Panel (or an auto log-in if values are provided), call the following:

``` c#
SUGARManager.Account.DisplayLogInPanel(Action<bool> onComplete);
```
**Note**: It is important that the SUGAR GameObject is present in your scene before attempting to make any calls.

Further details of SUGARManager functionality can be found [here](sugarmanager.md)