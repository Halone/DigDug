using UnityEngine;
using System.Collections;

public abstract class Movable : MonoBehaviour {
    [SerializeField]
    protected float m_Speed;

    protected abstract void MoveX();
    protected abstract void MoveY();
}
