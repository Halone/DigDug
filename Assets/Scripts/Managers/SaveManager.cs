using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class SaveManager : Singleton<SaveManager> {

    private const string PATH_JSON = "Jsons/";
    private const string NAME_HIGHCORES_JSON = "highscores";
    private const int NBR_HIGHSCORE = 5;

    private List<string> m_HighscoreNames;
    private List<int> m_HighscoreScores;

    protected override IEnumerator CoroutineStart()
    {
        yield return true;

        isReady = true;
        GetSavedData();
    }

    private void GetSavedData()
    {
        m_HighscoreNames = new List<string>();
        m_HighscoreScores = new List<int>();

        TextAsset l_JsonScore = Resources.Load(PATH_JSON + NAME_HIGHCORES_JSON) as TextAsset;
        if (l_JsonScore == null) Debug.LogError(NAME_HIGHCORES_JSON + " not found.");

        JSONObject l_ScoreData = new JSONObject(l_JsonScore.ToString());
        for(int i = 1; i < (NBR_HIGHSCORE+1); i++)
        {
            JSONObject l_ScoreDatad = l_ScoreData.GetField(i.ToString());

            m_HighscoreNames.Add(l_ScoreDatad.GetField("name").str);
            m_HighscoreScores.Add(Convert.ToInt32(l_ScoreDatad.GetField("score").i));
        }
    }

    public bool IsNewHighscore(int p_Score)
    {
        foreach (int l_Score in m_HighscoreScores)
            if (p_Score > l_Score)
                return true;
            
        return false;
    }

    public void AddNewHighScore(int p_Score, string p_PlayerName)
    {
        KeyValuePair<int, string> l_Pair = new KeyValuePair<int, string>(p_Score, p_PlayerName);
        KeyValuePair<int, string> l_TempPair;

        foreach (string l in m_HighscoreNames)
            print("Avant " + l);

        for (int i = 0; i< NBR_HIGHSCORE; i++)
        {
            if (l_Pair.Key > m_HighscoreScores[i])
            {
                l_TempPair = new KeyValuePair<int, string>(m_HighscoreScores[i], m_HighscoreNames[i]);
                m_HighscoreScores[i] = l_Pair.Key;
                m_HighscoreNames[i]  = l_Pair.Value;

                l_Pair = l_TempPair;
            }
        }

        foreach (string l in m_HighscoreNames)
            print("Apres " + l);
    }

    public KeyValuePair<string, string> GetFormattedLeaderboard()
    {
        string l_LeaderboardStrName  = "";
        string l_LeaderboardStrScore = "";
        for (int i = 0; i < NBR_HIGHSCORE; i++)
        {
            l_LeaderboardStrName += (i+1) + ": " + m_HighscoreNames[i] + "\n \n";
            l_LeaderboardStrScore += "Score : " + m_HighscoreScores[i] + "\n \n";
        }

        return new KeyValuePair<string, string>(l_LeaderboardStrName, l_LeaderboardStrScore);
    }

}