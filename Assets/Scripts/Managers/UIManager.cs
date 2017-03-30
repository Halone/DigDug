using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System;

public class UIManager : BaseManager<UIManager> {
    #region Variables
    private GameObject m_CurrentScreen;
    private int m_Score;
    
    public Action onStartGame;
    public Action onEndGame;

    #region Screen
    [Header("Screen")]
    public GameObject MainScreen;
    public GameObject EndScreen;
    public GameObject LeaderboardScreen;
    public GameObject HUD;
    #endregion

    #region EndScreen
    [Header("HUD_Elem")]
    public Text HUD_Score;

    [Header("Leaderboard_Elem")]
    public Text Leaderboard_DispayName;
    public Text Leaderboard_DispayScore;

    [Header("EndScreen_Elem")]
    public Text End_Title;
    public Text End_Score;
    public Text End_NewName;
    public GameObject AddNewHighScorePanel;
    #endregion
    #endregion
    
    #region Init & Base Methodes
    protected override IEnumerator CoroutineStart() {
        isReady = true;
        yield return true;
    }
    protected override void Init()
    {
        base.Init();
        Player.onDeath += ShowEndScreen;
        LevelManager.instance.onAllEnemiesDyed += ShowEndScreen;
    }
    protected override void Menu() {
        ChangeScreen(MainScreen);
    }

    protected override void Play() {
        m_Score = 0;
        ChangeScreen(HUD);
        UpdateScore(0);
    }
    #endregion

    #region UIManagement
    public void UpdateScore(int p_ScoreIncrement = 1) {
        m_Score         += p_ScoreIncrement;
        HUD_Score.text  = "Score: " + m_Score;
    }

    private void ChangeScreen(GameObject p_NewScreen) {
        if(m_CurrentScreen != null) m_CurrentScreen.SetActive(false);

        p_NewScreen.SetActive(p_NewScreen);
        m_CurrentScreen = p_NewScreen;
    }

    private void ShowEndScreen(bool p_Win) {
        if (onEndGame != null)
            onEndGame();

        End_Title.text = (p_Win) ? "You Win" : "You Loose";
        End_Score.text = m_Score.ToString();

        AddNewHighScorePanel.SetActive(ScoreManager.instance.IsNewHighscore(m_Score) && p_Win);
        ChangeScreen(EndScreen);
    }
    #endregion

    #region ButtonMethodes
    public void OnClickPlay() {
        if (onStartGame != null) onStartGame();
    }

    public void OnClickShowLeaderboard() {
        KeyValuePair<string, string> l_LeaderboardFormattedData = ScoreManager.instance.GetFormattedLeaderboard();
        Leaderboard_DispayName.text                             = l_LeaderboardFormattedData.Key;
        Leaderboard_DispayScore.text                            = l_LeaderboardFormattedData.Value;

        ChangeScreen(LeaderboardScreen);
    }

    public void OnClickEndScreenPlay() {
        ChangeScreen(MainScreen);
    }

    public void OnClickSubmitNewScore() {
        ScoreManager.instance.AddNewHighScore(m_Score, End_NewName.text);
        ChangeScreen(MainScreen);
    }

    public void OnBackToMainMenu() {
        ChangeScreen(MainScreen);
    }
    #endregion

    public void temp_ShowWin() {
        ShowEndScreen(true);
    }

    public void temp_ShowLoose() {
        ShowEndScreen(false);
    }
}