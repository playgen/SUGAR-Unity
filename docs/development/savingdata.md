# Saving Data
SUGAR Unity includes a class which handles saving and retrieving of data, similar to the way in which [Player Prefs](https://docs.unity3d.com/ScriptReference/PlayerPrefs.html) are set, retrieved and deleted. The class uses the following prefix: **SUGAR_PREFS_** followed by the key provided.

## Current Usage
Currently the saving and retrieving of data is used for storing player login tokens, allowing for players login details to be saved so they can log in without entering details every time