﻿using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

[ExecuteInEditMode]
public class MeshCreator: MonoBehaviour {
    #region Variables
    private const string PATH_JSON_WRITTING     = @"Assets\Resources\JSON\";
    private const string PATH_PREFABS           = "Prefabs/";
    private const string PATH_JSON              = ".json";
    private const string NAME_FILE_LEVEL        = "level";
    private const string FIELD_SIZE_X           = "size_X";
    private const string FIELD_SIZE_Y           = "size_Y";
    private const string FIELD_MAP              = "map";
    private const string FIELD_POS              = "position";
    private const string FIELD_TYPE             = "type";
    private const string FIELD_TEXTURES         = "textures";
    private const string FIELD_ENEMIES          = "enemies";
    private const string FIELD_X                = "x";
    private const string FIELD_Y                = "y";
    private const string FIELD_POSITION         = "position";
    private const string FIELD_PLAYER           = "player";
    private const string NAME_FILE_ENEMY_PLONG  = "EnemyPlongeur";
    private const string NAME_FILE_ENEMY_DRAG   = "EnemyDragon";
    private const string NAME_FILE_PLAYER       = "Player";
    private const float UNIT_TEXTURE            = 0.0625f;

    private Mesh m_Mesh;
    private List<Vector3> m_Vertices;
    private List<int> m_Triangles;
    private List<Vector2> m_UV;
    private MeshCollider m_Collider;
    private List<Vector3> m_ColVertices;
    private List<int> m_ColTriangles;
    private Dictionary<string, GameObject> m_TypeToEnemy;
    private Dictionary<GameObject, string> m_Enemies;
    private GameObject m_PlayerObj;
    private Vector3 m_PlayerMapPos;
    private GameObject m_PrefabEnemyPlong;
    private GameObject m_PrefabEnemyDrag;
    private GameObject m_PrefabPlayer;

     [Range(12, 48)]
    public int MapSize_X;
    [Range(14, 56)]
    public int MapSize_Y;

    [HideInInspector]
    public List<Vector2> m_Pos;
    [HideInInspector]
    public List<TILE_TYPE> m_Type;
    [HideInInspector]
    public List<Vector2> m_Textures;
    #endregion

    #region Initialisation & Destroy
    void Start() {
        m_Mesh              = gameObject.GetComponent<MeshFilter>().sharedMesh;
        m_Vertices          = new List<Vector3>();
        m_Triangles         = new List<int>();
        m_UV                = new List<Vector2>();
        m_Collider          = gameObject.GetComponent<MeshCollider>();
        m_ColVertices       = new List<Vector3>();
        m_ColTriangles      = new List<int>();
        m_Enemies           = new Dictionary<GameObject, string>();
        m_PrefabEnemyPlong  = Resources.Load(PATH_PREFABS + NAME_FILE_ENEMY_PLONG) as GameObject;
        m_PrefabEnemyDrag   = Resources.Load(PATH_PREFABS + NAME_FILE_ENEMY_DRAG) as GameObject;
        m_PrefabPlayer      = Resources.Load(PATH_PREFABS + NAME_FILE_PLAYER) as GameObject;
        m_TypeToEnemy       = new Dictionary<string, GameObject>();

        m_TypeToEnemy.Add(NAME_FILE_ENEMY_PLONG, m_PrefabEnemyPlong);
        m_TypeToEnemy.Add(NAME_FILE_ENEMY_DRAG, m_PrefabEnemyDrag);
        m_TypeToEnemy.Add(NAME_FILE_PLAYER, m_PrefabPlayer);
    }
    #endregion

    #region Map Managment
    public void BuildMap(Vector2 p_Texture1, Vector2 p_Texture2) {
        if (m_Textures != null) m_Textures.Clear();
        else m_Textures = new List<Vector2>();
        m_Textures.Add(Vector2.zero);
        m_Textures.Add(p_Texture1);
        m_Textures.Add(p_Texture2);

        GenerateMap();
        UpdateMap();
    }

    private void UpdateMap() {
        ConstructMesh();
        GenerateMesh();
    }

