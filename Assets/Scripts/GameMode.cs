using System.Collections;
using System.Collections.Generic;
using TMOT.UI;
using UnityEngine;

namespace TMOT
{
    public abstract class GameMode : Singleton<GameMode>
    {
        [SerializeField]
        GameObject gameUIPrefab;

        //GameObject gameUI;

        protected override void Awake()
        {
            base.Awake();
            Instantiate(gameUIPrefab);
            
        }

    
    }
}