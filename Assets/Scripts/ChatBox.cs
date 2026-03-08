using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ChatBox : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _text;
    [SerializeField] private TextMeshProUGUI _title;
    [SerializeField] private Button _button;
    [SerializeField] private float _charsPerSecond = 30f;

    public event Action OnChatBoxNext;

    private bool _isAnimating = false;
    private Coroutine _typewriterCoroutine;

    void Start()
    {
        _button.onClick.AddListener(OnButtonClicked);
    }

    private void OnButtonClicked()
    {
        if (_isAnimating)
        {
            // Skip to full text on first click while animating
            StopCoroutine(_typewriterCoroutine);
            _text.maxVisibleCharacters = int.MaxValue;
            _isAnimating = false;
        }
        else
        {
            OnChatBoxNext?.Invoke();
        }
    }

    public void Show(string text, string title)
    {
        _title.text = title;
        _text.text = text;
        gameObject.SetActive(true);

        if (_typewriterCoroutine != null)
            StopCoroutine(_typewriterCoroutine);

        _typewriterCoroutine = StartCoroutine(AnimateText());
    }

    private IEnumerator AnimateText()
    {
        _isAnimating = true;
        _text.maxVisibleCharacters = 0;

        _text.ForceMeshUpdate();
        int totalChars = _text.textInfo.characterCount;

        float delay = 1f / _charsPerSecond;
        for (int i = 1; i <= totalChars; i++)
        {
            _text.maxVisibleCharacters = i;
            yield return new WaitForSeconds(delay);
        }

        _isAnimating = false;
    }

    public void Hide()
    {
        if (_typewriterCoroutine != null)
            StopCoroutine(_typewriterCoroutine);
        _isAnimating = false;
        gameObject.SetActive(false);
    }
}
