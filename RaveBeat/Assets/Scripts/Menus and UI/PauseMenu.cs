using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class PauseMenu : MonoBehaviour
{
    public static bool gameIsPaused = false;
    public static float pauseTime = 0.0f;
    private float pausedTime = 0.0f;

    public GameObject pauseMenuUI;
    public GameObject gameMenuUI;
    public GameObject scoreMenuUI;

    public GameObject pauseSelected;

    void Update() 
    {
        if (Input.GetButtonDown("GamePause"))
        {
            if (!gameIsPaused)
            {
                Pause();
            }
        }

    }

    public void Pause() 
    {
        if(!scoreMenuUI.activeSelf)
        {
            pausedTime = (float)AudioSettings.dspTime;
            gameMenuUI.SetActive(false);
            pauseMenuUI.SetActive(true);
            GameObject.Find("EventSystem").GetComponent<EventSystem>().SetSelectedGameObject(null);
            GameObject.Find("EventSystem").GetComponent<EventSystem>().SetSelectedGameObject(pauseSelected);
            gameIsPaused = true;
        }
    }

    public void Resume() 
    {
        pauseTime = (float)(AudioSettings.dspTime - pausedTime);
        pauseMenuUI.SetActive(false);
        gameMenuUI.SetActive(true);
        gameIsPaused = false;
    }

    public void Restart()
    {
        gameIsPaused = false;
        pauseMenuUI.SetActive(false);
        scoreMenuUI.SetActive(false);
        gameMenuUI.SetActive(true);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void Quit()
    {
        gameIsPaused = false;
        pauseMenuUI.SetActive(false);
        scoreMenuUI.SetActive(false);
        gameMenuUI.SetActive(true);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 1);
    }

}
