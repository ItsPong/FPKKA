using UnityEngine;
using UnityEngine.UI;

public class GameTimer : MonoBehaviour
{
    public Text timerText; 
    private float playTime = 0f;
    private bool isPlayerAlive = true;

    void Update()
    {
        if (isPlayerAlive)
        {
            playTime += Time.deltaTime;
            int minutes = Mathf.FloorToInt(playTime / 60F);
            int seconds = Mathf.FloorToInt(playTime % 60F);
            timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
        }
    }

    public void PlayerDied()
    {
        isPlayerAlive = false;
    }
}
