using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TitleUI : MonoBehaviour
{
    public Button startButton;
    public Image startButtonLighter;
    public Button quitButton;
    public Texture2D mainCursor;

    private void OnEnable()
    {
        quitButton.onClick.AddListener(OnQuitPressed);
        startButton.onClick.AddListener(OnStartPressed);
        Cursor.SetCursor(mainCursor, new Vector2(24, 16), CursorMode.Auto);
    }

    private void OnDisable()
    {
        quitButton.onClick.RemoveAllListeners();
        startButton.onClick.RemoveAllListeners();
    }

    public void OnQuitPressed()
    {
        Debug.Log("OnQuitPressed");
        Application.Quit();
    }

    public void OnStartPressed()
    {
        Debug.Log("OnStartPressed");
        SceneManager.LoadScene("JLWarningScreen");
    }
}
