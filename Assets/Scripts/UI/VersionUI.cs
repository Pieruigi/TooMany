using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace TMOT.UI
{
    public class VersionUI : MonoBehaviour
    {

        void Awake()
        {
            GetComponent<TMP_Text>().text = Application.version; 
            
        }

        // Start is called before the first frame update
        void Start()
        {
            
        }

        // Update is called once per frame
        void Update()
        {
            
        }
    }
}