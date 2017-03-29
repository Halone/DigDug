﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager: BaseManager<LevelManager> {
    #region Variables
    private const string PATH_JSON          = "JSON/";
    private const string PATH_PREFABS       = "Prefabs/";
    private const string NAME_FILE_LEVEL    = "level";
    private const string NAME_FILE_COLLIDER = "TileCollider";
    private const string NAME_FILE_PLAYER   = "Player";
    private const string FIELD_SIZE_X       = "size_X";
    private const string FIELD_SIZE_Y       = "size_Y";
    private const string FIELD_MAP          = "map";
    private const string FIELD_POS          = "position";
    private const string FIELD_TYPE         = "type";
    private const string FIELD_TEXTURES     = "textures";
    private const float UNIT_TEXTURE        = 0.0625f;

    private int m_MapSizeX;
    private int m_MapSizeY;
    private Dictionary<Vector2, TILE_TYPE> m_Model;
    private Dictionary<TILE_TYPE, Vector2[]> m_Textures;
    private List<Vector3> m_VerticesMesh;
    private List<int> m_TrianglesMesh;
    private List<Vector2> m_UV;
    private List<Vector3> m_VerticesCollider;
    private List<int> m_TrianglesCollider;
    private Mesh m_World;
    private GameObject m_PrefabTileCollider;
    private GameObject m_PrefabPlayer;
    private Vector3 m_PosStartPlayer;

    public Action<Dictionary<Vector2, TILE_TYPE>> onUpdateCollider;
    #endregion

    #region Initialisation
    protected override IEnumerator CoroutineStart() {
        m_Model                 = new Dictionary<Vector2, TILE_TYPE>();
        m_Textures              = new Dictionary<TILE_TYPE, Vector2[]>();
        m_VerticesMesh          = new List<Vector3>();
        m_TrianglesMesh         = new List<int>();
        m_UV                    = new List<Vector2>();
        m_VerticesCollider      = new List<Vector3>();
        m_TrianglesCollider     = new List<int>();
        m_World                 = gameObject.transform.GetChild(0).GetComponent<MeshFilter>().mesh;
        m_PrefabTileCollider    = Resources.Load(PATH_PREFABS + NAME_FILE_COLLIDER) as GameObject;
        m_PrefabPlayer          = Resources.Load(PATH_PREFABS + NAME_FILE_PLAYER) as GameObject;
        m_PosStartPlayer        = new Vector3();

        yield return true;
        isReady = true;
    }

    protected override void Init() {
        TileCollider.onTileDestroy += UpdateMap;
        base.Init();
    }
    #endregion

    #region Level Managment
    protected override void Menu() {
        ClearLevel();
    }

    protected override void Play() {
        //DATA
        InitLevel();
        //VIEW
        CreatView();
        GenerateMesh();
        //COLLIDER
        CreatCollider();
        if (onUpdateCollider != null) onUpdateCollider(m_Model);
        //PLAYER
        Instantiate(m_PrefabPlayer, m_PosStartPlayer, Quaternion.identity, gameObject.transform);
        //CAMERA
        Camera.main.transform.position  = m_World.bounds.center + Vector3.back * 10;
        Camera.main.orthographicSize    = Mathf.Min(m_MapSizeX, m_MapSizeY) / 2;
        Camera.main.orthographicSize    /= Camera.main.pixelRect.width / Camera.main.pixelRect.height;

        List<Vector2>  l;
        GetPath(new Vector2(2, 10), out l);
    }

    private void UpdateWorld() {
        CreatView();
        GenerateMesh();
        if (onUpdateCollider != null) onUpdateCollider(m_Model);
    }

    #region Map Managment
    private void InitLevel() {
        #region Load Json
        TextAsset l_JsonFile = Resources.Load(PATH_JSON + NAME_FILE_LEVEL) as TextAsset;//only one level
        if (l_JsonFile == null) Debug.LogError(NAME_FILE_LEVEL + " not found.");
        JSONObject l_JsonLevel = new JSONObject(l_JsonFile.ToString());
        #endregion

        #region Sizes
        m_MapSizeX = (int)l_JsonLevel.GetField(FIELD_SIZE_X).f;
        m_MapSizeY = (int)l_JsonLevel.GetField(FIELD_SIZE_Y).f;
        #endregion

        #region Model
        List<JSONObject> l_JsonMap = l_JsonLevel.GetField(FIELD_MAP).list;
        foreach (JSONObject l_Tile in l_JsonMap) {
            m_Model.Add(JSONTemplates.ToVector2(l_Tile.GetField(FIELD_POS)), (TILE_TYPE)l_Tile.GetField(FIELD_TYPE).f);
        }
        #endregion

        #region Textures
        JSONObject l_JsonTextures   = l_JsonLevel.GetField(FIELD_TEXTURES);
        string[] l_TileTypes        = Enum.GetNames(typeof(TILE_TYPE));
        JSONObject l_Texture;
        Vector2[] l_Square;
        Vector2 l_BasePosition;
        foreach (string typeName in l_TileTypes) {
            l_Texture       = l_JsonTextures.GetField(typeName);
            l_Square        = new Vector2[4];
            l_BasePosition  = (l_Texture != null) ? JSONTemplates.ToVector2(l_Texture) : Vector2.zero;

            l_Square[0] = new Vector2(l_BasePosition.x * UNIT_TEXTURE, l_BasePosition.y * UNIT_TEXTURE + UNIT_TEXTURE);
            l_Square[1] = new Vector2(l_BasePosition.x * UNIT_TEXTURE + UNIT_TEXTURE, l_BasePosition.y * UNIT_TEXTURE + UNIT_TEXTURE);
            l_Square[2] = new Vector2(l_BasePosition.x * UNIT_TEXTURE + UNIT_TEXTURE, l_BasePosition.y * UNIT_TEXTURE);
            l_Square[3] = new Vector2(l_BasePosition.x * UNIT_TEXTURE, l_BasePosition.y * UNIT_TEXTURE);

            m_Textures.Add((TILE_TYPE)Enum.Parse(typeof(TILE_TYPE), typeName), l_Square);
        }
        #endregion

        #region Player
        m_PosStartPlayer.Set(Mathf.Floor(m_MapSizeX / 2), m_MapSizeY + 1, 0);
        #endregion
    }

    private void CreatView() {
        Vector2 l_Pos   = new Vector2();
        int cptTile     = 0;
        TILE_TYPE l_Type;

        for (int cptLine = 0; cptLine < m_MapSizeY; cptLine++) {
            for (int cptColumn = 0; cptColumn < m_MapSizeX; cptColumn++) {
                l_Pos.Set(cptColumn, cptLine);
                l_Type = GetTileType(l_Pos);

                if (l_Type != TILE_TYPE.EMPTY) {
                    BuildVerticesMesh(cptColumn, cptLine);
                    BuildUV(m_Textures[l_Type]);
                    BuildTrianglesMesh(cptTile);
                    cptTile += 4;
                }
            }
        }
    }

    private void BuildVerticesMesh(int p_X, int p_Y) {
        m_VerticesMesh.Add(new Vector3(p_X, p_Y, 0));
        m_VerticesMesh.Add(new Vector3(p_X + 1, p_Y, 0));
        m_VerticesMesh.Add(new Vector3(p_X + 1, p_Y - 1, 0));
        m_VerticesMesh.Add(new Vector3(p_X, p_Y - 1, 0));
    }

    private void BuildUV(Vector2[] p_Texture) {
        m_UV.Add(p_Texture[0]);
        m_UV.Add(p_Texture[1]);
        m_UV.Add(p_Texture[2]);
        m_UV.Add(p_Texture[3]);
    }

    private void BuildTrianglesMesh(int p_Iteration) {
        m_TrianglesMesh.Add(p_Iteration);
        m_TrianglesMesh.Add(p_Iteration + 1);
        m_TrianglesMesh.Add(p_Iteration + 3);
        m_TrianglesMesh.Add(p_Iteration + 1);
        m_TrianglesMesh.Add(p_Iteration + 2);
        m_TrianglesMesh.Add(p_Iteration + 3);
    }

    private void CreatCollider() {
        Vector2 l_Pos = new Vector2();
        TILE_TYPE l_Type;

        for (int cptLine = 0; cptLine < m_MapSizeY; cptLine++) {
            for (int cptColumn = 0; cptColumn < m_MapSizeX; cptColumn++) {
                l_Pos.Set(cptColumn, cptLine);
                l_Type = GetTileType(l_Pos);

                if (l_Type != TILE_TYPE.EMPTY) {
                    Instantiate(m_PrefabTileCollider, l_Pos, Quaternion.identity, gameObject.transform);
                }
            }
        }
    }

    private void GenerateMesh() {
        m_World.Clear();

        m_World.vertices    = m_VerticesMesh.ToArray();
        m_World.triangles   = m_TrianglesMesh.ToArray();
        m_World.uv          = m_UV.ToArray();

        m_VerticesMesh.Clear();
        m_TrianglesMesh.Clear();
        m_UV.Clear();

        m_World.Optimize();
        m_World.RecalculateNormals();
    }

    private void UpdateMap(Vector2 p_Pos) {
        if (m_Model.ContainsKey(p_Pos)) {
            m_Model[p_Pos] = TILE_TYPE.EMPTY;
            Debug.Log(m_Model[p_Pos]);
            UpdateWorld();
        }
    }
    #endregion

    #region Utils
    private TILE_TYPE GetTileType(Vector2 p_Pos) {
        TILE_TYPE l_Type;

        if (m_Model.TryGetValue(p_Pos, out l_Type)) return l_Type;
        else return TILE_TYPE.EMPTY;
    }

    private void ClearLevel() {
        m_Model.Clear();
        m_Textures.Clear();
        m_VerticesMesh.Clear();
        m_TrianglesMesh.Clear();
        m_UV.Clear();
        m_VerticesCollider.Clear();
        m_TrianglesCollider.Clear();
    }
    #endregion
    #endregion

    #region Pathfinding
    public bool GetPath(Vector2 m_Pos, out List<Vector2> m_Path)
    {
        Dictionary<Vector2, int> l_PropaMap = new Dictionary<Vector2, int>();
        List<Vector2> l_List = new List<Vector2>();
        l_List.Add(m_Pos);

        RecursiveMethode(l_PropaMap, l_List);
        print(l_PropaMap.Count);

        m_Path = new List<Vector2>();
        return true;
    }

    public Vector2 GetModelPos(GameObject m_Obj)
    {
        return new Vector2();
    }

    public Dictionary<Vector2, int> RecursiveMethode(Dictionary<Vector2, int> p_Map, List<Vector2> p_List, int p_Iter = 0)
    {
        List<Vector2> l_NextIter = new List<Vector2>();
        DIRECTION[] l_Directions = (DIRECTION[])Enum.GetValues(typeof(DIRECTION));
        TILE_TYPE l_Type;

        foreach (Vector2 l_Pos in p_List)
        {
            foreach (DIRECTION l_Direction in l_Directions)
            {
                Vector2 l_NextPos = GetNextCellPos(l_Pos, l_Direction);
                if(m_Model.TryGetValue(l_NextPos, out l_Type))
                {
                    if(l_Type == TILE_TYPE.EMPTY && !p_Map.ContainsKey(l_NextPos))
                    {
                        p_Map.Add(l_NextPos, p_Iter);
                        l_NextIter.Add(l_NextPos);
                    }
                }
            }
        }

        if (l_NextIter.Count > 0)
            return RecursiveMethode(p_Map, l_NextIter, p_Iter++);
        else
            return p_Map;
    }

    #region Utils
    private Vector2 GetNextCellPos(Vector2 p_CurrentCell, DIRECTION p_Direction)
    {
        Vector2 p_NextCellPos = p_CurrentCell;
        switch (p_Direction)
        {
            case DIRECTION.UP:
                p_NextCellPos.y++;
                break;
            case DIRECTION.RIGHT:
                p_NextCellPos.x++;
                break;
            case DIRECTION.BOTTOM:
                p_NextCellPos.y--;
                break;
            case DIRECTION.LEFT:
                p_NextCellPos.x--;
                break;
            default:
                Debug.LogError("Pathfing2D: Error Direction '" + p_Direction + "' not found");
                break;
        }
        return p_NextCellPos;

    }
    #endregion
    #endregion
}

public enum DIRECTION
{
    UP,
    RIGHT,
    BOTTOM,
    LEFT
}