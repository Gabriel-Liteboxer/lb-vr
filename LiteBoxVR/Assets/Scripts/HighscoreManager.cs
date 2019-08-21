using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HighscoreManager : MonoBehaviour
{
    [System.Serializable]
    public class Highscore
    {
        public string playerName;

        public int score;

        public int streak;

        public GameObject scoreObject;

        public void UpdateScore()
        {
            HighscoreDisplayer highDisplay = scoreObject.GetComponent<HighscoreDisplayer>();

            highDisplay.SetScore(playerName, score, streak);

        }
    }

    [System.Serializable]
    public class ScoreDifficultyLevel
    {
        [HideInInspector]
        public string difficultyName;

        public Highscore[] highscores;

    }

    [System.Serializable]
    public class SongHighScores
    {
        [HideInInspector]
        public string SongName;

        public SongConfig associatedSong;
        
        public ScoreDifficultyLevel[] scoreDifficultyLevels;

    }
    /*
    public class highScoreKey
    {
        public SongConfig associatedSong;

        public int difficulty;

    }*/

    public TMPro.TextMeshProUGUI header;

    public GameObject highScorePrefab;

    public float highscorePositionSpacing;

    public float highscorePositionOffset;

    public SongHighScores[] songHighScores;

    Dictionary<SongConfig, Dictionary<int, Highscore>> highScoreDict;

    List<Highscore> highscoreList;

    public string[] difficultyNames;

    [Header("Fake stuff")]

    public string[] FakeNames;

    private void Start()
    {
        GenerateFakeHighScores();

    }

    private void Update()
    {
        foreach (Highscore hs in highscoreList)
        {
            if(hs.scoreObject.activeInHierarchy)
                hs.scoreObject.transform.localScale = Vector3.Lerp(hs.scoreObject.transform.localScale, Vector3.one, Time.deltaTime*7);
        }
    }

    public void GenerateFakeHighScores()
    {
        highscoreList = new List<Highscore>();

        foreach (SongHighScores shs in songHighScores)
        {
            shs.SongName = shs.associatedSong.SongName;
            
            shs.scoreDifficultyLevels = new ScoreDifficultyLevel[3];

            for (int k = 0; k < shs.scoreDifficultyLevels.Length; k++)
            {
                shs.scoreDifficultyLevels[k] = new ScoreDifficultyLevel();

                shs.scoreDifficultyLevels[k].difficultyName = difficultyNames[k];

                shs.scoreDifficultyLevels[k].highscores = new Highscore[FakeNames.Length];

                for (int i = 0; i < FakeNames.Length; i++)
                {
                    shs.scoreDifficultyLevels[k].highscores[i] = new Highscore();

                    shs.scoreDifficultyLevels[k].highscores[i].scoreObject = GameObject.Instantiate(highScorePrefab, transform);

                    shs.scoreDifficultyLevels[k].highscores[i].playerName = FakeNames[i];

                    shs.scoreDifficultyLevels[k].highscores[i].score = Random.Range(4000, 9999);

                    shs.scoreDifficultyLevels[k].highscores[i].streak = Random.Range(0, 100);

                    shs.scoreDifficultyLevels[k].highscores[i].scoreObject.transform.localPosition = new Vector3(0, highscorePositionOffset + -i * highscorePositionSpacing, 0);

                    shs.scoreDifficultyLevels[k].highscores[i].UpdateScore();

                    highscoreList.Add(shs.scoreDifficultyLevels[k].highscores[i]);

                }

            }

        }

    }

    public void SetHighscorePage(SongConfig aSong, int aDifficultyLevel)
    {
        header.text = "Highscores (" + difficultyNames[aDifficultyLevel] + ")";

        foreach (Highscore hs in highscoreList)
        {
            hs.scoreObject.SetActive(false);
            hs.scoreObject.transform.localScale = new Vector3(1, 0, 1);
        }

        foreach (SongHighScores shs in songHighScores)
        {
            if (shs.associatedSong == aSong)
            {
                for (int i = 0; i < shs.scoreDifficultyLevels[aDifficultyLevel].highscores.Length; i++)
                {
                    shs.scoreDifficultyLevels[aDifficultyLevel].highscores[i].scoreObject.SetActive(true);

                }


            }

        }

    }
}
