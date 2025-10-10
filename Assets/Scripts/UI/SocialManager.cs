using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TMOT.UI
{
    public class SocialManager : MonoBehaviour
    {
        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        public void OpenDiscord()
        {
            Application.OpenURL("https://discord.gg/zjWr8VSWaD");
        }

        public void OpenTwitter()
        {
            Application.OpenURL("https://x.com/TheCreepyDev");
        }
    }
    
}
