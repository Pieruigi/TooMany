using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TMOT
{
    public static class StageManager
    {
        public const int MaxStage = 9; 
        
        static string prefsParamFormat = "Stage_{0}";

        public const float StepMultiplier = 0.18f;// 0.18 x 9 = 1.62


        static float[] redBots = new float[] { 1f, 1f, 1.1875f, 1.1875f, 1.1875f, 1.1875f, 1.375f, 1.375f, 1.375f, 1.5625f, 1.5625f, 1.5625f, 1.75f };
        static float[] blueBots = new float[] { 1f, 1.1875f, 1.1875f, 1.1875f, 1.375f, 1.375f, 1.375f, 1.5625f, 1.5625f, 1.5625f, 1.75f, 1.75f, 1.75f };

        static float[] supplyBots = new float[] { 1f, 1f, 1f, 1.1875f, 1.1875f, 1.375f, 1.375f, 1.375f, 1.5625f, 1.5625f, 1.5625f, 1.75f, 1.75f };

        static float[] spawnMuls = new float[] { .9f, 0.9f, 0.95f, 1f, 1.05f, 1.1f, 1.15f };

        public static float GetRedBotMul(int stage)
        {

            return stage >= redBots.Length ? redBots[redBots.Length - 1] : redBots[stage];
        }

        public static float GetBlueBotMul(int stage)
        {
            return stage >= blueBots.Length ? blueBots[blueBots.Length - 1] : blueBots[stage];
        }

        public static float GetSupplyBotMul(int stage)
        {
            return stage >= supplyBots.Length ? supplyBots[supplyBots.Length - 1] : supplyBots[stage];
        }

        public static float GetSpawnMul(int stage)
        {
            return stage >= spawnMuls.Length ? spawnMuls[spawnMuls.Length - 1] : spawnMuls[stage];
        }

        public static int GetLastStage(int mapId, GameModeType modeId)
        {

            string param = string.Format(prefsParamFormat, modeId);
            // I should probably change this with hidden achievement or stats on Steam
            //int stage = PlayerPrefs.GetInt(param, 0);
            int stage = SteamStatsManager.Instance.GetStageStat((int)modeId);

            Debug.Log($"StageManager - Load - Param:{param}:{stage}");

            return stage;
        }

        public static void UpdateLastStage(int mapId, GameModeType modeId, int stage)
        {
            // Stage max first
            

            Debug.Log("StageManager - UpdateLastStage");
            string param = string.Format(prefsParamFormat, modeId);

            // int stageMax = SteamStatsManager.Instance.GetStageMaxStat((int)modeId);
            // if (stageMax < stage)
            //     SteamStatsManager.Instance.UpdateStageMaxStat((int)modeId);

            // I should probably change this with hidden achievement or stats on Steam
            int savedStage = SteamStatsManager.Instance.GetStageStat((int)modeId);// PlayerPrefs.GetInt(param, 0);
            Debug.Log($"StageManager - Load - Param:{param}:{savedStage}");
            if (stage <= savedStage) return;
            //if (stage > MaxStage) return;

            Debug.Log($"StageManager - Save - Param:{param}:{stage}");
            SteamStatsManager.Instance.UpdateStageStat((int)modeId);
            //PlayerPrefs.SetInt(param, stage);
            PlayerPrefs.Save();
        }
        
    }
}