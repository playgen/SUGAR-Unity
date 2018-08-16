# SUGARManager
Interaction with SUGAR in the SUGAR Unity Package is handled through SUGARManager.

## CurrentUser
The currently signed in user, set when a user signs in or registers using the [AccountUnityClient](../api/PlayGen.SUGAR.Unity.AccountUnityClient.yml).

The [ActorResponse](http://api.sugarengine.org/v1/api/PlayGen.SUGAR.Contracts.ActorResponse.html) object contains the user's Id, Name and Description.

## CurrentGroup
The current 'primary' group for the signed in user. Automatically set when signing in to the first group in that user's list of group that they're a member of. Can be manually set using the 'SetCurrentGroup' method in SUGARManager.

The [ActorResponse](http://api.sugarengine.org/v1/api/PlayGen.SUGAR.Contracts.ActorResponse.html) object contains the group's Id, Name and Description.

## Unity Clients
SUGAR Manager Property | Functionality | Unity Client Class
--- | --- | ---
Account | Sign in, register and sign out | [AccountUnityClient](../api/PlayGen.SUGAR.Unity.AccountUnityClient.yml)
Actor | Get, create, update and delete groups. Get and update users | [ActorUnityClient](../api/PlayGen.SUGAR.Unity.ActorUnityClient.yml)
Evaluation | Get list of achievements and skills and user/group progress in each, with an event triggered when one is completed | [EvaluationUnityClient](../api/PlayGen.SUGAR.Unity.EvaluationUnityClient.yml)
GameData | Get and send data related to the game | [GameDataUnityClient](../api/PlayGen.SUGAR.Unity.GameDataUnityClient.yml)
Resource | Get current resources, add resources and send resources to other users | [ResourceUnityClient](../api/PlayGen.SUGAR.Unity.ResourceUnityClient.yml)
GameLeaderboard | Get leaderboards for this game | [LeaderboardListUnityClient](../api/PlayGen.SUGAR.Unity.LeaderboardListUnityClient.yml)
Leaderboard | Get the current standings for a leaderboard | [LeaderboardUnityClient](../api/PlayGen.SUGAR.Unity.LeaderboardUnityClient.yml)
UserFriend | Get current user's list of friends and send and handle friend requests | [UserFriendUnityClient](../api/PlayGen.SUGAR.Unity.UserFriendUnityClient.yml)
UserGroup | Get current user's list of groups and send and handle group requests | [UserGroupUnityClient](../api/PlayGen.SUGAR.Unity.UserGroupUnityClient.yml)
GroupMember | Get list of members for a group | [GroupMemberUnityClient](../api/PlayGen.SUGAR.Unity.GroupMemberUnityClient.yml)
Unity | Create client, enable and disable SUGAR objects, start and stop the loading spinner | [SUGARUnityManager](../api/PlayGen.SUGAR.Unity.SUGARUnityManager.yml)

## Client
All functionality within SUGAR Unity goes through this SUGAR Client. It is created within the 'CreateSUGARClient' method and set within the 'CreateSUGARClient' method of [SUGARUnityManager](../api/PlayGen.SUGAR.Unity.SUGARUnityManager.yml), both of which are triggered from its 'Awake' method.

Any player-facing SUGAR functionality can be accessed using the Client directly instead. 

**Important**: the Client does not make the same checks as SUGAR Unity to ensure that functionality can only be called once a user is signed in and that only functionality that the user has access to is available, so additional care should be taken when using the Client directly to avoid calls failing.

## UserSignedIn
Boolean used to confirm if a user is signed in. If no user is signed in, most functionality will not be accessible.

## ClassId
Class name for the currently signed in user. Can be set when launching the application via the Command Line or via the 'SetClassId' method in SUGARManager.

## GameId
The Id of the game. Set in 'Awake' in the [SUGARUnityManager](../api/PlayGen.SUGAR.Unity.SUGARUnityManager.yml) using the value set in the Inspector and cannot be overwritten.
