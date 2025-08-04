using System;
using System.Collections;
using UnityEngine;

public class SlowTimeWhenEnabled : MonoBehaviour
{
    private float speedCache;
    private Coroutine changingTime;
    [SerializeField] private Player player;
    
    private void OnEnable()
    {
        player = GameManager.Instance.player ?? FindFirstObjectByType<Player>();
        changingTime = StartCoroutine(SlowTime());
    }

    private void OnDisable()
    {
        if (changingTime != null) StopCoroutine(changingTime);
        changingTime = null;
        player.banJumping = false;
        GameManager.Instance.player.currentSpeed = speedCache;
        Time.timeScale = 1;
    }

    private IEnumerator SlowTime()
    {
        if (changingTime != null) yield break;
        speedCache = GameManager.Instance.player.currentSpeed;
        player.banJumping = true;
        Time.timeScale = 0.75f;
        while (Time.timeScale > 0.1f)
        {
            var newScale = Time.timeScale - Time.deltaTime * 5;
            Time.timeScale = Mathf.Clamp(newScale, 0, 0.5f);
            GameManager.Instance.player.currentSpeed = speedCache * newScale;
            yield return new WaitForSecondsRealtime(Time.unscaledDeltaTime);
        }
        Time.timeScale = 0.1f;
        changingTime = null;
    }
}
