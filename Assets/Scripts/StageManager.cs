using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TMOT
{
    public static class StageManager
    {
        public const int MaxStage = 9; 
        
        static string prefsParamFormat = "MP_{0}_MD_{1}";



        static float[] redBots = new float[] { 1f, 1f, 1.1875f, 1.1875f, 1.1875f, 1.1875f, 1.375f, 1.375f, 1.375f, 1.5625f, 1.5625f, 1.5625f, 1.75f };
        static float[] blueBots = new float[] { 1f, 1.1875f, 1.1875f, 1.1875f, 1.375f, 1.375f, 1.375f, 1.5625f, 1.5625f, 1.5625f, 1.75f, 1.75f, 1.75f };

        static float[] supplyBots = new float[] { 1f, 1f, 1f, 1.1875f, 1.1875f, 1.375f, 1.375f, 1.375f, 1.5625f, 1.5625f, 1.5625f, 1.75f, 1.75f };

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

        public static int GetLastStage(int mapId, GameModeType modeId)
        {
            
            string param = string.Format(prefsParamFormat, 0, modeId);
            // I should probably change this with hidden achievement or stats on Steam
            int stage = PlayerPrefs.GetInt(param, 0);

            Debug.Log($"StageManager - Load - Param:{param}:{stage}");

            return stage;
        }

        public static void UpdateLastStage(int mapId, GameModeType modeId, int stage)
        {
            Debug.Log("StageManager - UpdateLastStage");
            string param = string.Format(prefsParamFormat, 0, modeId);
            
            // I should probably change this with hidden achievement or stats on Steam
            int savedStage = PlayerPrefs.GetInt(param, 0);
            Debug.Log($"StageManager - Load - Param:{param}:{savedStage}");
            if (stage <= savedStage) return;
            if (stage > MaxStage) return;

            Debug.Log($"StageManager - Save - Param:{param}:{stage}");
            PlayerPrefs.SetInt(param, stage);
            PlayerPrefs.Save();
        }
        
    }
}