    private void GenerateMap() {
        m_Pos   = new List<Vector2>();
        m_Type  = new List<TILE_TYPE>();

        for (int cptX = 0; cptX < MapSize_X; cptX++) {
            int l_Stone     = GetNoise(cptX, 0, MapSize_Y * 0.8f, 15);
            l_Stone         += GetNoise(cptX, 0, MapSize_Y * 0.4f, 30);
            l_Stone         += GetNoise(cptX, 0, MapSize_Y * 0.1f, 10);
            l_Stone         += (int)(MapSize_Y * 0.5f);

            int l_Dirt  = GetNoise(cptX, (int)(MapSize_Y * 0.8f), MapSize_Y / 2, 30);
            l_Dirt      += GetNoise(cptX, (int)(MapSize_Y * 0.75f), MapSize_Y / 2, 30);
            l_Dirt      += (int)(MapSize_Y * 0.75f);

            for (int cptY = 0; cptY < MapSize_Y; cptY++) {
                m_Pos.Add(new Vector2(cptX, cptY));

                if (cptY < l_Stone) {
                    if (GetNoise(cptX, cptY, 14, 16) > 10) m_Type.Add(TILE_TYPE.GRASS);
                    else if (GetNoise(cptX, cptY * 2, 14, 16) > 10) m_Type.Add(TILE_TYPE.EMPTY);
                    else m_Type.Add(TILE_TYPE.STONE);

                }
                else if (cptY < l_Dirt) m_Type.Add(TILE_TYPE.GRASS);
                else m_Type.Add(TILE_TYPE.EMPTY);
            }
        }
    }

    private int GetNoise(int p_X, int p_Y, float p_Scale, float p_Magnitude, float p_Exponentielle = 1.0f) {
        return (int)Mathf.Pow(Mathf.PerlinNoise(p_X / p_Scale, p_Y / p_Scale) * p_Magnitude, p_Exponentielle);
    }

    private void ConstructMesh() {
        for (int cptX = 0; cptX < MapSize_X; cptX++) {
            for (int cptY = 0; cptY < MapSize_Y * 2; cptY++) {
                if (GetBlockType(cptX, cptY) != TILE_TYPE.EMPTY) {
                    BuildCollider(cptX, cptY);
                    BuildMesh(cptX, cptY, m_Textures[(int)GetBlockType(cptX, cptY)]);
                }
            }
        }

        BuildColliderTriangle();
        BuildMeshTriangle();
    }

    private void BuildCollider(int p_X, int p_Y) {
        #region TOP
        if (GetBlockType(p_X, p_Y + 1) == TILE_TYPE.EMPTY) {
            m_ColVertices.Add(new Vector3(p_X, p_Y, 1));
            m_ColVertices.Add(new Vector3(p_X + 1, p_Y, 1));
            m_ColVertices.Add(new Vector3(p_X + 1, p_Y, 0));
            m_ColVertices.Add(new Vector3(p_X, p_Y, 0));
        }
        #endregion

        #region BOTTOM
        if (GetBlockType(p_X, p_Y - 1) == TILE_TYPE.EMPTY) {
            m_ColVertices.Add(new Vector3(p_X, p_Y - 1, 0));
            m_ColVertices.Add(new Vector3(p_X + 1, p_Y - 1, 0));
            m_ColVertices.Add(new Vector3(p_X + 1, p_Y - 1, 1));
            m_ColVertices.Add(new Vector3(p_X, p_Y - 1, 1));
        }
        #endregion

        #region LEFT
        if (GetBlockType(p_X - 1, p_Y) == TILE_TYPE.EMPTY) {
            m_ColVertices.Add(new Vector3(p_X, p_Y - 1, 1));
            m_ColVertices.Add(new Vector3(p_X, p_Y, 1));
            m_ColVertices.Add(new Vector3(p_X, p_Y, 0));
            m_ColVertices.Add(new Vector3(p_X, p_Y - 1, 0));
        }
        #endregion

        #region RIGHT
        if (GetBlockType(p_X + 1, p_Y) == TILE_TYPE.EMPTY) {
            m_ColVertices.Add(new Vector3(p_X + 1, p_Y, 1));
            m_ColVertices.Add(new Vector3(p_X + 1, p_Y - 1, 1));
            m_ColVertices.Add(new Vector3(p_X + 1, p_Y - 1, 0));
            m_ColVertices.Add(new Vector3(p_X + 1, p_Y, 0));
        }
        #endregion
    }

