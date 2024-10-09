using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CommandButtonChooser : MonoBehaviour
{
    Button _button;
    TextMeshProUGUI _text;
    
    void Awake() {
        _button = GetComponentInChildren<Button>();
        _text = GetComponentInChildren<TextMeshProUGUI>();
    }

    public void InitializeButton(Action callback, string label) {
        Clear();
        
        _button.onClick.AddListener(() => callback());
        _text.SetText(label);
        gameObject.SetActive(true);
    }

    public void Clear(bool disable = false) {
        _button.onClick.RemoveAllListeners();
        _text.SetText("");
        
        if (disable)
            gameObject.SetActive(false);
    }
}