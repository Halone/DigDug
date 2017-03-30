using System;
using System.Collections;
using UnityEngine;

public class Player: Singleton<Player> {
    public static Action<bool> onDeath;
    private const string TAG_ENEMY = "Enemy";

    [SerializeField]
    private float m_Speed;
    private float m_MoveThreshold = 0.1f;

    protected override IEnumerator CoroutineStart()
    {
        LevelManager.instance.onClearLevel += DestroyUnit;
        isReady = true;
        yield return true;
    }

   private void Update () {
        if (Mathf.Abs(Input.GetAxis("Horizontal")) > m_MoveThreshold) transform.Translate(Vector3.right * m_Speed * Input.GetAxis("Horizontal") * Time.deltaTime);
        else if (Mathf.Abs(Input.GetAxis("Vertical")) > m_MoveThreshold) transform.Translate(Vector3.up * m_Speed * Input.GetAxis("Vertical") * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider p_Collider)
    {
        if(p_Collider.tag == TAG_ENEMY)
            if (onDeath != null)
                onDeath(false);
    }

    private void DestroyUnit()
    {
        LevelManager.instance.onClearLevel -= DestroyUnit;
        Destroy(gameObject);
    }
}