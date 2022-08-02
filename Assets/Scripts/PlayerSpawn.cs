using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSpawn : MonoBehaviour
{
    public Transform Spawn (Transform Player)
    {
        Transform CurrentPlayer = Instantiate (Player, transform.position, transform.rotation);

        Camera [] PlayerCameraArray = CurrentPlayer.GetComponentsInChildren<Camera>();

        foreach (Camera PlayerCamera in PlayerCameraArray)
        {
            PlayerCamera.targetDisplay = 0;
        }

        return CurrentPlayer;
    }
}
