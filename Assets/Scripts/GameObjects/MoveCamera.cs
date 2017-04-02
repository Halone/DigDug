using UnityEngine;
using System.Collections;
using System;

public class MoveCamera : MonoBehaviour {
    private Action doAction;

    private IEnumerator Start()
    {
        SetModeVoid();

        while (LevelManager.instance == null && !LevelManager.instance.isReady)
            yield return null;

        GameManager.instance.onMenu += SetModeVoid;
        LevelManager.instance.onGenerationEnd += SetModeFollowPlayer;
    }

	private void Update()
    {
        doAction();
    }

    private void SetModeVoid()
    {
        doAction = DoActionVoid;
    }

    private void SetModeFollowPlayer()
    {
        doAction = DoActionFollowPlayer;
    }

    private void DoActionVoid() {}

    private void DoActionFollowPlayer()
    {
        Vector2 l_PlayerPos;
        if(LevelManager.instance.IsCameraInBounds(out l_PlayerPos))
        {
            //Le set ne veut pas fonctionner
            transform.position = new Vector3(transform.position.x, l_PlayerPos.y, transform.position.z);
        }
    }
}
