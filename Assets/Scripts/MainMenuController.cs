using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class MainMenuController : MonoBehaviour
{
    [SerializeField] TMP_Text versionNumber;

    bool startingGame;
    Coroutine startRoutine;

    private void Start()
    {
        startingGame = false;
        startRoutine = null;

        versionNumber.text = $"v{Application.version}";

        FadeController.instance.StartFade(0f, 1f);
    }

    public void StartGame()
    {
        if (!startingGame
            && !FadeController.instance.isFading
            && startRoutine == null)
        {
            startingGame = true;
            startRoutine = StartCoroutine(StartGameRoutine());
        }
    }

    IEnumerator StartGameRoutine()
    {
        FadeController.instance.StartFade(1f, 1.5f);

        while (FadeController.instance.isFading)
            yield return null;

        yield return new WaitForSeconds(1f);

        SceneManager.LoadScene(1);

        startRoutine = null;
    }

    public void Settings()
    {
        if (!startingGame && !FadeController.instance.isFading)
        {
            print("Open settings menu here");
        }
    }

    public void Credits()
    {
        if (!startingGame && !FadeController.instance.isFading)
        {
            print("Open credits menu here");
        }
    }
}
