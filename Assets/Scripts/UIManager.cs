using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;

public class UIManager : MonoBehaviour {
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
    #endregion
    #region EndScreen
    [Header("EndScreenElem")]
    public Text Title;
    public Text Score;
    #endregion
    #endregion

    // Use this for initialization
    void Start () {
        ChangeScreen(MainScreen);
    }

    #region UIManagement
    private void ChangeScreen(GameObject p_NewScreen)
    {
        if(m_CurrentScreen != null) m_CurrentScreen.SetActive(false);
        p_NewScreen.SetActive(p_NewScreen);
        m_CurrentScreen = p_NewScreen;
    }

    private void ShowEndScreen(bool p_Win)
    {
        Title.text = (p_Win) ? "You Win" :"You Loose";
        Score.text = m_Score.ToString();

        //
        ChangeScreen(EndScreen);
    }
    #endregion

    #region ButtonMethodes
    public void OnClickPlay()
    {
        if(onStartGame != null)
            onStartGame.Invoke();

        m_CurrentScreen.SetActive(false);
        m_CurrentScreen = null;
    }

    public void OnClickShowLeaderboard()
    {
        if (onShowLeaderboard != null)
            onShowLeaderboard.Invoke();

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
        m_Score = 0;
        ShowEndScreen(true);
    }

    public void temp_ShowWinBigScore()
    {
        m_Score = 100;
        ShowEndScreen(true);
    }

    public void temp_ShowLoose()
    {
        ShowEndScreen(false);
    }
}
