using System.ComponentModel;
using PlayGen.SUGAR.Client;
using UnityEngine;

namespace PlayGen.SUGAR.Unity
{
    class SavedPrefsHandler : ISavedPrefsHandler
    {
	    public string Prefix => "SUGAR_PREFS_";

	    public T Get<T>(string key)
	    {
		    var value = PlayerPrefs.GetString(Prefix + key);
			var converter = TypeDescriptor.GetConverter(typeof(T));

			return (T) converter.ConvertFromString(value);
		}

	    public void Save<T>(string key, T value)
	    {
		    PlayerPrefs.SetString(Prefix + key, value.ToString());
	    }

	    public void Delete(string key)
	    {
		    PlayerPrefs.DeleteKey(Prefix + key);
	    }
	}
}
