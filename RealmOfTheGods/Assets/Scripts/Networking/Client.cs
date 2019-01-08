using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Events;

public enum UnitType {
    Null,
    MagicGroundUnit,
    MagicFlyingUnit,
    MagicRangedUnit,
    MightGroundUnit,
    MightGroundUnitFast
}

[Serializable]
public enum TeamType {
    Null,
    Yellow,
    Blue,
    Red,
    Green
}

[System.Serializable]
public class TeamToGameObject {
    public TeamType team;
    public GameObject prefab;
}

public class MyGameObjectEvent : UnityEvent<GameObject> {
}

[Serializable]
public class TypeToPrefab {
    public UnitType type;
    public GameObject gameObject;
}

public class Client : NetworkBehaviour
{
    // Put the template scriptableobject in here to support persistance
    [SerializeField] private ClientConnection clientConnectionBase;

    // private reference to custom client persistance
    private ClientConnection clientConnection;
    
    [SerializeField] private GameObject basePrefab;
    [SerializeField] private GameObject flagPrefab;
    [SerializeField] private GameObject unitPrefab;

    [SerializeField] private TypeToPrefab[] typeToPrefabs;

    [SerializeField] private TeamToGameObject[] teamToGameObjects;

    private Playground playground;

    public static MyGameObjectEvent OnBasePlaced;

    public GameObject baseCore;

    private List<GameObject> warriors;
    private List<GameObject> flags;
    
    public TeamType team;

    public static Client LocalClient {
        get;
        private set;
    }

