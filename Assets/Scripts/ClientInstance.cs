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

    #region EventResponse

    public void InvokeAvatarSpawned(GameObject go)  
        //This fires or dispatches the OnAvatarSpawned event, along with the GameObject reference of the thing that just spawned
    {
        OnAvatarSpawned?.Invoke(go);
        currentAvatar = go;
        go.GetComponent<Health>().OnAvatarDestroyed += SetupAvatarRespawn;
    }


    #endregion

    #region Client
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
        FindObjectOfType<UIManager>().SetLocalPlayerForUI(this);
    }

    [Client]

    private void Update()
    {
        if (!currentAvatar)
        {
            CmdRequestSpawn();
        }
    }

    private void SetupAvatarRespawn()
    {
        Debug.Log("Oh did you want to respawn?");
        //CmdRequestSpawn();
    }

    [Command]
    public void CmdRequestSpawn()
    {
        NetworkSpawnAvatar();
    }
    #endregion

    #region Server

    [Server]
    private void NetworkSpawnAvatar()
    {
        GameObject go = Instantiate(tankPrefab, transform.position, Quaternion.identity);
        NetworkServer.Spawn(go, base.connectionToClient);
    }

    #endregion

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

