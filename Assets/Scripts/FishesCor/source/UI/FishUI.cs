using System;
using TMPro;
using UnityEngine;

public class FishUI : MonoBehaviour
{
    public TextMeshProUGUI debug_text;
    public UIPauseMenu pause;

    void Awake()
    {
        G.ui = this;
    }

    void Start()
    {
        Reset();
    }

    void Reset()
    {
        pause.gameObject.SetActive(false);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            pause.Toggle();
        }
    }
}