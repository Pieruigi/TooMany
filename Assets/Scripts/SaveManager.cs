using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TMOT
{
    public class SaveManager : SingletonPersistent<SaveManager>
    {
        public const string ProgressParam = "Progress";

        public const int ProgressMax = 4;

        [SerializeField]
        int progress;

        public int GameProgress
        {
            get{ return progress; }
        }

        

        protected override void Awake()
        {
            base.Awake();

            progress = PlayerPrefs.GetInt(ProgressParam, 0);
        }

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        public void UpdateGameProgress()
        {
            if (progress == ProgressMax) return;

            progress++;
            PlayerPrefs.SetInt(ProgressParam, progress);
            PlayerPrefs.Save();
        }
    }
}