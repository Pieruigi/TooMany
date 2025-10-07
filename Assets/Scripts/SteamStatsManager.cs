using System;
using Steamworks;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

namespace TMOT
{
    public class SteamStatsManager : SingletonPersistent<SteamStatsManager>
    {


        protected override void Awake()
        {
            base.Awake();

            InitializeSteamStats();

#if UNITY_EDITOR
            DebugAllStats();
#endif
            
        }

        void Update()
        {
#if UNITY_EDITOR

            if (Input.GetKeyDown(KeyCode.T))
            {
                ResetAllStats();
            }
#endif
            
        }

        void OnEnable()
        {
            MonsterController.OnHitByPlayer += HandleOnMonsterHit;
            CustomDroneController.OnCustomDroneHit += HandleOnCustomDroneHit;
        }

        void OnDisable()
        {
            MonsterController.OnHitByPlayer -= HandleOnMonsterHit;
            CustomDroneController.OnCustomDroneHit -= HandleOnCustomDroneHit;
        }

        private void HandleOnMonsterHit(MonsterController monsterController)
        {
            string name = "DESTROYED_BOTS";
            GetStatInt(name, out int dbg);
            Debug.Log($"TEST - Beforre dest bots:{dbg}");
            IncrementStat(name, 1);
            GetStatInt(name, out dbg);
            Debug.Log($"TEST - After dest bots:{dbg}");

            // Get bot destroyed stat
            if (GetStatInt(name, out int count))
            {
                string format = "DESTROY_BOTS_{0}";
                Debug.Log($"TEST - Destroyed {count} bot(s).");
                if (count >= 100)
                    SteamAchievementManager.Instance.UnlockAchievement(string.Format(format, 1));
                if (count >= 250)
                    SteamAchievementManager.Instance.UnlockAchievement(string.Format(format, 2));
                if (count >= 500)
                    SteamAchievementManager.Instance.UnlockAchievement(string.Format(format, 3));

            }
        }

        private void HandleOnCustomDroneHit(CustomDroneController drone)
        {
            switch (drone.Type)
            {
                case CustomDroneType.Diamond:
                    HandleOnDiamondPicked();
                    break;
            }
        }

        void HandleOnDiamondPicked()
        {
            string name = "STOLEN_DIAMONDS";
            IncrementStat(name, 1);

            // Get bot destroyed stat
            if (GetStatInt(name, out int count))
            {
                string format = "STEAL_DMD_{0}";
                Debug.Log($"TEST - Stolen {count} bot(s).");
                if (count >= 100)
                    SteamAchievementManager.Instance.UnlockAchievement(string.Format(format, 1));
                if (count >= 250)
                    SteamAchievementManager.Instance.UnlockAchievement(string.Format(format, 2));
                if (count >= 500)
                    SteamAchievementManager.Instance.UnlockAchievement(string.Format(format, 3));

            }
        }

        private void InitializeSteamStats()
        {
            if (!SteamManager.Initialized) return;

            //SteamUserStats.RequestCurrentStats();
            Debug.Log("Steam Stats Manager inizializzato");
        }

        public int GetProgressStat()
        {
            if (!SteamManager.Initialized) return 0;

            GetStatInt("PROGRESS", out int progress);

            return progress;
        }

        public void UpdateProgressStat()
        {
            if (!SteamManager.Initialized) return;

            SetStat("PROGRESS", GetProgressStat() + 1);

#if UNITY_EDITOR
            DebugAllStats();
#endif
            

        }

        public int GetStageStat(int gameMode)
        {
            if (!SteamManager.Initialized) return 0;

            GetStatInt($"STAGE_{gameMode + 1}", out int stage);

            return stage;
        }

        public void UpdateStageStat(int gameMode)
        {
            if (!SteamManager.Initialized) return;



            SetStat($"STAGE_{gameMode + 1}", GetStageStat((int)gameMode) + 1);

#if UNITY_EDITOR
            DebugAllStats();
#endif
            

        }

        // ==================== SCRITTURA STATISTICHE ====================

        void SetStat(string statName, int value)
        {
            if (!SteamManager.Initialized) return;

            bool success = SteamUserStats.SetStat(statName, value);
            if (success)
            {
                SteamUserStats.StoreStats();
                Debug.Log($"Statistica {statName} aggiornata: {value}");
            }
            else
            {
                Debug.LogError($"Errore nell'aggiornare statistica: {statName}");
            }
        }

        void SetStat(string statName, float value)
        {
            if (!SteamManager.Initialized) return;

            bool success = SteamUserStats.SetStat(statName, value);
            if (success)
            {
                SteamUserStats.StoreStats();
                Debug.Log($"Statistica {statName} aggiornata: {value}");
            }
            else
            {
                Debug.LogError($"Errore nell'aggiornare statistica: {statName}");
            }
        }

        void IncrementStat(string statName, int increment = 1)
        {
            if (!SteamManager.Initialized) return;

            if (GetStatInt(statName, out int currentValue))
            {
                SetStat(statName, currentValue + increment);
            }
        }

        // ==================== LETTURA STATISTICHE ====================

        bool GetStatInt(string statName, out int value)
        {
            value = 0;

            if (!SteamManager.Initialized) return false;

            bool success = SteamUserStats.GetStat(statName, out value);
            if (!success)
            {
                Debug.LogWarning($"Statistica {statName} non trovata");
            }

            return success;
        }

        bool GetStatFloat(string statName, out float value)
        {
            value = 0f;

            if (!SteamManager.Initialized) return false;

            bool success = SteamUserStats.GetStat(statName, out value);
            if (!success)
            {
                Debug.LogWarning($"Statistica {statName} non trovata");
            }

            return success;
        }

        // ==================== UTILITY ====================

        void ResetAllStats(bool includeAchievements = false)
        {
            if (!SteamManager.Initialized) return;

            SteamUserStats.ResetAllStats(includeAchievements);
            SteamUserStats.StoreStats();
            Debug.Log("Statistiche resetate" + (includeAchievements ? " (inclusi achievement)" : ""));
        }

        void ForceStoreStats()
        {
            if (!SteamManager.Initialized) return;

            SteamUserStats.StoreStats();
            Debug.Log("Statistiche forzatamente salvate");
        }

        void RequestStatsRefresh()
        {
            if (!SteamManager.Initialized) return;

            //SteamUserStats.RequestCurrentStats();
            Debug.Log("Statistiche ricaricate da Steam");
        }
        
        public void DebugAllStats()
        {
            if (!SteamManager.Initialized)
            {
                Debug.LogError("Steam non inizializzato");
                return;
            }

            Debug.Log("=== STEAM STATS DEBUG ===");
            
            // Lista di tutte le stats che hai creato su Steamworks
            string[] statsToCheck = {
                "DESTROYED_BOTS",
                "STOLEN_DIAMONDS",
                "PROGRESS",
                "STAGE_1",
                "STAGE_2",
                "STAGE_3",
                "STAGE_4",
                "STAGE_5",
                "STAGE_6",
                
            };
            
            foreach (string statName in statsToCheck)
            {
                if (SteamUserStats.GetStat(statName, out int intValue))
                {
                    Debug.Log($"{statName}: {intValue}");
                }
                else if (SteamUserStats.GetStat(statName, out float floatValue))
                {
                    Debug.Log($"{statName}: {floatValue:F2}");
                }
                else
                {
                    Debug.LogWarning($"{statName}: Non trovata");
                }
            }
        }
    }
}