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

    #region Initialisation
    protected override IEnumerator CoroutineStart() {
        while (UIManager.instance == null || LevelManager.instance == null) yield return false;
        while (!UIManager.instance.isReady || !LevelManager.instance.isReady) yield return false;
        Init();

        while (!UIManager.instance.isInit || !LevelManager.instance.isInit) yield return false;
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