using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameoverWindow : MonoBehaviour
{
    [SerializeField] GameObject gameOverWindowObject; // todo make band aid fix for making show() work better

    private Text scoreText;
    private Text highscoreText;

    private void Awake()
    {
        scoreText = transform.Find("gameOverScoreText").GetComponent<Text>(); //string referenced
        highscoreText = transform.Find("highscoreText").GetComponent<Text>();
    }

    private void Start()
    {
        Player.GetInstance().OnDeath += PopUpOnDeath;
        Hide(); // this ahs to be in start, so show() can actually work
    }

    private void PopUpOnDeath(object sender, System.EventArgs e)
    {
        int currentScore = Level.GetInstance().GetPipesPassed();
        if ( currentScore > PlayerPrefs.GetInt("highscore"))
        {
            SaveHighscore(currentScore);
        }
        scoreText.text = currentScore.ToString(); // this session's score
        highscoreText.text = PlayerPrefs.GetInt("highscore").ToString();
        Show();
    }

    private void SaveHighscore(int currentScore)
    {
        PlayerPrefs.SetInt("highscore", currentScore);
        PlayerPrefs.Save();
    }

    private void Hide()
    {
        gameOverWindowObject.SetActive(false);
    }

    private void Show()
    {
        gameOverWindowObject.SetActive(true);
    }

    public void OnRestartButtonClick()
    {
        SoundManager.PlaySound(SoundManager.SoundType.ButtonClick);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void OnMainMenuButtonClick()
    {
        SoundManager.PlaySound(SoundManager.SoundType.ButtonClick);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 1);
    }
}
