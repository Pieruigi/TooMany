using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using TMOT;
using UnityEngine;

public class TestHitUI : MonoBehaviour
{
    [SerializeField]
    GameObject hitText;

    void Awake()
    {
        hitText.SetActive(false);
    }
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnEnable()
    {
        MonsterController.OnHitPlayer += HandleOnHitPlayer;
    }

    void OnDisable()
    {
        MonsterController.OnHitPlayer -= HandleOnHitPlayer;
    }

    private void HandleOnHitPlayer(MonsterController monsterController)
    {
        ShowHitText(monsterController).Forget();
    }

    private async UniTaskVoid ShowHitText(MonsterController monsterController)
    {
        if (hitText.activeSelf) return;
        hitText.SetActive(true);
        await UniTask.Delay(2000);
        hitText.SetActive(false);
    }
}
