using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;

public class DemoManager : SingletonPersistent<DemoManager>
{
    [SerializeField]
    GameObject panel;

    protected override void Awake()
    {
        base.Awake();

#if !DEMO
        Destroy(gameObject);

#else
        panel.SetActive(false);
#endif
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public async UniTask Show(float time)
    {
        panel.SetActive(true);

        await UniTask.Delay(TimeSpan.FromSeconds(time));

        panel.SetActive(false);
    }
}
