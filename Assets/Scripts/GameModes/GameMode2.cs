using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TMOT
{
    public class GameMode2 : GameMode
    {
        protected override void StartGameMode()
        {
            PlayerController.Instance.SetState(PlayerState.Prey);
        }
    }
}