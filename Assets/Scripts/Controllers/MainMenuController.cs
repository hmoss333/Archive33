using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class MainMenuController : MonoBehaviour
{
    [SerializeField] GameObject mainMenu, creditsMenu;
    [SerializeField] TMP_Text versionNumber;
    [SerializeField] CanvasGroup longNightButtonCanvas;

    bool startingGame;
    Coroutine startRoutine;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        startingGame = false;
        startRoutine = null;

        versionNumber.text = $"v{Application.version}";

        FadeController.instance.StartFade(0f, 1f);

        PlayerPrefs.SetInt("longNightMode", 1); //TODO: remove this from final build
        if (PlayerPrefs.GetInt("longNightMode") > 0)
            PlayerPrefs.SetInt("longNightMode", 1);
        longNightButtonCanvas.alpha = PlayerPrefs.GetInt("longNightMode", 0) == 0 ? 0.5f : 1f;
        longNightButtonCanvas.interactable = PlayerPrefs.GetInt("longNightMode", 0) == 1;
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

    public void LongNightMode()
    {
        if (!startingGame && !FadeController.instance.isFading)
        {
            PlayerPrefs.SetInt("longNightMode", 2);
            StartGame();
        }
    }

    public void SelectNight(int shiftNum)
    {
        if (!startingGame && !FadeController.instance.isFading)
        {
            PlayerPrefs.SetInt("shiftNum", shiftNum);
            StartGame();
        }
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
            mainMenu.SetActive(false);
            creditsMenu.SetActive(true);
        }
    }

    public void Back()
    {
        mainMenu.SetActive(true);
        creditsMenu.SetActive(false);
    }
}
