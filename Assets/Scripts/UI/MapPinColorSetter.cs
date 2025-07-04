using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


namespace TMOT.UI
{
    public class MapPinColorSetter : MonoBehaviour
    {
        [SerializeField]
        Color[] colors;

        Image image;

        void Awake()
        {
            image = GetComponent<Image>();
            image.color = colors[0];
        }

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        public void SetColor(int index)
        {
            Debug.Log("TEST  - AAAAAAAAAAAAAAAAAAAAAAAAAA index:" + index);
            image.color = colors[index];
        }
    }
}