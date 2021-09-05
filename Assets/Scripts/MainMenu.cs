using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void OnStartButtonClick()
    {
        SoundManager.PlaySound(SoundManager.SoundType.ButtonClick);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
}
