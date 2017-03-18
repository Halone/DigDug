﻿using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using System;

public class UIManager : BaseManager<UIManager> {
    #region Variables
    private GameObject m_CurrentScreen;
    private int m_Score;

    #region Event
    [Header("Event")]
    public UnityAction onStartGame;
    public UnityAction onShowLeaderboard;
    public UnityAction onBackToMainMenu;
    #endregion
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

    // Use this for initialization
    #region Init & Base Methodes
    protected override IEnumerator CoroutineStart()
    {
        yield return true;

        isReady = true;
    }

    protected override void Menu()
    {
        base.Menu();
        ChangeScreen(MainScreen);
    }

    protected override void Play()
    {
        base.Play();
        m_Score = 0;
        ChangeScreen(HUD);

        UpdateScore(0);
    }
    #endregion

    #region UIManagement
    public void UpdateScore(int p_ScoreIncrement = 1)
    {
        m_Score += p_ScoreIncrement;

        HUD_Score.text = "Score: " + m_Score;
    }

    private void ChangeScreen(GameObject p_NewScreen)
    {
        if(m_CurrentScreen != null) m_CurrentScreen.SetActive(false);
        p_NewScreen.SetActive(p_NewScreen);
        m_CurrentScreen = p_NewScreen;
    }

    private void ShowEndScreen(bool p_Win)
    {
        End_Title.text = (p_Win) ? "You Win" :"You Loose";
        End_Score.text = m_Score.ToString();

        AddNewHighScorePanel.SetActive(SaveManager.instance.IsNewHighscore(m_Score) && p_Win);

        ChangeScreen(EndScreen);
    }
    #endregion

    #region ButtonMethodes
    public void OnClickPlay()
    {
        if(onStartGame != null)
            onStartGame.Invoke();
    }

    public void OnClickShowLeaderboard()
    {
        if (onShowLeaderboard != null)
            onShowLeaderboard.Invoke();

        KeyValuePair<string, string> l_LeaderboardFormattedData = SaveManager.instance.GetFormattedLeaderboard();
        Leaderboard_DispayName.text = l_LeaderboardFormattedData.Key;
        Leaderboard_DispayScore.text = l_LeaderboardFormattedData.Value;

        ChangeScreen(LeaderboardScreen);
    }

    public void OnClickEndScreenPlay()
    {
        if (onBackToMainMenu != null)
            onBackToMainMenu.Invoke();

        ChangeScreen(MainScreen);
    }

    public void OnClickSubmitNewScore()
    {
        SaveManager.instance.AddNewHighScore(m_Score, End_NewName.text);

        if (onBackToMainMenu != null)
            onBackToMainMenu.Invoke();

        ChangeScreen(MainScreen);
    }

    public void OnBackToMainMenu()
    {
        ChangeScreen(MainScreen);
    }
    #endregion

    public void temp_ShowWin()
    {
        ShowEndScreen(true);
    }

    public void temp_ShowLoose()
    {
        ShowEndScreen(false);
    }
}
