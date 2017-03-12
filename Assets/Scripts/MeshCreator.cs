﻿using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class MeshCreator: MonoBehaviour {
    #region Variables
    private const float UNIT_TEXTURE = 0.0625f;

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

    public List<Vector2> m_Pos;
    public List<MAP_TYPE> m_Type;
    public List<Vector2> m_Text;

    public enum MAP_TYPE {
        EMPTY,
        GRASS,
        STONE
    }
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
        if (m_Text != null) m_Text.Clear();
        else m_Text = new List<Vector2>();
        m_Text.Add(Vector2.zero);
        m_Text.Add(p_Texture1);
        m_Text.Add(p_Texture2);

        GenerateMap();
        UpdateMap();
    }

    private void UpdateMap() {
        ConstructMesh();
        GenerateMesh();
    }

    private void GenerateMap() {
        m_Pos   = new List<Vector2>();
        m_Type  = new List<MAP_TYPE>();

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

                    if (GetNoise(cptX, cptY, 14, 16) > 10) m_Type.Add(MAP_TYPE.GRASS);
                    else if (GetNoise(cptX, cptY * 2, 14, 16) > 10) m_Type.Add(MAP_TYPE.EMPTY);
                    else m_Type.Add(MAP_TYPE.STONE);

                }
                else if (cptY < l_Dirt) {
                    m_Pos.Add(new Vector2(cptX, cptY));
                    m_Type.Add(MAP_TYPE.GRASS);
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
                if (GetBlockType(cptX, cptY) != MAP_TYPE.EMPTY) {
                    BuildCollider(cptX, cptY);
                    BuildMesh(cptX, cptY, m_Text[(int)GetBlockType(cptX, cptY)]);
                }
            }
        }

        BuildColliderTriangle();
        BuildMeshTriangle();
    }

    private void BuildCollider(int p_X, int p_Y) {
        #region TOP
        if (GetBlockType(p_X, p_Y + 1) == MAP_TYPE.EMPTY) {
            m_ColVertices.Add(new Vector3(p_X, p_Y, 1));
            m_ColVertices.Add(new Vector3(p_X + 1, p_Y, 1));
            m_ColVertices.Add(new Vector3(p_X + 1, p_Y, 0));
            m_ColVertices.Add(new Vector3(p_X, p_Y, 0));
        }
        #endregion

        #region BOTTOM
        if (GetBlockType(p_X, p_Y - 1) == MAP_TYPE.EMPTY) {
            m_ColVertices.Add(new Vector3(p_X, p_Y - 1, 0));
            m_ColVertices.Add(new Vector3(p_X + 1, p_Y - 1, 0));
            m_ColVertices.Add(new Vector3(p_X + 1, p_Y - 1, 1));
            m_ColVertices.Add(new Vector3(p_X, p_Y - 1, 1));
        }
        #endregion

        #region LEFT
        if (GetBlockType(p_X - 1, p_Y) == MAP_TYPE.EMPTY) {
            m_ColVertices.Add(new Vector3(p_X, p_Y - 1, 1));
            m_ColVertices.Add(new Vector3(p_X, p_Y, 1));
            m_ColVertices.Add(new Vector3(p_X, p_Y, 0));
            m_ColVertices.Add(new Vector3(p_X, p_Y - 1, 0));
        }
        #endregion

        #region RIGHT
        if (GetBlockType(p_X + 1, p_Y) == MAP_TYPE.EMPTY) {
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

    private MAP_TYPE GetBlockType(int p_X, int p_Y) {
        if (p_X <= -1 || p_X >= MapSize_X || p_Y <= -1 || p_Y >= MapSize_Y * 2) return MAP_TYPE.EMPTY;
        else {
            int l_Index = m_Pos.IndexOf(new Vector2(p_X, p_Y));

            return (l_Index != -1) ? m_Type[l_Index] : MAP_TYPE.EMPTY;
        }
    }

    public void DestroyBlockAt(Vector3 p_Pos) {
        Vector2 l_Vec = new Vector2(Mathf.Floor(p_Pos.x), Mathf.Ceil(p_Pos.y));

        if (GetBlockType((int)l_Vec.x, (int)l_Vec.y) != MAP_TYPE.EMPTY) {
            m_Type[m_Pos.IndexOf(l_Vec)] = MAP_TYPE.EMPTY;
            UpdateMap();
        }
    }
    #endregion
}