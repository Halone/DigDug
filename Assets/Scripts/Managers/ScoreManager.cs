using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ScoreManager: BaseManager<ScoreManager> {
    #region Variables
    private const string PATH_JSON          = "json/";
    private const string NAME_FILE_SCORE    = "highscores";
    private const string FIELD_NAME         = "name";
    private const string FIELD_SCORE        = "score";
    private const int NB_HIGHSCORE          = 5;

    private List<string> m_HighscoreNames;
    private List<int> m_HighscoreScores;
    #endregion

    protected override IEnumerator CoroutineStart() {
        m_HighscoreNames    = new List<string>();
        m_HighscoreScores   = new List<int>();

        TextAsset l_JsonScore = Resources.Load(PATH_JSON + NAME_FILE_SCORE) as TextAsset;
        if (l_JsonScore == null) Debug.LogError(NAME_FILE_SCORE + " not found.");

        JSONObject l_ScoreData = new JSONObject(l_JsonScore.ToString());
        for (int i = 1; i < (NB_HIGHSCORE + 1); i++) {
            JSONObject l_ScoreDatad = l_ScoreData.GetField(i.ToString());

            m_HighscoreNames.Add(l_ScoreDatad.GetField(FIELD_NAME).str);
            m_HighscoreScores.Add((int)l_ScoreDatad.GetField(FIELD_SCORE).f);
        }

        yield return true;
        isReady = true;
    }

    public bool IsNewHighscore (int p_Score) {
        foreach (int l_Score in m_HighscoreScores) {
            if (p_Score > l_Score) return true;
        }
            
        return false;
    }

    public void AddNewHighScore(int p_Score, string p_PlayerName) {
        KeyValuePair<int, string> l_Pair = new KeyValuePair<int, string>(p_Score, p_PlayerName);
        KeyValuePair<int, string> l_TempPair;

        for (int i = 0; i< NB_HIGHSCORE; i++) {
            if (l_Pair.Key > m_HighscoreScores[i]) {
                l_TempPair = new KeyValuePair<int, string>(m_HighscoreScores[i], m_HighscoreNames[i]);
                m_HighscoreScores[i]    = l_Pair.Key;
                m_HighscoreNames[i]     = l_Pair.Value;
                l_Pair                  = l_TempPair;
            }
        }
    }

    public KeyValuePair<string, string> GetFormattedLeaderboard() {
        string l_LeaderboardStrName  = "";
        string l_LeaderboardStrScore = "";

        for (int i = 0; i < NB_HIGHSCORE; i++) {
            l_LeaderboardStrName += (i+1) + ": " + m_HighscoreNames[i] + "\n \n";
            l_LeaderboardStrScore += "Score : " + m_HighscoreScores[i] + "\n \n";
        }

        return new KeyValuePair<string, string>(l_LeaderboardStrName, l_LeaderboardStrScore);
    }
}