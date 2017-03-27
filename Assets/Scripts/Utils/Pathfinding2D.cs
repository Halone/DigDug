using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Pathfinding2D {

    #region Public Interface
    public static bool GetPath(Dictionary<Vector2, TILE_TYPE> p_Model, Vector2 p_Origin, Vector2 p_Target, out List<Vector2> p_Path)
    {
        Debug.Log("Init Pathfinding2D");
        Dictionary<Vector2, int> l_PropagationMap;

        if (GeneratePropagation(p_Model, p_Origin, p_Target, out l_PropagationMap))
        {
            p_Path = GetPath(l_PropagationMap, p_Target);
            return false;
        }
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

        if (false)
        {
            p_PropagationMap = l_PropagationMap;
            return false;
        }
        p_PropagationMap = l_PropagationMap;
        return true;
    }
    #endregion


    #region Path Reading
    private static List<Vector2> GetPath(Dictionary<Vector2, int> p_PropagationMap,Vector2 p_Target)
    {
        Debug.Log("Start Path Reading");
        List<Vector2> l_Path = new List<Vector2>();
        Vector2 l_CurrentModelPos = p_Target;
        Vector2 l_CurrentBestCell = new Vector2();
        int l_CurrentPathIndex = p_PropagationMap[p_Target];

        while (l_CurrentPathIndex > 0)
        {
            IsNextCellValid(p_PropagationMap, GetNextCellPos(l_CurrentModelPos, DIRECTION.UP)       , l_CurrentPathIndex, l_CurrentBestCell);
            IsNextCellValid(p_PropagationMap, GetNextCellPos(l_CurrentModelPos, DIRECTION.RIGHT)    , l_CurrentPathIndex, l_CurrentBestCell);
            IsNextCellValid(p_PropagationMap, GetNextCellPos(l_CurrentModelPos, DIRECTION.BOTTOM)   , l_CurrentPathIndex, l_CurrentBestCell);
            IsNextCellValid(p_PropagationMap, GetNextCellPos(l_CurrentModelPos, DIRECTION.LEFT)     , l_CurrentPathIndex, l_CurrentBestCell);

            l_CurrentModelPos = l_CurrentBestCell;
        }

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
                Debug.Log("Pathfing2D: Error Direction '" + p_Direction + "' not found");
                return p_NextCellPos;
        }
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
