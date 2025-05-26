using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : Singleton<GameManager>
{
    public bool isPaused { get; set; }

    [SerializeField] private Image fadeImage;
    [SerializeField] private float fadeDuration = 1f;

    private bool isFading = false;
    private void Start()
    {
        if (fadeImage == null)
        {
            return;
        }

        // Set the fade image to be fully transparent at the start
        Color color = fadeImage.color;
        color.a = 0f;
        fadeImage.color = color;
        StartCoroutine(FadeLoadScene());

    }

    public void SwitchScene(string nextSceneName)
    {
        if (isFading)
            return;
        if (fadeImage == null)
        {
            SceneManager.LoadScene(nextSceneName);
            return;
        }
        StartCoroutine(FadeAndSwitchScene(nextSceneName));
    }
    private IEnumerator FadeAndSwitchScene(string nextSceneName)
    {
        isFading = true;
        // Fade out to black
        yield return StartCoroutine(Fade(0f, 1f));

        // Load new scene
        SceneManager.LoadScene(nextSceneName);
        isFading = false;

        yield return StartCoroutine(Fade(1f, 0f));
    }

    private IEnumerator FadeLoadScene()
    {
        isFading = true;
        // Fade out to black
        yield return StartCoroutine(Fade(1f, 0f));

        isFading = false;

    }


    private IEnumerator Fade(float startAlpha, float endAlpha)
    {
        float elapsed = 0f;
        Color color = fadeImage.color;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.unscaledDeltaTime;
            float alpha = Mathf.Lerp(startAlpha, endAlpha, elapsed / fadeDuration);
            color.a = alpha;
            fadeImage.color = color;
            yield return null;
        }

        color.a = endAlpha;
        fadeImage.color = color;
    }

    private void Update()
    {
        PauseInputCheck();
    }

    public void PauseGame()
    {
        isPaused = true;
        Time.timeScale = 0;
    }

    public void UnpauseGame()
    {
        isPaused = false;
        Time.timeScale = 1;
    }

    private void PauseInputCheck()
    {
        if (InputManager.PauseWasPressed)
        {
            if (!isPaused)
                PauseGame();
            else
                UnpauseGame();
        }
    }
}
