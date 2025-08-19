using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace TMOT.UI
{
    public class NotInteractable : MonoBehaviour, IPointerClickHandler
    {
        [SerializeField]
        string message;

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }
        
        public void OnPointerClick(PointerEventData eventData)
        {
            if (GetComponent<Selectable>().interactable) return;

            if (string.IsNullOrEmpty(message))
                FindObjectOfType<MessageUI>().Show();
            else
                FindObjectOfType<MessageUI>().Show(message);
        }
    }
}