using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class Enemy : MonoBehaviour {
    public static Action onDie;
    private Action doAction;

    private List<Vector2> m_CurrentPath;
    private Vector2 m_CurrentPos;
    private int m_CurrentPathIndex = 0;
    private bool m_HuntPlayer = false;
    private bool m_GoToPlayer = false;

    private uint m_CurrentPumpState = 0;

    private void Start()
    {
        LevelManager.instance.onClearLevel += DestroyUnit;

        m_CurrentPos = transform.position;
        if (SetDestinationToPlayer())
        {
            m_HuntPlayer = true;
            GoToNextCell();
        }
        else
        {
            StartCoroutine(Move(m_CurrentPos, LevelManager.instance.MoveToNearRandomPos(m_CurrentPos)));
            StartCoroutine(WaitBeforeSearchingPlayer(10.0f));
        }
    }
    
    private bool SetDestinationToPlayer(bool p_GoThroughWall = false)
    {
        List<Vector2> m_NewPath;
        if (m_CurrentPos != null && LevelManager.instance.GetPath(m_CurrentPos, out m_NewPath, p_GoThroughWall))
        {
            m_CurrentPath = m_NewPath;
            m_CurrentPathIndex = 0;
            return true;
        }
        m_CurrentPath = null;
        return false;
    }

    private void GoToNextCell()
    {
        m_CurrentPos = transform.position;
        if (m_GoToPlayer)
        {
            m_GoToPlayer = false;
            m_HuntPlayer = true;
            SetDestinationToPlayer(true);
            GoToNextCell();
        }
        else if (m_CurrentPath != null && m_CurrentPath.Count > 0 && (m_CurrentPathIndex + 1) <= (m_CurrentPath.Count - 1))
        {
            StopCoroutine("Move");
            StartCoroutine(Move(m_CurrentPos, m_CurrentPath[m_CurrentPathIndex + 1]));
            m_CurrentPathIndex++;
        }
        else
        {
            if (m_HuntPlayer)
            {
                SetDestinationToPlayer();
                GoToNextCell();
            }
            else
            {
                Move(m_CurrentPos, LevelManager.instance.MoveToNearRandomPos(m_CurrentPos));
            }
        }
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

    private IEnumerator WaitBeforeSearchingPlayer(float WaitingDuration)
    {
        yield return new WaitForSeconds(WaitingDuration);
        m_GoToPlayer = true;
    }

    #region Pump
    private IEnumerator Degonfle(float WaitingDuration)
    {
        yield return new WaitForSeconds(WaitingDuration);
        m_CurrentPumpState--;
        CheckPumpState();
    }

    private void CheckPumpState()
    {
        StopCoroutine("Degonfle");
        if (m_CurrentPumpState > 0)
                StartCoroutine(Degonfle(3));
    }

    public void PumpEnemy() {
        m_CurrentPumpState++;
        if (m_CurrentPumpState >= 3)
            Die();
        else
        {
            StopCoroutine("Degonfle");
            StartCoroutine(Degonfle(3));
        }
    }
    #endregion

    private void Die()
    {
        UIManager.instance.UpdateScore();
        LevelManager.instance.onClearLevel -= DestroyUnit;
        if (onDie != null)
            onDie();
        Destroy(gameObject);
    }

    private void DestroyUnit()
    {
        LevelManager.instance.onClearLevel -= DestroyUnit;
        Destroy(gameObject);
    }
}