    private void Update() {
        if (isServer) {
            if(warriors != null) {
                for (int i = 0; i < warriors.Count; i++) {
                    if(warriors[i].transform.position != flags[i].transform.position) {
                        if (warriors[i].GetComponentInChildren<Humanoid>().stunTimer <= 0.0f) {
                            warriors[i].transform.position = Vector3.MoveTowards(warriors[i].transform.position, flags[i].transform.position, warriors[i].GetComponentInChildren<Humanoid>().speed * Time.deltaTime);
                        }
                    }
                    RpcSyncGameObject(i, warriors[i].transform.localPosition, warriors[i].transform.localRotation);
                }
            }
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

    public void SpawnUnitClient(Vector3 pos, UnitType type) {
        if (!isLocalPlayer) { return; }
        CmdSpawnUnit(pos, type);
        CmdSpawnFlag(pos);
    }

    public void SpawnUnitClient(TeamType team) {
        this.team = team;
        CmdSpawnTeamUnit(team);
        CmdSpawnTeamFlag(team);
    }

    public void SetUnitFlag(Vector3 pos, TeamType team) {
        if(!isLocalPlayer) { return; }
        CmdSetFlag(pos, team);
    }

    protected void SetClientBaseServer(GameObject newBaseCore) {
        baseCore = newBaseCore;
        playground = baseCore.GetComponent<Playground>();
        RpcSyncBaseOnce(baseCore.transform.rotation, baseCore);
    }

    public void SpawnBaseClient() {
        CmdSpawnBase();
    }

    [Command]
    private void CmdSetFlag(Vector3 pos, TeamType team) {
        int index = 0;
        for (int i = 0; i < flags.Count; i++) {
            if(warriors[i].GetComponentInChildren<Humanoid>().team == team) {
                index = i;
            }
        }
        flags[index].transform.localPosition = pos;
    }

    [Command]
    private void CmdSpawnTeamFlag(TeamType team) {
        Debug.Log("Start team flag thing");
        for (int i = 0; i < teamToGameObjects.Length; i++) {
            if (teamToGameObjects[i].team == team) {
                GameObject flag = Instantiate(flagPrefab, Vector3.zero, Quaternion.identity);
                flag.transform.parent = baseCore.transform;
                for (int j = 0; j < playground.teamToSpawnPoint.Length; j++) {
                    if (playground.teamToSpawnPoint[j].team == team) {
                        flag.transform.position = playground.teamToSpawnPoint[j].prefab.transform.position;
                        break;
                    }
                }

                if (flags == null) {
                    flags = new List<GameObject>();
                }
                flags.Add(flag);
                Debug.Log("Team flag spawned");

                break;
            }
        }
    }

    [Command]
    private void CmdSpawnFlag(Vector3 pos) {
        GameObject flag = Instantiate(flagPrefab, baseCore.transform.position + pos, Quaternion.identity);
        flag.transform.parent = baseCore.transform;
        flag.transform.localPosition = pos;
        if(flags == null) {
            flags = new List<GameObject>();
        }
        flags.Add(flag);
    }

    [Command]
    private void CmdSpawnBase() {
        baseCore = Instantiate(basePrefab, Vector3.zero, Quaternion.identity);
        playground = baseCore.GetComponent<Playground>();
        NetworkServer.Spawn(baseCore);
        foreach (ClientConnection clientConnection in clientConnection.clients) {
            clientConnection.client.SetClientBaseServer(baseCore);
        }
    }

    [ClientRpc]
    private void RpcSyncBaseOnce(Quaternion rot, GameObject go) {
        if (!isLocalPlayer) { return; }
        baseCore = go;
        OnBasePlaced.Invoke(go);
    }

    [Command]
    private void CmdSpawnUnit(Vector3 pos, UnitType unit) {
        GameObject go = null;
        foreach (TypeToPrefab typeToPrefab in typeToPrefabs) {
            if(typeToPrefab.type == unit) {
                go = Instantiate(typeToPrefab.gameObject);
            }
        }
        if (go != null) {
            go.transform.parent = baseCore.transform;
            go.transform.localPosition = pos;
            go.transform.localRotation = Quaternion.identity;
            NetworkServer.Spawn(go);
            RpcSyncUnitOnce(go.transform.localPosition, go.transform.localRotation, go, baseCore);
            if (warriors == null) {
                warriors = new List<GameObject>();
            }
            warriors.Add(go);
        }
    }

    [Command]
    private void CmdSpawnTeamUnit(TeamType team) {
        for (int i = 0; i < teamToGameObjects.Length; i++) {
            if(teamToGameObjects[i].team == team) {
                GameObject unit = Instantiate(teamToGameObjects[i].prefab, Vector3.zero, Quaternion.identity);
                unit.transform.parent = baseCore.transform;
                for (int j = 0; j < playground.teamToSpawnPoint.Length; j++) {
                    if(playground.teamToSpawnPoint[j].team == team) {
                        unit.transform.position = playground.teamToSpawnPoint[j].prefab.transform.position;
                        break;
                    }
                }

                NetworkServer.Spawn(unit);
                RpcSyncUnitOnce(unit.transform.localPosition, unit.transform.localRotation, unit, baseCore);

                if (warriors == null) {
                    warriors = new List<GameObject>();
                }
                warriors.Add(unit);
                Debug.Log("Team unit spawned");

                break;
            }
        }
    }

    public void RespawnUnitServer(TeamType team) {
        for (int i = 0; i < warriors.Count; i++) {
            if(warriors[i].GetComponentInChildren<Humanoid>().team == team) {
                for (int j = 0; j < playground.teamToSpawnPoint.Length; j++) {
                    if(playground.teamToSpawnPoint[j].team == team) {
                        warriors[i].transform.position = playground.teamToSpawnPoint[j].prefab.transform.position;
                    }
                }
                RpcSyncUnitOnce(warriors[i].transform.localPosition, warriors[i].transform.localRotation, warriors[i], baseCore);
            }
        }
    }

    [ClientRpc]
    private void RpcSyncUnitOnce(Vector3 localPos, Quaternion localRot, GameObject go, GameObject parent) {
        if(!isLocalPlayer) { return; }
        go.transform.parent = parent.transform;
        go.transform.localPosition = localPos;
        go.transform.localRotation = localRot;

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
