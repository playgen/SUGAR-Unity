using System.ComponentModel;
using PlayGen.SUGAR.Client;
using UnityEngine;

namespace PlayGen.SUGAR.Unity
{
    class SavedPrefsHandler : ISavedPrefsHandler
    {
	    public string Prefix => "SUGAR_PREFS_LOGIN_";

	    public T Get<T>(string key)
	    {
		    var value = PlayerPrefs.GetString(Prefix + key);
			var converter = TypeDescriptor.GetConverter(typeof(T));

			if (converter != null)
			{
				return (T) converter.ConvertFromString(value);
			}

			return default(T);
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
