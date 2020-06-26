using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SplashScreenManager : MonoBehaviour
{
    [Header("Settings")]
    public float fadeDuration;
    public float showDuration;
    public Image fadeScreen;

    private void Awake()
    {
        StartCoroutine(FadeScreen());
    }

    private IEnumerator FadeScreen()
    {
        Color fadeScreenColor = fadeScreen.color;

        for(float f = 1; f > 0; f -= Time.deltaTime / fadeDuration)
        {
            fadeScreenColor.a = f;
            fadeScreen.color = fadeScreenColor;

            yield return new WaitForSeconds(Time.deltaTime);
        }

        fadeScreenColor.a = 0;
        fadeScreen.color = fadeScreenColor;

        yield return new WaitForSeconds(showDuration);

        for (float f = 0; f < 1; f += Time.deltaTime / fadeDuration)
        {
            fadeScreenColor.a = f;
            fadeScreen.color = fadeScreenColor;

            yield return new WaitForSeconds(Time.deltaTime);
        }

        fadeScreenColor.a = 1;
        fadeScreen.color = fadeScreenColor;

        UnityEngine.SceneManagement.SceneManager.LoadScene(SettingsManager.LOBBY_SCENE);
    }
}
