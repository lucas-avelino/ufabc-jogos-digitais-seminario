using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ScreenFlashManager : MonoBehaviour
{
    [SerializeField] private Image _flashImage;
    [SerializeField] private float _flashDuration = 0.2f;
    [SerializeField] private Color _flashColor = Color.white;

    private CanvasGroup _canvasGroup;
    private Coroutine _flashCoroutine;

    void Start()
    {
        if (_flashImage == null)
        {
            Debug.LogError("FlashImage is not assigned in ScreenFlashManager!");
            return;
        }

        _canvasGroup = _flashImage.GetComponent<CanvasGroup>();
        if (_canvasGroup == null)
        {
            _canvasGroup = _flashImage.gameObject.AddComponent<CanvasGroup>();
        }

        // Set initial state
        Color color = _flashColor;
        color.a = 0f;
        _flashImage.color = color;
    }

    /// <summary>
    /// Flash the screen with the configured color and duration
    /// </summary>
    public void Flash()
    {
        Flash(_flashDuration, _flashColor);
    }

    /// <summary>
    /// Flash the screen with a custom duration
    /// </summary>
    public void Flash(float duration)
    {
        Flash(duration, _flashColor);
    }

    /// <summary>
    /// Flash the screen with custom duration and color
    /// </summary>
    public void Flash(float duration, Color color)
    {
        // Stop previous flash if still running
        if (_flashCoroutine != null)
        {
            StopCoroutine(_flashCoroutine);
        }

        _flashCoroutine = StartCoroutine(FlashCoroutine(duration, color));
    }

    private IEnumerator FlashCoroutine(float duration, Color color)
    {
        float halfDuration = duration / 2f;

        // Fade in
        float elapsedTime = 0f;
        while (elapsedTime < halfDuration)
        {
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Lerp(0f, 1f, elapsedTime / halfDuration);
            Color flashColor = color;
            flashColor.a = alpha;
            _flashImage.color = flashColor;
            yield return null;
        }

        // Ensure we reach full alpha
        Color fullColor = color;
        fullColor.a = 1f;
        _flashImage.color = fullColor;

        // Fade out
        elapsedTime = 0f;
        while (elapsedTime < halfDuration)
        {
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Lerp(1f, 0f, elapsedTime / halfDuration);
            Color flashColor = color;
            flashColor.a = alpha;
            _flashImage.color = flashColor;
            yield return null;
        }

        // Ensure we reach zero alpha
        Color finalColor = color;
        finalColor.a = 0f;
        _flashImage.color = finalColor;
    }
}
