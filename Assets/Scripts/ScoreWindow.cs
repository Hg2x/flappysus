using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreWindow : MonoBehaviour
{
    private Text scoreText;
    private void Awake()
    {
        scoreText = transform.Find("scoreText").GetComponent<Text>(); // todo clean this string reference
    }

    private void Update()
    {
        scoreText.text = Level.GetInstance().GetPipesPassed().ToString();
    }
}
