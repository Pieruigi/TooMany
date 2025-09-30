using System.Collections;
using System.Collections.Generic;
using Steamworks;
using UnityEngine;

namespace TMOT
{
    public enum SteamAchievementId { BEAT_GM_1 }

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

            if (Input.GetKeyDown(KeyCode.R))
            {
                ResetAchievement(SteamAchievementId.BEAT_GM_1.ToString());
            }
#endif

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
            achievementId = "NEW_ACHIEVEMENT_18_0";
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
        }

        // Verifica se un achievement è già sbloccato
        public bool IsAchievementUnlocked(string achievementId)
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
        public void ResetAchievement(string achievementId)
        {
            if (!SteamManager.Initialized) return;

            SteamUserStats.ClearAchievement(achievementId);
            SteamUserStats.StoreStats();
            Debug.Log($"Achievement resettato: {achievementId}");
        }
        
    }
}