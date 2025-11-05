using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class MainMenuController : MonoBehaviour
{
    [SerializeField] Transform titleCamPos, menuCamPos;
    [SerializeField] float camSpeed;
    [SerializeField] private bool movingCamera = false;

    [SerializeField] GameObject mainMenu, shiftSelectMenu, controlsMenu, creditsMenu;
    [SerializeField] TMP_Text shiftSelectText;
    [SerializeField] TMP_Text versionNumber;
    [SerializeField] CanvasGroup selectShiftButtonCanvas;
    [SerializeField] CanvasGroup longNightButtonCanvas;
    [SerializeField] TMP_Text longNightButtonText;
    int shiftSelectNum;
    float longNightScore;

    [SerializeField] AudioSource clickAudioSource;
    [SerializeField] AudioClip clickAudio;

    bool startingGame;
    Coroutine startRoutine;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        startingGame = false;
        startRoutine = null;

        shiftSelectNum = 0;
        shiftSelectText.text = $"Shift: {shiftSelectNum + 1}";
        versionNumber.text = $"v{Application.version}";

        FadeController.instance.StartFade(0f, 3f);

        //PlayerPrefs.SetInt("maxShift", 5); //TODO: remove this from final build
        selectShiftButtonCanvas.alpha = DataController.instance.maxShiftNum == 0 ? 0.5f : 1f; //PlayerPrefs.GetInt("maxShift", 0) == 0 ? 0.5f : 1f;
        selectShiftButtonCanvas.interactable = DataController.instance.maxShiftNum >= 1; //PlayerPrefs.GetInt("maxShift", 0) >= 1;

        //PlayerPrefs.SetInt("longNightMode", 1); //TODO: remove this from final build
        //if (PlayerPrefs.GetInt("longNightMode") > 0)
        //    PlayerPrefs.SetInt("longNightMode", 1);
        longNightButtonCanvas.alpha = DataController.instance.longNightMode == 0 ? 0.5f : 1f;//PlayerPrefs.GetInt("longNightMode", 0) == 0 ? 0.5f : 1f;
        longNightButtonCanvas.interactable = DataController.instance.longNightMode == 1;//PlayerPrefs.GetInt("longNightMode", 0) == 1;
        longNightScore = DataController.instance.longNightScore;//PlayerPrefs.GetFloat("longNightScore", 0f);
        longNightScore = Mathf.RoundToInt(longNightScore);
        longNightButtonText.text = PlayerPrefs.GetInt("longNightMode") > 0 ? $"Long Night Mode ({longNightScore})" : "Long Night Mode";

        if (PlayerPrefs.GetInt("newGame", 0) == 0)
        {
            mainMenu.SetActive(false);
            shiftSelectMenu.SetActive(false);
            controlsMenu.SetActive(false);
            creditsMenu.SetActive(false);
            Camera.main.transform.position = titleCamPos.position;
            Camera.main.transform.rotation = titleCamPos.rotation;
        }
        else
        {
            mainMenu.SetActive(true);
            Camera.main.transform.position = menuCamPos.position;
            Camera.main.transform.rotation = menuCamPos.rotation;
        }
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0)
            && PlayerPrefs.GetInt("newGame", 0) == 0)
        {
            movingCamera = true;
        }

        if (movingCamera)
        {
            Camera.main.transform.position = Vector3.Lerp(Camera.main.transform.position, menuCamPos.position, camSpeed * Time.deltaTime);
            Camera.main.transform.rotation = Quaternion.Slerp(Camera.main.transform.rotation, menuCamPos.rotation, camSpeed * Time.deltaTime);
            PlayerPrefs.SetInt("newGame", 1);

            if (Vector3.Distance(Camera.main.transform.position, menuCamPos.position) <= 0.125f)
            {
                mainMenu.SetActive(true);
                movingCamera = false;
            }
        }

        if (Input.GetMouseButtonDown(0))
        {
            clickAudioSource.Stop();
            clickAudioSource.PlayOneShot(clickAudio);
        }
    }

    public void StartGame()
    {
        if (!startingGame
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
        if (!startingGame)
        {
            PlayerPrefs.SetInt("longNightMode", 2);
            StartGame();
        }
    }

    public void SelectShiftMenu()
    {
        mainMenu.SetActive(false);
        shiftSelectMenu.SetActive(true);
        controlsMenu.SetActive(false);
        creditsMenu.SetActive(false);
    }

    public void ModifyShiftNum(int shiftNum)
    {
        shiftSelectNum += shiftNum;
        shiftSelectNum = Mathf.Clamp(shiftSelectNum, 0, PlayerPrefs.GetInt("maxShift", 0));
        if (shiftSelectNum >= 4) { shiftSelectNum = PlayerPrefs.GetInt("maxShift", 0) - 1; }
        shiftSelectText.text = $"Shift: {shiftSelectNum + 1}";
    }

    public void SelectNight()
    {
        int shiftNum = shiftSelectNum;
        PlayerPrefs.SetInt("shiftNum", shiftNum);
        StartGame();
    }

    public void Settings()
    {
        if (!startingGame)
        {
            print("Open settings menu here");
        }
    }

    public void Controls()
    {
        if (!startingGame)
        {
            print("Open controls menu here");
            mainMenu.SetActive(false);
            shiftSelectMenu.SetActive(false);
            controlsMenu.SetActive(true);
            creditsMenu.SetActive(false);
        }
    }

    public void Credits()
    {
        if (!startingGame)
        {
            print("Open credits menu here");
            mainMenu.SetActive(false);
            shiftSelectMenu.SetActive(false);
            controlsMenu.SetActive(false);
            creditsMenu.SetActive(true);
        }
    }

    public void Back()
    {
        mainMenu.SetActive(true);
        shiftSelectMenu.SetActive(false);
        controlsMenu.SetActive(false);
        creditsMenu.SetActive(false);
    }

    public void Exit()
    {
        Application.Quit();
    }

    private void OnApplicationQuit()
    {
        PlayerPrefs.SetInt("newGame", 0);
    }
}
