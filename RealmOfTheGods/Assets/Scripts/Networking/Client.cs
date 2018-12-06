﻿using System;
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
    [SerializeField] private GameObject flagPrefab;

    public static MyGameObjectEvent OnBasePlaced;

    public GameObject baseCore;

    private List<GameObject> warriors;
    private List<GameObject> flags;

    public static Client LocalClient {
        get;
        private set;
    }

    private void Update() {
        if (isServer) {
            /*
            if(warriors != null) {
                for (int i = 0; i < warriors.Count; i++) {
                    if(warriors[i].transform.position != flags[i].transform.position) {
                        warriors[i].transform.position = Vector3.MoveTowards(warriors[i].transform.position, flags[i].transform.position, warriors[i].GetComponent<Unit>().speed * Time.deltaTime);
                    }

                    RpcSyncGameObject(i, warriors[i].transform.localPosition, warriors[i].transform.localRotation);
                }
            }
            */
        }
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
    }

    public void SpawnWarriorClient(Vector3 pos) {
        if (!isLocalPlayer) { return; }
        Debug.Log("Spawn warrior client pos: " + pos);
        CmdSpawnWarrior(pos);
        CmdSpawnFlag(pos);
    }

    public void SetUnitFlag(Vector3 pos, GameObject unit) {
        if(!isLocalPlayer) { return; }
        CmdSetFlag(pos, unit);
    }

    protected void SetClientBaseServer(GameObject newBaseCore) {
        baseCore = newBaseCore;
        RpcSyncBaseOnce(baseCore.transform.rotation, baseCore);
    }

    public void SpawnBaseClient() {
        CmdSpawnBase();
    }

    [Command]
    private void CmdSetFlag(Vector3 pos, GameObject unit) {
        for (int i = 0; i < warriors.Count; i++) {
            if (warriors[i] == unit) {
                flags[i].transform.position = pos;
            }
        }
    }

    [Command]
    private void CmdSpawnFlag(Vector3 pos) {
        GameObject flag = Instantiate(flagPrefab, baseCore.transform.position + pos, Quaternion.identity);
        flag.transform.parent = baseCore.transform;
        if(flags == null) {
            flags = new List<GameObject>();
        }
        flags.Add(flag);
    }

    [Command]
    private void CmdSpawnBase() {
        baseCore = Instantiate(basePrefab, Vector3.zero, Quaternion.identity);
        NetworkServer.Spawn(baseCore);
        foreach (ClientConnection clientConnection in clientConnection.clients) {
            clientConnection.client.SetClientBaseServer(baseCore);
        }
    }

    [ClientRpc]
    private void RpcSyncBaseOnce(Quaternion rot, GameObject go) {
        if (!isLocalPlayer) { return; }
        Debug.Log("Reached rpc sync base once");
        baseCore = go;
        OnBasePlaced.Invoke(go);
    }

    [Command]
    private void CmdSpawnWarrior(Vector3 pos) {
        Debug.Log("Cmd Spawn warrior server pos: " + pos);
        GameObject go = Instantiate(warriorPrefab);
        go.transform.parent = baseCore.transform;
        go.transform.localPosition = pos;
        go.transform.localRotation = Quaternion.identity;
        NetworkServer.Spawn(go);
        Debug.Log("Cmd Spawn warrior server local pos: " + go.transform.localPosition);
        RpcSyncWarriorOnce(go.transform.localPosition, go.transform.localRotation, go, baseCore);
        if(warriors == null) {
            warriors = new List<GameObject>();
        }
        warriors.Add(go);
    }

    [ClientRpc]
    private void RpcSyncWarriorOnce(Vector3 localPos, Quaternion localRot, GameObject go, GameObject parent) {
        go.transform.parent = parent.transform;
        go.transform.localPosition = localPos;
        go.transform.localRotation = localRot;

        Debug.Log("Rpc Spawn warrior client server pos: " + go.transform.localPosition);

        if (warriors == null) {
            warriors = new List<GameObject>();
        }

        warriors.Add(go);
    }

    [ClientRpc]
    private void RpcSyncGameObject(int index, Vector3 localPos, Quaternion localRotation) {
        if(!isLocalPlayer) { return; }
        warriors[index].transform.localPosition = localPos;
        warriors[index].transform.localRotation = localRotation;
    }

    // Called when Client loses connection, Clientside
    public void OnConnectionLost()
    {
        
    }
}
