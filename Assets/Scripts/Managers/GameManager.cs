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
        //while (MenuManager.instance == null) yield return false;
        //while (!MenuManager.instance.isReady) yield return false;
        Init();

        //while (!MenuManager.instance.isInit) yield return false;
        Menu();

        yield return true;
    }

    protected override void Destroy() {
        onInit  = null;
        onMenu  = null;
        onPlay  = null;
        onLoose = null;
        onWin   = null;

        base.Destroy();
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