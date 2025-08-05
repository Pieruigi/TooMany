using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace TMOT.UI
{
    public class MainPanel : MonoBehaviour
    {
        [SerializeField]
        Button playButton;

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        public void PlayGame()
        {
            GameManager.Instance.PlayGame();    
        }
    }
}