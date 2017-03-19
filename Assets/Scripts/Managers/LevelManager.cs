using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager: BaseManager<LevelManager> {
    #region Variables
    private const string PATH_JSON          = "";
    private const string NAME_FILE_LEVEL    = "level";
    private const string FIELD_SIZE_X       = "size_X";
    private const string FIELD_SIZE_Y       = "size_Y";
    private const string FIELD_MAP          = "map";
    private const string FIELD_POS          = "position";
    private const string FIELD_TYPE         = "type";

    private enum TILE_TYPE {
        EMPTY,
        GRASS,
        STONE
    }

    private Dictionary<Vector2, TILE_TYPE> m_MapType;
    private int m_MapSize_X;
    private int m_MapSize_Y;
    private List<Vector3> m_VerticesMesh;
    private List<int> m_TrianglesMesh;
    private List<Vector2> m_UV;
    private List<Vector3> m_VerticesCollider;
    private List<int> m_TrianglesCollider;
    #endregion

    #region Initialisation
    protected override IEnumerator CoroutineStart() {
        TextAsset l_JsonLevel = Resources.Load(PATH_JSON + NAME_FILE_LEVEL) as TextAsset;
        if (l_JsonLevel == null) Debug.LogError(NAME_FILE_LEVEL + " not found.");
        else GenerateMap(new JSONObject(l_JsonLevel.ToString()));
        
        yield return true;
        isReady = true;
    }
    #endregion

    #region Level Managment
    protected override void Play() {
        
    }

    #region Map Managment
    private void GenerateMap(JSONObject p_JsonLevel) {
        m_MapType   = new Dictionary<Vector2, TILE_TYPE>();
        m_MapSize_X = (int)p_JsonLevel.GetField(FIELD_SIZE_X).f;
        m_MapSize_Y = (int)p_JsonLevel.GetField(FIELD_SIZE_Y).f;

        List<JSONObject> l_JsonMap = p_JsonLevel.GetField(FIELD_MAP).list;
        foreach (JSONObject l_Tile in l_JsonMap) {
            m_MapType.Add(JSONTemplates.ToVector2(l_Tile.GetField(FIELD_POS)), (TILE_TYPE)l_Tile.GetField(FIELD_TYPE).f);
        }
    }

    private void ConstructTiles() {
        Vector2 l_Pos = new Vector2();
        TILE_TYPE l_Type;

        for (int cptColumn = 0; cptColumn < m_MapSize_Y; cptColumn++) {
            for (int cptLine = 0; cptLine < m_MapSize_X; cptLine++) {
                l_Pos.Set(cptLine, cptColumn);
                l_Type = GetTileType(l_Pos);

                if (l_Type != TILE_TYPE.EMPTY) {
                    
                }
            }
        }
    }
    #endregion

    #region Utils
    private TILE_TYPE GetTileType(Vector2 p_Pos) {
        TILE_TYPE l_Type;

        if (m_MapType.TryGetValue(p_Pos, out l_Type)) return l_Type;
        else return TILE_TYPE.EMPTY;
    }
    #endregion
    #endregion
}