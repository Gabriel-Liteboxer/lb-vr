using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class HighscoreDisplayer : MonoBehaviour
{
    public TextMeshProUGUI nameText;

    public TextMeshProUGUI scoreText;

    public TextMeshProUGUI streakText;

    public void SetScore (string aName, int aScore, int aStreak)
    {
        nameText.text = aName;

        scoreText.text = aScore.ToString();

        streakText.text = aStreak.ToString();

    }
}
