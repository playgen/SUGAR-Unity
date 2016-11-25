using System;
using System.Collections.Generic;
using System.Linq;
using PlayGen.SUGAR.Client;
using UnityEngine;

namespace SUGAR.Unity
{
    class AchievementUnityClient : MonoBehaviour
    {
        private AchievementClient _achievementClient;

        void Start()
        {
            _achievementClient = SUGAR.Client.Achievement;
        }

        public void GetAchievements()
        {
            var achievements = _achievementClient.GetGameProgress(SUGAR.GameId, SUGAR.CurrentUser.Id);
        }
    }
}
