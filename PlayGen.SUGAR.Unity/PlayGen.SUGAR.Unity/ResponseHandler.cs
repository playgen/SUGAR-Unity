﻿using UnityEngine;
using System.Diagnostics;

namespace PlayGen.SUGAR.Unity
{
	public class ResponseHandler : MonoBehaviour
	{
		[Tooltip("If there are more than one responses queued to execute, this controls how much time will be allocated to executing them." +
				 "\nAt least one attempt to execute any pending response will be made per frame." +
				 "\nA value of 0 will execute all responses in available in each frame.")]
		[SerializeField]
		private int _responseMillisecondBudgetPerFrame;
		private bool _tryExecuteNextResponse;
		private readonly Stopwatch _stopwatch = new Stopwatch();

		private void Update()
		{
			if (Application.platform != RuntimePlatform.WebGLPlayer)
			{
				_stopwatch.Reset();

				while (_tryExecuteNextResponse
					&& (_responseMillisecondBudgetPerFrame == 0 || _stopwatch.ElapsedMilliseconds < _responseMillisecondBudgetPerFrame))
				{
					_stopwatch.Start();

					_tryExecuteNextResponse = SUGARManager.client.TryExecuteResponse();

					_stopwatch.Stop();
				}

				_tryExecuteNextResponse = true;
			}
		}
	}
}