using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace TMOT.UI
{
    public class MainPanel : MonoBehaviour
    {
#if UNITY_WEBGL
        [SerializeField]
        GameObject exitButton;
#endif

#if UNITY_WEBGL
        void Awake()
        {
            Destroy(exitButton);
        }
#endif

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