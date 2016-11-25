﻿using UnityEngine;
using PlayGen.SUGAR.Client;
using PlayGen.SUGAR.Contracts.Shared;
using SUGAR.Unity;


namespace SUGAR.Unity
{
    [RequireComponent(typeof(AccountUnityClient))]
    public class SUGARManager : MonoBehaviour
    {
        [SerializeField] private string _baseAddress;

        public static ActorResponse CurrentUser { get; set; }

        public static SUGARClient SugarClient;

        void Awake()
        {
            SugarClient = new SUGARClient(_baseAddress); // hTTPhANDLER ?>?!
            DontDestroyOnLoad(this);
        }
    }
}