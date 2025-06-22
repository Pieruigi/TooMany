using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
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

    private async void HandleOnHitPlayer(MonsterController monsterController)
    {
        if (hitText.activeSelf) return;
        hitText.SetActive(true);
        await Task.Delay(2000);
        hitText.SetActive(false);
    }
}
