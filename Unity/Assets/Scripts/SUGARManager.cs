using UnityEngine;
using PlayGen.SUGAR.Client;

namespace SUGAR.Unity
{
    [RequireComponent(typeof(AccountUnityClient))]
    public class SUGARManager : MonoBehaviour
    {
        [SerializeField] private string _baseAddress;

		[SerializeField] private int _gameId;

        void Awake()
        {
			if (SUGAR.Register(this))
			{
				DontDestroyOnLoad(this);
			}
			else
			{
				Destroy(gameObject);
			}
			SUGAR.Client = new SUGARClient(_baseAddress); // hTTPhANDLER ?>?!
			SUGAR.GameId = _gameId;
        }
    }
}