    private void BuildColliderTriangle(int p_Iteration = 0) {
        m_ColTriangles.Add(p_Iteration);
        m_ColTriangles.Add(p_Iteration + 1);
        m_ColTriangles.Add(p_Iteration + 3);
        m_ColTriangles.Add(p_Iteration + 1);
        m_ColTriangles.Add(p_Iteration + 2);
        m_ColTriangles.Add(p_Iteration + 3);

        if (p_Iteration + 4 < m_ColVertices.Count) BuildColliderTriangle(p_Iteration + 4);
    }

    private void BuildMesh(int p_X, int p_Y, Vector2 p_Texture) {
        m_Vertices.Add(new Vector3(p_X, p_Y, 0));
        m_Vertices.Add(new Vector3(p_X + 1, p_Y, 0));
        m_Vertices.Add(new Vector3(p_X + 1, p_Y - 1, 0));
        m_Vertices.Add(new Vector3(p_X, p_Y - 1, 0));

        m_UV.Add(new Vector2(p_Texture.x * UNIT_TEXTURE, p_Texture.y * UNIT_TEXTURE + UNIT_TEXTURE));
        m_UV.Add(new Vector2(p_Texture.x * UNIT_TEXTURE + UNIT_TEXTURE, p_Texture.y * UNIT_TEXTURE + UNIT_TEXTURE));
        m_UV.Add(new Vector2(p_Texture.x * UNIT_TEXTURE + UNIT_TEXTURE, p_Texture.y * UNIT_TEXTURE));
        m_UV.Add(new Vector2(p_Texture.x * UNIT_TEXTURE, p_Texture.y * UNIT_TEXTURE));
    }

    private void BuildMeshTriangle(int p_Iteration = 0) {
        m_Triangles.Add(p_Iteration);
        m_Triangles.Add(p_Iteration + 1);
        m_Triangles.Add(p_Iteration + 3);
        m_Triangles.Add(p_Iteration + 1);
        m_Triangles.Add(p_Iteration + 2);
        m_Triangles.Add(p_Iteration + 3);

        if (p_Iteration + 4 < m_Vertices.Count) BuildMeshTriangle(p_Iteration + 4);
    }

    private void GenerateMesh() {
        Mesh l_ColliderMesh         = new Mesh();
        l_ColliderMesh.vertices     = m_ColVertices.ToArray();
        l_ColliderMesh.triangles    = m_ColTriangles.ToArray();
        m_Collider.sharedMesh       = l_ColliderMesh;

        m_ColVertices.Clear();
        m_ColTriangles.Clear();
        m_Mesh.Clear();

        m_Mesh.vertices     = m_Vertices.ToArray();
        m_Mesh.triangles    = m_Triangles.ToArray();
        m_Mesh.uv           = m_UV.ToArray();

        m_Vertices.Clear();
        m_Triangles.Clear();
        m_UV.Clear();

        m_Mesh.Optimize();
        m_Mesh.RecalculateNormals();
    }

    private TILE_TYPE GetBlockType(int p_X, int p_Y) {
        if (p_X <= -1 || p_X >= MapSize_X || p_Y <= -1 || p_Y >= MapSize_Y * 2) return TILE_TYPE.EMPTY;
        else {
            int l_Index = m_Pos.IndexOf(new Vector2(p_X, p_Y));

            return (l_Index != -1) ? m_Type[l_Index] : TILE_TYPE.EMPTY;
        }
    }
    #endregion

    public void AddUnitAt(Vector3 p_HitPos, string p_UnitType)
    {
        print(p_UnitType);
        print(m_TypeToEnemy[p_UnitType]);
        GameObject l_Unit = Instantiate<GameObject>(m_TypeToEnemy[p_UnitType]);
        l_Unit.transform.position = p_HitPos;
        m_Enemies.Add(l_Unit, p_UnitType);
    }

