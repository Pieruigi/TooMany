#if !UNITY_WEBGL
using System;
using System.Collections;
using System.Collections.Generic;
using Steamworks;
using Unity.Burst.CompilerServices;
using UnityEngine;

namespace TMOT
{
    public enum SteamAchievementId { BEAT_GM_1, BEAT_GM_2, BEAT_GM_3, BEAT_GM_4, BEAT_GM_5, BEAT_GM_6 }

    public class SteamAchievementManager : SingletonPersistent<SteamAchievementManager>
    {

        // Start is called before the first frame update
        void Start()
        {
            if (!SteamManager.Initialized)
            {
                Debug.LogWarning("Steam not initialized");
            }
            else
            {
                Debug.Log("Steam account:" + SteamFriends.GetPersonaName());
            }

            DebugAllAchievements();

        }

        // Update is called once per frame
        void Update()
        {
#if UNITY_EDITOR

            if (Input.GetKeyDown(KeyCode.U))
            {
                if (!IsAchievementUnlocked(SteamAchievementId.BEAT_GM_1.ToString()))
                    UnlockAchievement(SteamAchievementId.BEAT_GM_1.ToString());

            }

            if (Input.GetKeyDown(KeyCode.T))
            {
                HardResetAchievements();
            }

          
#endif

        }

        void OnEnable()
        {
            GameManager.OnStateChanged += HandleOnGameStateChanged;
        }

        void OnDisable()
        {
            GameManager.OnStateChanged -= HandleOnGameStateChanged;
        }

        private void HandleOnGameStateChanged(GameState oldState, GameState newState)
        {
            switch (newState)
            {
                case GameState.Winner:
                    // Update achievement
                    int gameMode = (int)GameManager.Instance.GameMode;
                    string achId = $"BEAT_GM_{gameMode + 1}";

                    Debug.Log($"TEST - Unlocking {achId}");
                    if (!IsAchievementUnlocked(achId)) UnlockAchievement(achId);

                    int gameStage = GameManager.Instance.GameStage;

                    string stgId;
                    // if (gameStage >= 4)
                    // {
                    //     stgId = $"GM_{gameMode + 1}_STG_5";
                    //     if (!IsAchievementUnlocked(stgId)) UnlockAchievement(stgId);
                    // }
                    if (gameStage >= 9)
                    {
                        stgId = $"GM_{gameMode + 1}_STG_10";
                        if (!IsAchievementUnlocked(stgId)) UnlockAchievement(stgId);
                    }
                    
                        
                    break;
            }
        }

        public void DebugAllAchievements()
        {
            if (!SteamManager.Initialized)
            {
                Debug.LogError("Steam non inizializzato");
                return;
            }

            uint numAchievements = SteamUserStats.GetNumAchievements();
            Debug.Log($"Numero achievement trovati: {numAchievements}");

            for (uint i = 0; i < numAchievements; i++)
            {
                string achievementId = SteamUserStats.GetAchievementName(i);
                bool achieved = SteamUserStats.GetAchievement(achievementId, out achieved);

                Debug.Log($"Achievement [{i}]: {achievementId} - Sbloccato: {achieved}");
            }
        }


        public void UnlockAchievement(string achievementId)
        {
            if (!SteamManager.Initialized)
            {
                Debug.LogWarning("Steam non inizializzato - Achievement non sbloccato: " + achievementId);
                return;
            }
            bool success = SteamUserStats.SetAchievement(achievementId);

            if (success)
            {
                SteamUserStats.StoreStats();
                Debug.Log($"Achievement sbloccato: {achievementId}");
            }
            else
            {
                Debug.LogError($"Errore nello sbloccare achievement: {achievementId}");
            }

            DebugAllAchievements();
        }

        // Verifica se un achievement è già sbloccato
        bool IsAchievementUnlocked(string achievementId)
        {
            if (!SteamManager.Initialized) return false;

            if (SteamUserStats.GetAchievement(achievementId, out bool achieved))
            {
                return achieved;
            }

            Debug.LogError($"Achievement non trovato: {achievementId}");
            return false;
        }

        // Reset achievement (per testing)
        void ResetAchievement(string achievementId)
        {
            if (!SteamManager.Initialized) return;

            SteamUserStats.ClearAchievement(achievementId);
            SteamUserStats.StoreStats();
            Debug.Log($"Achievement resettato: {achievementId}");
        }
        
        void HardResetAchievements()
        {
            if (!SteamManager.Initialized) return;
            
            // Prima resetta normalmente
            for (uint i = 0; i < SteamUserStats.GetNumAchievements(); i++)
            {
                string achievementId = SteamUserStats.GetAchievementName(i);
                SteamUserStats.ClearAchievement(achievementId);
            }
            SteamUserStats.StoreStats();
            
            // Poi forza una ricaricata
            SteamUserStats.ResetAllStats(true); // <-- il 'true' è importante!
            SteamAPI.RunCallbacks();
            
            Debug.Log("Hard reset completato");
        }
        
    }
}
#endif