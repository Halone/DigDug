using System;
using System.Collections;

public class GameManager: Singleton<GameManager> {
    #region Variables
    public Action onInit;
    public Action onMenu;
    public Action onPlay;
    public Action onLoose;
    public Action onWin;
    #endregion

    #region Initialisation & Destroy
    protected override IEnumerator CoroutineStart() {
        while (
            UIManager.instance == null && 
            ScoreManager.instance == null &&
            LevelManager.instance == null &&
            SoundsManager.instance == null
        ) yield return false;

        while (
            !UIManager.instance.isReady && 
            !ScoreManager.instance.isReady &&
            !LevelManager.instance.isReady &&
            !SoundsManager.instance.isReady
        ) yield return false;

        Init();

        while (
            !UIManager.instance.isInit &&
            !ScoreManager.instance.isInit &&
            !LevelManager.instance.isInit &&
            !SoundsManager.instance.isInit
        ) yield return false;

        UIManager.instance.onStartGame += Play;
        Menu();

        yield return true;
        isReady = true;
    }
    #endregion

    #region Game Events
    private void Init() {
        if (onInit != null) onInit();
    }

    private void Menu() {
        if (onMenu != null) onMenu();
    }

    private void Play() {
        if (onPlay != null) onPlay();
    }

    private void Loose() {
        if (onLoose != null) onLoose();
    }

    private void Win() {
        if (onWin != null) onWin();
    }
    #endregion
}