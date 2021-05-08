using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System;
using Cinemachine;

public class ClientInstance : NetworkBehaviour
{
    [SerializeField] GameObject tankPrefab = null;
    public static ClientInstance Instance;
    Camera cam;

    //public override void OnStartServer()
    //{
    //    base.OnStartServer();
    //    NetworkSpawnAvatar();
    //}

    public static Action<GameObject> OnAvatarSpawned;

    public void InvokeAvatarSpawned(GameObject go)
    {
        OnAvatarSpawned?.Invoke(go);
    }


    public override void OnStartLocalPlayer()
    {
        base.OnStartLocalPlayer();
        Instance = this;
        cam = Camera.main;
        if (!isLocalPlayer)
        {
            cam.enabled = false;
        }
        NetworkSpawnAvatar();
    }

    private void NetworkSpawnAvatar()
    {
        GameObject go = Instantiate(tankPrefab, transform.position, Quaternion.identity);
        NetworkServer.Spawn(go, base.connectionToClient);
        cam.enabled = true;
        cam.GetComponentInChildren<CinemachineVirtualCamera>().Follow = go.transform;
    }

    public static ClientInstance ReturnClientInstance(NetworkConnection conn = null)
    {
        if (NetworkServer.active && conn != null)
        {
            NetworkIdentity localPlayer;
            if (BolovaraNetworkManager.LocalPlayers.TryGetValue(conn, out localPlayer))
                return localPlayer.GetComponent<ClientInstance>();
            else
                return null;
        }
        else
        {
            return Instance;
        }
    }
}

