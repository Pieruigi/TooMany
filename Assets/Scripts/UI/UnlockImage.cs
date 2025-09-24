using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using JetBrains.Annotations;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

namespace TMOT.UI
{
    public class UnlockImage : MonoBehaviour
    {
        [SerializeField]
        bool isInternalMenu;

        CanvasGroup canvasGroup;

        Tweener shakeTween;


        //Toggle toggle;


        void Awake()
        {
            canvasGroup = GetComponent<CanvasGroup>();
            // if (!isInternalMenu)
            // {
            //     var button = GetComponentInParent<Button>();
            //     button?.onClick.AddListener(() => { canvasGroup.DOFade(0f, .2f).SetEase(Ease.InOutQuad).onComplete += () => { shakeTween?.Kill(); }; });
            // }
            // else
            // {
            //     // Check toggle
            //     toggle = GetComponentInParent<Toggle>();
                
            //     toggle?.onValueChanged.AddListener((v) =>
            //     {
            //         if (!v) return;
            //         var index = toggle.group.transform.GetComponentsInChildren<Toggle>().ToList().IndexOf(toggle);

            //         if (index == SaveManager.Instance.GameProgress)
            //         {
            //             canvasGroup.DOFade(0f, .2f).SetEase(Ease.InOutQuad).onComplete += () => { shakeTween?.Kill(); };
            //         }  
            //     });
            // }
            
        }

        // Start is called before the first frame update
        void Start()
        {
            var hidden = !SaveManager.Instance.IsNewGameModeUnlocked() || (isInternalMenu && !ShowMarkOnToggle());

            if (hidden)
                canvasGroup.alpha = 0;
            else
            {
                InitInput();
                StartTween();
            }
                
                
        }



        // Update is called once per frame
        void Update()
        {

        }

        void OnEnable()
        {
            if (!SaveManager.Instance) return;

            if (!isInternalMenu)
            {
                if (!SaveManager.Instance.IsNewGameModeUnlocked())
                {
                    shakeTween?.Kill();
                    canvasGroup.alpha = 0;
                }
                
            }
        }

        void OnDisable()
        {
            
        }

        void InitInput()
        {
            if (!isInternalMenu)
            {
                // Menu button
                var button = GetComponentInParent<Button>();
                //button?.onClick.AddListener(() => { canvasGroup.DOFade(0f, .2f).SetEase(Ease.InOutQuad).onComplete += () => { shakeTween?.Kill(); }; });
            }
            else
            {
                // Toggles
                var toggle = GetComponentInParent<Toggle>();
                // Get current toggle index
                var index = toggle.group.transform.GetComponentsInChildren<Toggle>().ToList().IndexOf(toggle);


                if (index == SaveManager.Instance.GameProgress)
                {
                    Debug.Log($"TEST - Index:{index}, GameProgress:{SaveManager.Instance.GameProgress}");
                    toggle?.onValueChanged.AddListener((v) =>
                    {
                        if (!v) return;

                        canvasGroup.DOFade(0f, .2f).SetEase(Ease.InOutQuad).onComplete += () => { shakeTween?.Kill(); SaveManager.Instance.ResetNewGameModeUnlocked(); };
                    });
                }
                
            }
        }

        bool ShowMarkOnToggle()
        {
            if (!SaveManager.Instance.IsNewGameModeUnlocked()) return false;

            var toggle = GetComponentInParent<Toggle>();
            var index = toggle.group.transform.GetComponentsInChildren<Toggle>().ToList().IndexOf(toggle);
            Debug.Log($"ToggleIndex:{index}");

            if (index != SaveManager.Instance.GameProgress)
                return false;

            return true;
        }

        void StartTween()
        {
            shakeTween = (transform as RectTransform).DOShakePosition(duration: 1f, strength: 10, vibrato: 40, snapping: true, randomness: 90).SetLoops(-1);

            
        }

       
    }
}