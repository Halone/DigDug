using System;
using UnityEngine;

public class Player: MonoBehaviour {
    #region Variables
    private const string TAG_ENEMY = "Enemy";

    public static Action<bool> onDeath;
    public static Bounds worldBounds;

    [SerializeField]
    private float m_Speed;
    [SerializeField]
    private float m_Range;
    [SerializeField]
    private float m_Margin;
    private float m_MoveThreshold;
    private Vector3 m_Direction;
    private Vector3 m_OffSet;
    #endregion

    #region Initialisation
    void Start() {
        LevelManager.instance.onClearLevel += DestroyUnit;
        m_MoveThreshold = 0.1f;
        m_Direction     = Vector3.left;
        m_OffSet        = new Vector3(0.5f, -0.5f, 0);
    }
    #endregion

    #region Player Managment
    private void Update () {
        if (Mathf.Abs(Input.GetAxis("Horizontal")) > m_MoveThreshold) {
            if (m_Direction == Vector3.up || m_Direction == Vector3.down) gameObject.transform.GetChild(0).Rotate(Vector3.forward, -90);//rotation sprite
            m_Direction = (Input.GetAxis("Horizontal") > 0) ? Vector3.right : Vector3.left;//nouvelle direction
            gameObject.transform.GetComponentInChildren<SpriteRenderer>().flipX = !(m_Direction == Vector3.left);//direction sprite

            Vector3 l_Pos = gameObject.transform.position + ((m_Direction == Vector3.right) ? Vector3.right * (1 + m_Margin) : Vector3.left * m_Margin);
            if (worldBounds.Contains(l_Pos)) transform.Translate(Vector3.right * m_Speed * Input.GetAxis("Horizontal") * Time.deltaTime);//mouvement logique
        }
        else if (Mathf.Abs(Input.GetAxis("Vertical")) > m_MoveThreshold) {
            if (m_Direction == Vector3.left || m_Direction == Vector3.right) gameObject.transform.GetChild(0).Rotate(Vector3.forward, 90);//rotation sprite
            m_Direction = (Input.GetAxis("Vertical") > 0) ? Vector3.up : Vector3.down;//nouvelle direction
            gameObject.transform.GetComponentInChildren<SpriteRenderer>().flipX = !(m_Direction == Vector3.down);//direction sprite

            Vector3 l_Pos = gameObject.transform.position + ((m_Direction == Vector3.down) ? Vector3.down * (1 + m_Margin) : Vector3.up * m_Margin);
            if (worldBounds.Contains(l_Pos)) transform.Translate(Vector3.up * m_Speed * Input.GetAxis("Vertical") * Time.deltaTime);//mouvement logique
        }
        else if (Input.GetKeyDown(KeyCode.Space)) TryPump();
    }

    private void TryPump() {
        Ray l_Ray = new Ray(gameObject.transform.position + m_OffSet, m_Direction);
        RaycastHit l_Hit;
        
        //Replace with graphisme
        Debug.DrawRay(gameObject.transform.position + m_OffSet - Vector3.forward, m_Direction, Color.red, 1);

        if (Physics.Raycast(l_Ray, out l_Hit, m_Range + 0.5f)) {
            if (l_Hit.collider.tag == TAG_ENEMY) l_Hit.collider.transform.GetComponent<Enemy>().PumpEnemy();
        }
    }

    private void OnTriggerEnter(Collider p_Collider) {
        if (p_Collider.tag == TAG_ENEMY) {
            if (onDeath != null) onDeath(false);
        }
    }

    private void DestroyUnit() {
        LevelManager.instance.onClearLevel -= DestroyUnit;
        Destroy(gameObject);
    }
    #endregion
}