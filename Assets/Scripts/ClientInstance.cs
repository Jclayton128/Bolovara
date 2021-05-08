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
    public GameObject currentAvatar;

    public static Action<GameObject> OnAvatarSpawned; //Anytime an observer to this event hears it, they get passed a reference Game Object

    public void InvokeAvatarSpawned(GameObject go)  
        //This fires or dispatches the OnAvatarSpawned event, along with the GameObject reference of the thing that just spawned
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
        CmdRequestSpawn();
    }

    [Command]
    private void CmdRequestSpawn()
    {
        NetworkSpawnAvatar();
    }

    [Server]
    private void NetworkSpawnAvatar()
    {
        GameObject go = Instantiate(tankPrefab, transform.position, Quaternion.identity);
        NetworkServer.Spawn(go, base.connectionToClient);
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

