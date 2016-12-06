using PlayGen.SUGAR.Client.Utilities;
using UnityEngine;

namespace PlayGen.SUGAR.Unity.Utilities
{
    public class Logging : MonoBehaviour
    {
        [SerializeField]
        private bool _debug;
        [SerializeField]
        private bool _warning;
        [SerializeField]
        private bool _exception;

        private void Awake()
        {
            var logger = new Logger()
            {
                IsDebugEnabled = _debug,
                IsWarningEnabled = _warning,
                IsExceptionEnabled = _exception
            };
            
            Log.SetLogger(logger);
        }
    }
}
