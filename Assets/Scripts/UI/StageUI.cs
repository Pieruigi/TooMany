using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace TMOT.UI
{
    public class StageUI : MonoBehaviour
    {
        [SerializeField]
        TMP_Text debugField;

        [SerializeField]
        List<ChipFX> chips = new List<ChipFX>();

        [SerializeField]
        TMP_Text modeField;

        [SerializeField]
        TMP_Text mapField;

        string modeStringFormat = "Mode: {0}";
        string mapStringFormat = "Map: {0}";

        string[] modes = { "Classic", "Thief", "Joke", "Switch", "Revenge", "PacMan" };
        string[] maps = { "Sewer", "Block", "Castle" };

        //bool skip = true;

        void Awake()
        {

        }

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        void OnEnable()
        {
            // if (skip)
            // {
            //     skip = false;
            //     return;
            // }

            InitChips();

            // int stage = StageManager.GetLastStage(GameManager.Instance.MapId, GameManager.Instance.GameMode);
            // debugField.text = (stage + 1).ToString();
            InitInfo();
        }

        void OnDisable()
        {

        }

        void ChipSetNotSelectableAll()
        {
            foreach (var c in chips)
                c.SetState(ChipFXState.NotSelectable);
        }

        void InitInfo()
        {
            modeField.text = string.Format(modeStringFormat, modes[(int)GameManager.Instance.GameMode]);
            mapField.text = string.Format(mapStringFormat, maps[GameManager.Instance.MapId]);
        }

        void InitChips()
        {
            int stage = GameManager.Instance.GameStage;
            for (int i = 0; i < chips.Count; i++)
            {
                if (i <= stage)
                    chips[i].SetState(ChipFXState.Selected);
                else
                    chips[i].SetState(ChipFXState.NotSelectable);
            }
        }

        public void ReportPointerDown(ChipFX chip)
        {
            var index = GetChipIndex(chip);

            GameManager.Instance.GameStage = index;

            for (int i = 0; i < chips.Count; i++)
            {
                if (i == index)
                {
                    chips[i].SetState(ChipFXState.Selected);
                }
                else
                {
                    if (i < index)
                    {
                        chips[i].SetState(ChipFXState.Selected);
                    }
                    else
                    {
                        if (chips[i].State == ChipFXState.Selected)
                            chips[i].SetState(ChipFXState.Selectable);    
                    }
                    
                    
                             
                }
            }

        }

        public int GetChipIndex(ChipFX chip)
        {
            return chips.IndexOf(chip);
        }
    }
}