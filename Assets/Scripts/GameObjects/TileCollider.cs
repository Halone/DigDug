using System;
using System.Collections.Generic;
using UnityEngine;

public class TileCollider: MonoBehaviour {
    #region Variables
    private const string TAG_PLAYER = "Player";

    private static List<Vector3> m_VerticesPos;

    public static Action<Vector2> onTileDestroy;

    private MeshCollider m_Collider;
    private Mesh m_Mesh;
    private List<Vector3> m_Vertices;
    private List<int> m_Triangles;
    #endregion

    #region Initialisation & Destroy
    void Awake() {
        if (m_VerticesPos == null) {
            m_VerticesPos = new List<Vector3>();

            m_VerticesPos.Add(new Vector3(0, 0, 1));
            m_VerticesPos.Add(new Vector3(1, 0, 1));
            m_VerticesPos.Add(new Vector3(1, 0, 0));
            m_VerticesPos.Add(new Vector3(0, 0, 0));
            m_VerticesPos.Add(new Vector3(0, -1, 0));
            m_VerticesPos.Add(new Vector3(1, -1, 0));
            m_VerticesPos.Add(new Vector3(1, -1, 1));
            m_VerticesPos.Add(new Vector3(0, -1, 1));
        }

        m_Collider  = gameObject.transform.GetComponent<MeshCollider>();
        m_Mesh      = new Mesh();
        m_Vertices  = new List<Vector3>();
        m_Triangles = new List<int>();

        LevelManager.instance.onUpdateCollider += UpdateMesh;
    }

    void OnDestroy() {
        LevelManager.instance.onUpdateCollider -= UpdateMesh;

        m_Triangles.Clear();
        m_Vertices.Clear();
        m_Mesh.Clear();

        m_Triangles = null;
        m_Vertices  = null;
        m_Mesh      = null;
        m_Collider  = null;
    }
    #endregion

    #region Collider Managment
    private void UpdateMesh(Dictionary<Vector2, TILE_TYPE> p_Model) {
        Vector2 l_Pos   = new Vector2();
        float l_X       = gameObject.transform.localPosition.x;
        float l_Y       = gameObject.transform.localPosition.y;
        int l_Iteration = 0;
        TILE_TYPE l_Type;

        #region TOP
        l_Pos.Set(l_X, l_Y + 1);
        if (p_Model.TryGetValue(l_Pos, out l_Type)) {
            if (l_Type == TILE_TYPE.EMPTY) {
                AddTop(l_Iteration);
                l_Iteration += 4;
            }
        }
        else {
            AddTop(l_Iteration);
            l_Iteration += 4;
        }
        #endregion

        #region BOTTOM
        l_Pos.Set(l_X, l_Y - 1);
        if (p_Model.TryGetValue(l_Pos, out l_Type)) {
            if (l_Type == TILE_TYPE.EMPTY) {
                AddBottom(l_Iteration);
                l_Iteration += 4;
            }
        }
        else {
            AddBottom(l_Iteration);
            l_Iteration += 4;
        }
        #endregion

        #region LEFT
        l_Pos.Set(l_X - 1, l_Y);
        if (p_Model.TryGetValue(l_Pos, out l_Type)) {
            if (l_Type == TILE_TYPE.EMPTY) {
                AddLeft(l_Iteration);
                l_Iteration += 4;
            }
        }
        else {
            AddLeft(l_Iteration);
            l_Iteration += 4;
        }
        #endregion

        #region RIGHT
        l_Pos.Set(l_X + 1, l_Y);
        if (p_Model.TryGetValue(l_Pos, out l_Type)) {
            if (l_Type == TILE_TYPE.EMPTY) {
                AddRight(l_Iteration);
                l_Iteration += 4;
            }
        }
        else {
            AddRight(l_Iteration);
            l_Iteration += 4;
        }
        #endregion

        m_Mesh.Clear();
        m_Mesh.vertices         = m_Vertices.ToArray();
        m_Mesh.triangles        = m_Triangles.ToArray();
        m_Collider.sharedMesh   = m_Mesh;
        m_Vertices.Clear();
        m_Triangles.Clear();
    }

    void OnTriggerEnter(Collider p_Collider) {
        if (p_Collider.gameObject.tag == TAG_PLAYER)  {
            if (onTileDestroy != null) onTileDestroy(gameObject.transform.position);
            Destroy(gameObject);
        }
    }

    #region Utils
    private void AddTop(int p_Iteration) {
        m_Vertices.Add(m_VerticesPos[0]);
        m_Vertices.Add(m_VerticesPos[1]);
        m_Vertices.Add(m_VerticesPos[2]);
        m_Vertices.Add(m_VerticesPos[3]);

        AddTriangles(p_Iteration);
    }

    private void AddBottom(int p_Iteration) {
        m_Vertices.Add(m_VerticesPos[4]);
        m_Vertices.Add(m_VerticesPos[5]);
        m_Vertices.Add(m_VerticesPos[6]);
        m_Vertices.Add(m_VerticesPos[7]);

        AddTriangles(p_Iteration);
    }

    private void AddLeft(int p_Iteration) {
        m_Vertices.Add(m_VerticesPos[7]);
        m_Vertices.Add(m_VerticesPos[0]);
        m_Vertices.Add(m_VerticesPos[3]);
        m_Vertices.Add(m_VerticesPos[4]);

        AddTriangles(p_Iteration);
    }

    private void AddRight(int p_Iteration) {
        m_Vertices.Add(m_VerticesPos[1]);
        m_Vertices.Add(m_VerticesPos[6]);
        m_Vertices.Add(m_VerticesPos[5]);
        m_Vertices.Add(m_VerticesPos[2]);

        AddTriangles(p_Iteration);
    }

    private void AddTriangles(int p_Iteration) {
        m_Triangles.Add(p_Iteration);
        m_Triangles.Add(p_Iteration + 1);
        m_Triangles.Add(p_Iteration + 3);
        m_Triangles.Add(p_Iteration + 1);
        m_Triangles.Add(p_Iteration + 2);
        m_Triangles.Add(p_Iteration + 3);
    }
    #endregion
    #endregion
}