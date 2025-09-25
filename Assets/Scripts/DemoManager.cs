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

    [SerializeField]
    List<GameObject> fields;

    List<string> texts = new List<string>()
    {
        "Not available in demo mode",
        "Speed is capped in demo mode"
    };

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

    void HideFieldAll()
    {
        foreach (var field in fields)
            field.SetActive(false);
    }

    public async UniTask Show(float time, int textId)
    {
        //panel.GetComponentInChildren<TMP_Text>().text = texts[textId];
        HideFieldAll();
        fields[textId].SetActive(true);

        panel.SetActive(true);

        await UniTask.Delay(TimeSpan.FromSeconds(time));

        panel.SetActive(false);
    }
}
