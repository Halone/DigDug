using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

[ExecuteInEditMode]
public class MeshCreator: MonoBehaviour {
    #region Variables
    private const string PATH_JSON = @"Assets\Resources\json\";
    private const string EXT_JSON = ".json";
    private const string NAME_FILE_LEVEL = "level";
    private const string FIELD_SIZE_X = "size_X";
    private const string FIELD_SIZE_Y = "size_Y";
    private const string FIELD_MAP = "map";
    private const string FIELD_POS = "position";
    private const string FIELD_TYPE = "type";
    private const string FIELD_TEXTURES = "textures";
    private const string FIELD_X = "x";
    private const string FIELD_Y = "y";
    private const float UNIT_TEXTURE        = 0.0625f;

    private Mesh m_Mesh;
    private List<Vector3> m_Vertices;
    private List<int> m_Triangles;
    private List<Vector2> m_UV;
    private MeshCollider m_Collider;
    private List<Vector3> m_ColVertices;
    private List<int> m_ColTriangles;

    [Range(30, 100)]
    public int MapSize_X;
    [Range(30, 100)]
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
        m_Mesh          = gameObject.GetComponent<MeshFilter>().sharedMesh;
        m_Vertices      = new List<Vector3>();
        m_Triangles     = new List<int>();
        m_UV            = new List<Vector2>();
        m_Collider      = gameObject.GetComponent<MeshCollider>();
        m_ColVertices   = new List<Vector3>();
        m_ColTriangles  = new List<int>();
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

            for (int cptY = 0; cptY < MapSize_Y * 2; cptY++) {
                if (cptY < l_Stone) {
                    m_Pos.Add(new Vector2(cptX, cptY));

                    if (GetNoise(cptX, cptY, 14, 16) > 10) m_Type.Add(TILE_TYPE.GRASS);
                    else if (GetNoise(cptX, cptY * 2, 14, 16) > 10) m_Type.Add(TILE_TYPE.EMPTY);
                    else m_Type.Add(TILE_TYPE.STONE);

                }
                else if (cptY < l_Dirt) {
                    m_Pos.Add(new Vector2(cptX, cptY));
                    m_Type.Add(TILE_TYPE.GRASS);
                }
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

    public void DestroyBlockAt(Vector3 p_Pos) {
        Vector2 l_Vec = new Vector2(Mathf.Floor(p_Pos.x), Mathf.Ceil(p_Pos.y));

        if (GetBlockType((int)l_Vec.x, (int)l_Vec.y) != TILE_TYPE.EMPTY) {
            m_Type[m_Pos.IndexOf(l_Vec)] = TILE_TYPE.EMPTY;
            UpdateMap();
        }
    }

    #endregion

    public void SaveLevel()
    {
        JSONObject l_JsonLevel = new JSONObject(JSONObject.Type.OBJECT);

        #region Sizes
        l_JsonLevel.AddField(FIELD_SIZE_X, MapSize_X);
        l_JsonLevel.AddField(FIELD_SIZE_Y, MapSize_Y * 2);
        #endregion

        #region Map
        JSONObject l_JsonMap = new JSONObject(JSONObject.Type.ARRAY);
        for (int cptTile = 0; cptTile < m_Type.Count; cptTile++)
        {
            if (m_Type[cptTile] != TILE_TYPE.EMPTY)
            {
                JSONObject l_JsonTile = new JSONObject(JSONObject.Type.OBJECT);

                l_JsonTile.AddField(FIELD_POS, JSONTemplates.FromVector2(m_Pos[cptTile]));
                l_JsonTile.AddField(FIELD_TYPE, (int)m_Type[cptTile]);

                l_JsonMap.Add(l_JsonTile);
            }
        }
        l_JsonLevel.AddField(FIELD_MAP, l_JsonMap);
        #endregion

        #region Textures
        JSONObject l_JsonTextures = new JSONObject(JSONObject.Type.OBJECT);
        string[] l_TypesName = Enum.GetNames(typeof(TILE_TYPE));
        for (int cptTexture = 0; cptTexture < m_Textures.Count; cptTexture++)
        {
            l_JsonTextures.AddField(l_TypesName[cptTexture], JSONTemplates.FromVector2(m_Textures[cptTexture]));
        }
        l_JsonLevel.AddField(FIELD_TEXTURES, l_JsonTextures);
        #endregion

        File.WriteAllText(PATH_JSON + NAME_FILE_LEVEL + EXT_JSON, l_JsonLevel.ToString());
    }
}

public enum TILE_TYPE {
    EMPTY,
    GRASS,
    STONE
}