    public void AddPlayerAt(Vector3 p_HitPos)
    {
        if (!m_PlayerObj)
            m_PlayerObj = Instantiate<GameObject>(m_TypeToEnemy[NAME_FILE_PLAYER]);

        m_PlayerObj.transform.position = p_HitPos;
        m_PlayerMapPos = p_HitPos;
    }

    public void SaveLevel() {
        JSONObject l_JsonLevel = new JSONObject(JSONObject.Type.OBJECT);

        #region Sizes
        l_JsonLevel.AddField(FIELD_SIZE_X, MapSize_X);
        l_JsonLevel.AddField(FIELD_SIZE_Y, MapSize_Y);
        #endregion

        #region Map
        JSONObject l_JsonMap = new JSONObject(JSONObject.Type.ARRAY);
        for (int cptTile = 0; cptTile < m_Type.Count; cptTile++) {
            //if (m_Type[cptTile] != TILE_TYPE.EMPTY) {
                JSONObject l_JsonTile = new JSONObject(JSONObject.Type.OBJECT);

                l_JsonTile.AddField(FIELD_POS, JSONTemplates.FromVector2(m_Pos[cptTile]));
                l_JsonTile.AddField(FIELD_TYPE, (int)m_Type[cptTile]);

                l_JsonMap.Add(l_JsonTile);
            //}
        }
        l_JsonLevel.AddField(FIELD_MAP, l_JsonMap);
        #endregion

        #region Textures
        JSONObject l_JsonTextures = new JSONObject(JSONObject.Type.OBJECT);
        string[] l_TypesName = Enum.GetNames(typeof(TILE_TYPE));
        for (int cptTexture = 0; cptTexture < m_Textures.Count; cptTexture++) {
            l_JsonTextures.AddField(l_TypesName[cptTexture], JSONTemplates.FromVector2(m_Textures[cptTexture]));
        }

        l_JsonLevel.AddField(FIELD_TEXTURES, l_JsonTextures);
        #endregion

        #region Enemies
        JSONObject l_JsonEnemy = new JSONObject(JSONObject.Type.ARRAY);
        foreach (KeyValuePair<GameObject, string> l_EnemyObj in m_Enemies)
        {
            JSONObject l_EnemyJson = new JSONObject(JSONObject.Type.OBJECT);
            JSONObject l_EnemyPosition = new JSONObject(JSONObject.Type.OBJECT);
            l_EnemyPosition.AddField(FIELD_X, Mathf.Round(l_EnemyObj.Key.transform.position.x));
            l_EnemyPosition.AddField(FIELD_Y, Mathf.Round(l_EnemyObj.Key.transform.position.y));

            l_EnemyJson.AddField(FIELD_POSITION, l_EnemyPosition);
            l_EnemyJson.AddField(FIELD_TYPE, l_EnemyObj.Value);

            l_JsonEnemy.Add(l_EnemyJson);
        }

        l_JsonLevel.AddField(FIELD_ENEMIES, l_JsonEnemy);
        #endregion

        #region Player
        JSONObject l_PlayerJson = new JSONObject(JSONObject.Type.OBJECT);
        JSONObject l_PlayerPosition = new JSONObject(JSONObject.Type.OBJECT);
        l_PlayerPosition.AddField(FIELD_X, Mathf.Round(m_PlayerObj.transform.position.x));
        l_PlayerPosition.AddField(FIELD_Y, Mathf.Round(m_PlayerObj.transform.position.y));
        
        l_PlayerJson.AddField(FIELD_POSITION, l_PlayerPosition);
        l_JsonLevel.AddField(FIELD_PLAYER, l_PlayerJson);
        #endregion

        File.WriteAllText(PATH_JSON_WRITTING + NAME_FILE_LEVEL + PATH_JSON, l_JsonLevel.ToString());
    }

    public bool AreUnitsPlaced()
    {
        return (m_PlayerObj && (m_Enemies.Count > 0));
    }
}

public enum TILE_TYPE {
    EMPTY,
    GRASS,
    STONE
}