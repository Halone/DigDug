using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

/*By Erwan CLEMENT, erwan.clement.pro@gmail.com*/
/*public class Pathfinding2D {
    static int temp_NbrIter = 0;
    #region Public Interface
    public static bool GetPath(Dictionary<Vector2, TILE_TYPE> p_Model, Vector2 p_Origin, Vector2 p_Target, out List<Vector2> p_Path)
    {
        Debug.Log("Init Pathfinding2D");
        Dictionary<Vector2, int> l_PropagationMap;

        Vector2 temp_newOrigin = new Vector2(p_Origin.x - 0.5f, p_Origin.y - 0.5f);

        if (GeneratePropagation(p_Model, temp_newOrigin, p_Target, out l_PropagationMap))
        {
            p_Path = GetPath(l_PropagationMap, p_Target);
            Debug.Log("Pathfinding2D: Pathfinding generation Succed");
            return false;
        }
        Debug.LogError("Pathfinding2D: Pathfinding generation failed");
        p_Path = new List<Vector2>();
        return false;
    }
    #endregion

    #region Private Interface
    #region Path Writing
    private static bool GeneratePropagation(Dictionary<Vector2, TILE_TYPE> p_Model, Vector2 p_Origin, Vector2 p_Target, out Dictionary<Vector2, int> p_PropagationMap)
    {
        Debug.Log("Start Path Writing");
        Dictionary<Vector2, int> l_PropagationMap = new Dictionary<Vector2, int>();

        RecursivePropagation(p_Model, l_PropagationMap, p_Origin, 0);

        p_PropagationMap = l_PropagationMap;
        return l_PropagationMap.ContainsKey(p_Target);
    }

    private static void RecursivePropagation(Dictionary<Vector2, TILE_TYPE> p_Model, Dictionary<Vector2, int> p_PropagationMap, Vector2 p_CurrentPos, int p_CurrentPropaIndex)
    {
        temp_NbrIter++;

        DIRECTION[] l_Directions = (DIRECTION[])Enum.GetValues(typeof(DIRECTION));

        foreach (DIRECTION p_Direction in l_Directions)
        {
            Vector2 p_NearCell = GetNextCellPos(p_CurrentPos, p_Direction);
            if (temp_NbrIter < 13 && p_Model.ContainsKey(p_NearCell))
            {
                if (!p_PropagationMap.ContainsKey(p_NearCell))
                {
                    p_PropagationMap.Add(p_NearCell, p_CurrentPropaIndex + 1);
                    RecursivePropagation(p_Model, p_PropagationMap, p_NearCell, p_CurrentPropaIndex + 1);
                }
                else if (p_PropagationMap[p_NearCell] > p_CurrentPropaIndex + 1)
                {
                    p_PropagationMap[p_NearCell] = p_CurrentPropaIndex + 1;
                    RecursivePropagation(p_Model, p_PropagationMap, p_NearCell, p_CurrentPropaIndex + 1);
                }
            }
        }
    }
    #endregion


    #region Path Reading
    private static List<Vector2> GetPath(Dictionary<Vector2, int> p_PropagationMap,Vector2 p_Target)
    {
        
        Debug.Log("Start Path Reading");
        DIRECTION[] l_Directions = (DIRECTION[])Enum.GetValues(typeof(DIRECTION));
        List<Vector2> l_Path = new List<Vector2>();
        Vector2 l_CurrentModelPos = p_Target;
        Vector2 l_CurrentBestCell = new Vector2();
        int l_CurrentPathIndex = p_PropagationMap[p_Target];
        l_Path.Add(l_CurrentModelPos);

        while (l_CurrentPathIndex > 0)
        {
            foreach(DIRECTION p_Direction in l_Directions)
                IsNextCellValid(p_PropagationMap, GetNextCellPos(l_CurrentModelPos, p_Direction), l_CurrentPathIndex, l_CurrentBestCell);

            l_CurrentModelPos = l_CurrentBestCell;
            l_Path.Add(l_CurrentModelPos);
        }

        l_Path.Reverse();
        return l_Path;
    }

    private static void IsNextCellValid(Dictionary<Vector2, int> p_PropagationMap, Vector2 p_TargetCell, int p_CurrentBestPathValue, Vector2 p_CurrentBestCell)
    {
        if (p_PropagationMap.ContainsKey(p_TargetCell) && p_PropagationMap[p_TargetCell] < p_CurrentBestPathValue)
        {
            p_CurrentBestCell = p_TargetCell;
            p_CurrentBestPathValue = p_PropagationMap[p_TargetCell];
        }
    }
    #endregion

    #region Utils
    private static Vector2 GetNextCellPos(Vector2 p_CurrentCell, DIRECTION p_Direction)
    {
        Vector2 p_NextCellPos = p_CurrentCell;
        switch (p_Direction)
        {
            case DIRECTION.UP:
                p_NextCellPos.y++;
                return p_NextCellPos;
            case DIRECTION.RIGHT:
                p_NextCellPos.x++;
                return p_NextCellPos;
            case DIRECTION.BOTTOM:
                p_NextCellPos.y--;
                return p_NextCellPos;
            case DIRECTION.LEFT:
                p_NextCellPos.x--;
                return p_NextCellPos;
            default:
                Debug.LogError("Pathfing2D: Error Direction '" + p_Direction + "' not found");
                return p_NextCellPos;
        }
    }
    #endregion
    #endregion
}*/

public class Pathfinding2D
{
    Dictionary<Vector2, TILE_TYPE> m_Model;

    public Dictionary<Vector2, int> RecursiveMethode(Dictionary<Vector2, int> p_Map, List<Vector2> p_List, int p_Iter = 0)
    {
        List<Vector2> l_NextIter = new List<Vector2>();
        DIRECTION[] l_Directions = (DIRECTION[])Enum.GetValues(typeof(DIRECTION));

        foreach (Vector2 l_Pos in p_List)
        {
            foreach (DIRECTION l_Direction in l_Directions)
            {
                Vector2 l_NextPos = GetNextCellPos(l_Pos, l_Direction);
                if(m_Model.ContainsKey(l_NextPos) && !p_Map.ContainsKey(l_NextPos)) // Ajouter chemin plus court
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
}

public enum DIRECTION
{
    UP,
    RIGHT,
    BOTTOM,
    LEFT
}
