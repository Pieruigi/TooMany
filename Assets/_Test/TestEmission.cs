using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestEmission : MonoBehaviour
{
    [SerializeField]
    Material material;

    [SerializeField]
    Color redColor, blueColor;

    bool isRed = false;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            Vector4 color = blueColor; 
            if (!isRed)
            {
                color = redColor;
            }
            color *= 6;
            material.SetVector("_BaseColor", color);
            isRed = !isRed;
        }
    }
}
