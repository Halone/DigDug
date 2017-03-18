using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager: BaseManager<LevelManager> {
    #region Variables
    private enum MAP_TYPE {
        EMPTY,
        GRASS,
        STONE
    }

    private Dictionary<Vector2, MAP_TYPE> m_Map;
    #endregion

    #region Initialisation & Destroy
    protected override IEnumerator CoroutineStart() {
        yield return true;
        isReady = true;
    }
    #endregion

    #region Level Managment
    protected override void Play() {
        GenerateMap();
    }

    private void GenerateMap() {
        
    }
    #endregion
}