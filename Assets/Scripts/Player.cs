using UnityEngine;

public class Player : MonoBehaviour {
    public MeshCreator world;

    [SerializeField]
    private float m_Speed_X;
    private Rigidbody m_Rigid;
    private float m_MoveThreshold = 0.1f;

    private void Start() {
        m_Rigid = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update () {
        if(Mathf.Abs(Input.GetAxis("Horizontal")) > m_MoveThreshold) transform.Translate(Vector3.right * m_Speed_X * Input.GetAxis("Horizontal") * Time.deltaTime);
        else if (Mathf.Abs(Input.GetAxis("Vertical")) > m_MoveThreshold) transform.Translate(Vector3.up * m_Speed_X * Input.GetAxis("Vertical") * Time.deltaTime);
    }

    /*private void OnCollisionEnter(Collision collision)
    {
        print("Here");
        if (collision.gameObject.CompareTag("World"))
            if (Input.GetKey("down"))
                l_World.DestroyBlockAt(transform.position + Vector3.down);
        if (Input.GetKey("right"))
            l_World.DestroyBlockAt(transform.position + Vector3.right);
        if (Input.GetKey("left"))
            l_World.DestroyBlockAt(transform.position + Vector3.left);
        if (Input.GetKey("up"))
            l_World.DestroyBlockAt(transform.position + Vector3.up);
    }*/

    private void OnTriggerEnter(Collider other) {
        if (other.CompareTag("World")) {
            if (Input.GetKey("down")) world.DestroyBlockAt(transform.position + Vector3.down);
            if (Input.GetKey("right")) world.DestroyBlockAt(transform.position + Vector3.right);
            if (Input.GetKey("left")) world.DestroyBlockAt(transform.position + Vector3.left);
            if (Input.GetKey("up")) world.DestroyBlockAt(transform.position + Vector3.up);
        }
    }
}
