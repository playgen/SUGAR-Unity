# SUGAR Unity Release Change Log
See below details for each release of SUGAR Unity and the notable changes made.

### Next Version
The following will be available in the next version of SUGAR Unity:
- 'success' callbacks renamed to 'onComplete'.
- Fix for being locked into UI when changing aspect ratio from landscape to portrait and vice versa.
- ActorUnityClient created so make functionality for getting groups and users more easily accessible.
- Callbacks now triggered when calling GameData methods with no signed in user.

### 1.2.2
- Updated SUGAR url.

### 1.2.1
- .meta files updated so duplicate .dlls don't cause build failure.

### 1.2.0
- More detailed exception logging.
- User groups set on user group client before login callback is fired.
- User resources set on resource client before login callback is fired.
- Fixed: Saved login credentials not reset when logging in with a different account.

### 1.1.3
- Count added as an Evaluation Criteria Query Type.
- Docs updated.