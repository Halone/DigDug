using UnityEngine;

public class BlockDestroyer: MonoBehaviour {
    public MeshCreator World;

    void Update() {
        int l_X = (int)Mathf.Round(gameObject.transform.position.x);
        int l_Y = (int)Mathf.Round(gameObject.transform.position.y);

        if (World.GetBlockType(l_X, l_Y) != (int)MeshCreator.MAP_TYPE.EMPTY) {
            World.DestroyBlock(l_X, l_Y);
        }
    }
}