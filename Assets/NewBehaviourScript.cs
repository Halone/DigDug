using UnityEngine;
using System.Collections;

public class NewBehaviourScript : MonoBehaviour {

    public enum DIRECTION
    {
        UP,
        RIGHT,
        BOTTOM,
        LEFT
    }

    #region Pathfinding
    public Dictionary<Vector2, int> RecursiveMethode(Dictionary<Vector2, int> p_Map, List<Vector2> p_List, int p_Iter = 0)
    {
        List<Vector2> l_NextIter = new List<Vector2>();
        DIRECTION[] l_Directions = (DIRECTION[])Enum.GetValues(typeof(DIRECTION));

        foreach (Vector2 l_Pos in p_List)
        {
            foreach (DIRECTION l_Direction in l_Directions)
            {
                Vector2 l_NextPos = GetNextCellPos(l_Pos, l_Direction);
                if (m_Model.ContainsKey(l_NextPos) && !p_Map.ContainsKey(l_NextPos)) // Ajouter chemin plus court
                {
                    p_Map.Add(l_NextPos, p_Iter);
                    l_NextIter.Add(l_NextPos);
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

    public bool GetPath(Vector2 m_Pos, out List<Vector2> m_Path)
    {
        Dictionary<Vector2, int> l_PropaMap = new Dictionary<Vector2, int>();
        List<Vector2> l_List = new List<Vector2>();
        l_List.Add(m_Pos);

        RecursiveMethode(l_PropaMap, l_List);
        print(l_PropaMap);

        m_Path = new List<Vector2>();
        return true;
    }

    public Vector2 GetModelPos(GameObject m_Obj)
    {
        return new Vector2();
    }

    public Vector2 GetPlayerModelPos()
    {
        if (!m_tempPlayer) m_tempPlayer = temp_Player;//transform.Find("Player");
        return m_tempPlayer.transform.position;

        if (onUpdateCollider != null) onUpdateCollider(m_Model);
    }
}
