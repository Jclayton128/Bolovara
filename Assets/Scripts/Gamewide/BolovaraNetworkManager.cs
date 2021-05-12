using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BolovaraNetworkManager : NetworkManager
{
    public static Dictionary<NetworkConnection, NetworkIdentity> LocalPlayers = new Dictionary<NetworkConnection, NetworkIdentity>();

    public override void OnServerAddPlayer(NetworkConnection conn)
    {
        Transform startPos = GetStartPosition();
        GameObject player = startPos != null
            ? Instantiate(playerPrefab, startPos.position, startPos.rotation)
            : Instantiate(playerPrefab);

        player.name = $"{playerPrefab.name} [connId={conn.connectionId}]";
        LocalPlayers[conn] = player.GetComponent<NetworkIdentity>();
        NetworkServer.AddPlayerForConnection(conn, player);
    }

    public override void OnServerDisconnect(NetworkConnection conn)
    {
        base.OnServerDisconnect(conn);
        LocalPlayers.Remove(conn);
    }

}
