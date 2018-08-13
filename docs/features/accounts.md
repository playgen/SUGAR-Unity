# Creating Accounts in Game
The Unity Client comes with functionality to create an account or login to SUGAR in your game. This is handled through the Account Panel, found at Assets/SUGAR/Example/Prefabs.

For all login/register functionality, the account panel must have the LoginUserInterface class attached, the panel prompts the user to login. Included in the prefabs folder ("Assets/SUGAR/Prefabs") is a fully functional login panel template. 

The login panel has a rememberMe option, which will set the IssueLoginToken flag to true in the login request, which will return a token for players to use for logging in on subsequent requests. If "Remember Me" has been selected, the interface will pre-populate user details.

The Account Panel uses the following Values found on the SUGAR Prefab in scene:
- **Allow Auto Login** - This enables the use of auto-login (bypassing the need for the login panel). If this is checked, and the command line arguments for auto login have been passed, then the Account Unity Client will attempt to login with the credentials. The command line options format is ``game.exe -u [username] -s [source] -autologin``.
- **Allow Register** - Currently, this displays the makes the register on the login panel visible for custom registration handlers. 
- **Default Source Token** - Source tokens are set to ensure the player is logging in from an allowed source. If the source is not set in the commandline options, then this value becomes the default.

In order to interact with SUGAR, users need to be logged in. By default this is done on startup of the game but can be done at a later point by using: 

``` c#
SUGARManager.Account.DisplayPanel();
```
**Note**: It is important that the SUGAR GameObject is present in your scene before attempting to make any calls, if not using Allow Auto Login, SUGAR will show the account panel immediately 

Further details of SUGARManager functionality can be found [here](sugarmanager.md)