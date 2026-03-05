using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ChatBox : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _text;
    [SerializeField] private Button _button;
    public event Action OnChatBoxNext;

    void Start()
    {
        _button.onClick.AddListener(() => OnChatBoxNext?.Invoke());
    }

    public void Show(string text)
    {
        _text.text = text;
        gameObject.SetActive(true);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }
}
