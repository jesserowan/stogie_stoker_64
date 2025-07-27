using System;
using UnityEngine;
using UnityEngine.UI;

public class RuntimeUI : MonoBehaviour
{
    public Image image;

    public Sprite winSprite;
    public Sprite loseSprite;

    private void Start()
    {
        image.sprite = null;
    }

    public void OnWin()
    {
        Debug.Log("OnWin");
        image.sprite = winSprite;
    }

    public void OnLose()
    {
        image.sprite = loseSprite;
    }
}
