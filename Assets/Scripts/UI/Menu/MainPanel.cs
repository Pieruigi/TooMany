using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace TMOT.UI
{
    public class MainPanel : MonoBehaviour
    {
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

        public void QuitGame()
        {
            Application.Quit();
        }
    }
}