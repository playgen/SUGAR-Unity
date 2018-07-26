# SUGARManager
Interaction with SUGAR in the SUGAR Unity Package, is handled through the SUGAR Manager. Before making any calls to SUGAR, there must be a currently signed in user. Check that a user is currently signed in with:

``` c#
    SUGARManager.CurrentUser != null
```

## SUGARManager.CurrentUser
The currently signed in user. [ActorResponse](http://api.sugarengine.org/v1/api/PlayGen.SUGAR.Contracts.ActorResponse.html) data from the Login Request

it is advised to never set the value of this manually.

## SUGARManager.CurrentGroup
The current group for the signed in user. [ActorResponse](http://api.sugarengine.org/v1/api/PlayGen.SUGAR.Contracts.ActorResponse.html) data retrieved from Group Selection in the GroupsList Prefab

it is advised to never set the value of this manually.

## SUGARManager Client
The SUGARManager Client data will be set once a user has successfully signed in, full details of requests that can be sent and data that can be retrieved can be found in the [Playgen.SUGAR.Unity API](http://api.sugarengine.org/v1/unity-client/api/PlayGen.SUGAR.Unity.SUGARManager.html). Attempting to make requests to SUGAR without first logging in, will fail due to the player data, [permissions](http://api.sugarengine.org/v1/concepts/rolesandpermissions.html) and [Session](http://api.sugarengine.org/v1/concepts/session.html) not being created.
