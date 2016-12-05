using System;
using PlayGen.SUGAR.Common.Shared.Utilities;

namespace PlayGen.SUGAR.Unity.Utilities
{
    public class Logger : ILogger
    {
        public bool IsDebugEnabled { get; set; }

        public bool IsWarningEnabled { get; set; }

        public bool IsExceptionEnabled { get; set; }
        
        public void Debug(string message)
        {
            if (IsDebugEnabled)
            {
                UnityEngine.Debug.Log(message);
            }
        }

        public void Warning(string message)
        {
            if (IsWarningEnabled)
            {
                UnityEngine.Debug.LogWarning(message);
            }
        }

        public void Exception(Exception exception)
        {
            if (IsExceptionEnabled)
            {
                UnityEngine.Debug.LogException(exception);
            }
        }
    }
}