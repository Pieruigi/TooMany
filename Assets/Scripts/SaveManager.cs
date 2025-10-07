using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.LookDev;

namespace TMOT
{
    public class SaveManager : SingletonPersistent<SaveManager>
    {
        public const string ProgressParam = "Progress";
        public const string NewGameModeUnlockedParam = "GameModeUnlocked";
        public const int ProgressMax = 5;

        [SerializeField]
        int progress;

        public int GameProgress
        {
            get { return progress; }
        }



        protected override void Awake()
        {
            base.Awake();

            
            //progress = PlayerPrefs.GetInt(ProgressParam, 0);
        }

        // Start is called before the first frame update
        void Start()
        {
            progress = SteamStatsManager.Instance.GetProgressStat();
        }

        // Update is called once per frame
        void Update()
        {

        }

        public void UpdateGameProgress()
        {
            if (progress == ProgressMax) return;


            progress++;

            //PlayerPrefs.SetInt(ProgressParam, progress);
            SteamStatsManager.Instance.UpdateProgressStat();
            progress = SteamStatsManager.Instance.GetProgressStat();
            PlayerPrefs.SetInt(NewGameModeUnlockedParam, 1);
            PlayerPrefs.Save();
        }

        public void ResetNewGameModeUnlocked()
        {
            PlayerPrefs.SetInt(NewGameModeUnlockedParam, 0);
            PlayerPrefs.Save();
        }




        public bool IsNewGameModeUnlocked()
        {
            return PlayerPrefs.GetInt(NewGameModeUnlockedParam, 0) > 0;
        }

       
    }
}