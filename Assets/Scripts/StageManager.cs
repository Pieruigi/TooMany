using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TMOT
{
    public static class StageManager
    {

        static float[] redBots = new float[] { 1f, 1f, 1.1875f, 1.1875f, 1.1875f, 1.1875f, 1.375f, 1.375f, 1.375f, 1.5625f };
        static float[] blueBots = new float[] { 1f, 1.1875f, 1.1875f, 1.1875f,  1.375f, 1.375f, 1.375f, 1.5625f, 1.5625f, 1.5625f };

        static float[] supplyBots = new float[] { 1f, 1f, 1f, 1.1875f, 1.1875f, 1.375f, 1.375f, 1.375f, 1.5625f, 1.5625f};

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
        
    }
}