using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TMOT
{
    public class GameMode6 : GameMode
    {

        void Update()
        {
            if (GameManager.Instance.GameState != GameState.Playing) return;

            if (Input.GetKeyDown(KeyCode.E))
                GameManager.Instance.ReportPlayerIsWinner();
        }

        protected override void StartGameMode()
        {
            PlayerController.Instance.SetState(PlayerState.Prey);
        }
        

    }
}