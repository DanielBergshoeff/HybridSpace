using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Client : NetworkBehaviour
{
    
    // Put the template scriptableobject in here to support persistance
    [SerializeField] private ClientConnection clientConnectionBase;

    // private reference to custom client persistance
    private ClientConnection clientConnection;

    // variable for demonstration purposes
    private int teamScore;


    // Called on player-object creation Serverside
    public override void OnStartServer()
    {
        base.OnStartServer();
    }

    // Called on player-object creation Clientside
    public override void OnStartLocalPlayer()
    {
        base.OnStartLocalPlayer();
        Debug.Log("Joined match");
        
        if (isLocalPlayer)
        {
            // Custom persistance, only exists on server
            // Call Commmand on player-object Serverside, providing device ip adress to recognise potential returning player

            //DEPRECATED
            //CmdGetConnection(Network.player.ipAddress);

            //NEW
            CmdGetConnection(NetworkManager.singleton.networkAddress);
        }
    }

    // Create custom persistance instance
    [Command]
    private void CmdGetConnection(string adress) {
        //Debug.Log("Player IP posted by player: " + adress);
        NetworkConnection conn = connectionToClient;
        conn.address = adress;
        clientConnection = clientConnectionBase.Get(conn, this);

        teamScore = clientConnection.teamScore;
        RpcSetScore(teamScore);
    }

    // ------------------------------------ CHANGE VALUE FROM CLIENT
    // Should only be called clientside
    public void AddScoreClient(int score)
    {
        teamScore += score;
        CmdSetScore(teamScore);
    }

    // When called clientside, will be executed serverside
    [Command]
    private void CmdSetScore(int score) {
        teamScore = score;

        // custom persistance
        clientConnection.SetPersistantScore(teamScore);
    }

    // ------------------------------------ CHANGE VALUE FROM SERVER
    // Should only be called serverside
    public void AddScoreServer(int score) {
        teamScore += score;
        RpcSetScore(teamScore);

        // custom persistance
        clientConnection.SetPersistantScore(teamScore);
    }

    // When called on server side, will be executed client side
    [ClientRpc]
    private void RpcSetScore(int score) {
        // check if this set score is meant for this client, 
        // otherwise it's executed on the client-object on every client game instance, 
        // not so much a problem with variables but could become an issue with methods chains
        if (!isLocalPlayer) { return; }

        teamScore = score;
    }

    // Called when Client loses connection, Clientside
    public void OnConnectionLost()
    {
        
    }
}
