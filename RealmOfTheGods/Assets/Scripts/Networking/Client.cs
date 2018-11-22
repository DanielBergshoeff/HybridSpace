using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Events;

public class MyGameObjectEvent : UnityEvent<GameObject> {
}

public class Client : NetworkBehaviour
{
    // Put the template scriptableobject in here to support persistance
    [SerializeField] private ClientConnection clientConnectionBase;

    // private reference to custom client persistance
    private ClientConnection clientConnection;

    [SerializeField] private GameObject warriorPrefab;
    [SerializeField] private GameObject basePrefab;

    public static MyGameObjectEvent OnBasePlaced;

    private GameObject baseCore;

    public static Client LocalClient {
        get;
        private set;
    }

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
            LocalClient = this;
            Debug.Log(hasAuthority);
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

    public void SpawnWarriorClient(Vector3 pos) {
        if (isLocalPlayer) {
            Debug.Log("Is local player");
            if (hasAuthority) {
                Debug.Log("Authority");
                CmdSpawnWarrior(pos);
            }
            else {
                Debug.Log("No authority");
            }
        }
        else {
            Debug.Log("Not the local player");
        }
            
    }

    public void SpawnBaseClient() {
        CmdSpawnBase();
    }

    [Command]
    private void CmdSpawnBase() {
        baseCore = Instantiate(basePrefab, Vector3.zero, Quaternion.identity);
        baseCore.transform.Rotate(new Vector3(90, 0, 0));
        NetworkServer.Spawn(baseCore);
        RpcRotateBaseOnce(baseCore.transform.rotation, baseCore);
    }

    [ClientRpc]
    private void RpcRotateBaseOnce(Quaternion rot, GameObject go) {
        go.transform.rotation = rot;
        OnBasePlaced.Invoke(baseCore);
    }

    [Command]
    private void CmdSpawnWarrior(Vector3 pos) {
        GameObject go = Instantiate(warriorPrefab, pos, Quaternion.identity);
        go.transform.parent = baseCore.transform;
        NetworkServer.Spawn(go);
        RpcSyncWarriorOnce(go.transform.localPosition, go.transform.localRotation, go, baseCore);
    }

    [ClientRpc]
    public void RpcSyncWarriorOnce(Vector3 localPos, Quaternion localRot, GameObject go, GameObject parent) {
        go.transform.parent = parent.transform;
        go.transform.localPosition = localPos;
        go.transform.localRotation = localRot;
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
