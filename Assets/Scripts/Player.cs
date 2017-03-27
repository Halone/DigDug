using UnityEngine;

public class Player: MonoBehaviour {
    [SerializeField]
    private float m_Speed_X;
    private float m_MoveThreshold = 0.1f;
    
    void Update () {
        if (Mathf.Abs(Input.GetAxis("Horizontal")) > m_MoveThreshold) transform.Translate(Vector3.right * m_Speed_X * Input.GetAxis("Horizontal") * Time.deltaTime);
        else if (Mathf.Abs(Input.GetAxis("Vertical")) > m_MoveThreshold) transform.Translate(Vector3.up * m_Speed_X * Input.GetAxis("Vertical") * Time.deltaTime);
    }
}