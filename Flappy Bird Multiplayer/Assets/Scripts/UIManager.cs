using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;

    [SerializeField] TextMeshProUGUI scoreText, finalScoreText, recordText;
    [SerializeField] GameObject gameOverWindow;

    private void Awake()
    {
        // Garante uma única instância
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject); // Evita múltiplos UIManagers
        }
    }

    public void UpdateScoreText()
    {
        scoreText.text = GameManager.instance.Score.ToString();
    }

    public void GameOver()
    {
        finalScoreText.text = GameManager.instance.Score.ToString();
        recordText.text = PlayerPrefs.GetInt("Record").ToString();
        gameOverWindow.SetActive(true);
        Time.timeScale = 0;
    }

    public void Restart()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}