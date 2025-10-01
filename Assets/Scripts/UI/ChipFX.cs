using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMOT;
using TMOT.UI;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public enum ChipFXState { NotSelectable, Selectable, Selected }

public class ChipFX : MonoBehaviour, IPointerDownHandler, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField]
    Sprite notSelectableSprite;

    [SerializeField]
    Sprite selectableSprite;

    [SerializeField]
    Sprite selectedSprite;

    ChipFXState state;
    public ChipFXState State
    {
        get{ return state; }
    }

    Image image;

    Vector3 originalPosition;

    StageUI stageUI;

    void Awake()
    {
        image = GetComponent<Image>();
        originalPosition = (transform as RectTransform).anchoredPosition;
        stageUI = GetComponentInParent<StageUI>();
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void SetState(ChipFXState state)
    {
        this.state = state;
        
        if(!image) image = GetComponent<Image>();

        switch (state)
        {
            case ChipFXState.NotSelectable:
                image.sprite = notSelectableSprite;
                break;
            case ChipFXState.Selectable:
                image.sprite = selectableSprite;
                break;
            case ChipFXState.Selected:
                image.sprite = selectedSprite;
                break;
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (state == ChipFXState.NotSelectable) return;
        stageUI.ReportPointerDown(this);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (state == ChipFXState.NotSelectable) return;
        if (stageUI.GetChipIndex(this) == GameManager.Instance.GameStage) return;
        (transform as RectTransform).DOShakeAnchorPos(.5f, 10, 10).SetLoops(-1, LoopType.Yoyo);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        
        (transform as RectTransform).DOKill();
        (transform as RectTransform).anchoredPosition = originalPosition;
    }
}
