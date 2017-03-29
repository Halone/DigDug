using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Enemy : MonoBehaviour {
    List<Vector2> m_CurrentPath;
    int m_CurrentPathIndex = 0;
    Vector2 EndMovePos;

    private void Start()
    {
        EndMovePos = transform.position;
        SetDestination();
        GoToNextCell();
    }
    
    private void SetDestination()
    {
        List<Vector2> m_NewPath;
        if (EndMovePos != null && LevelManager.instance.GetPath(EndMovePos, out m_NewPath))
        {
            m_CurrentPath = m_NewPath;
            m_CurrentPathIndex = 0;
        }
    }

    private void GoToNextCell()
    {
        if ((m_CurrentPathIndex + 1) <= (m_CurrentPath.Count - 1))
        {
            StopCoroutine("Move");
            EndMovePos = m_CurrentPath[m_CurrentPathIndex + 1];
            StartCoroutine(Move(m_CurrentPath[m_CurrentPathIndex], EndMovePos));
            m_CurrentPathIndex++;
        }
        else print("Enemy Arrived");
    }

    private IEnumerator Move(Vector2 m_CurrentPos, Vector2 m_Destination, float MoveDuration = 1)
    {
        Vector2 l_StartPos = m_CurrentPos;

        float startTime = Time.time;
        float EndTime = startTime + MoveDuration;
        while (Time.time < EndTime)
        {
            transform.position = Vector2.Lerp(l_StartPos, m_Destination, (Time.time - startTime) / MoveDuration);
            yield return null;
        }

        GoToNextCell();
    }
}
