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
    private static ClientConnection clientConnection;
    
    [SerializeField] private GameObject basePrefab;
    [SerializeField] private GameObject flagPrefab;
    [SerializeField] private GameObject unitPrefab;

    [SerializeField] private TypeToPrefab[] typeToPrefabs;

    [SerializeField] private TeamToGameObject[] teamToGameObjects;

    private static Playground playground;

    public static MyGameObjectEvent OnBasePlaced;

    public static GameObject baseCore;

    public static GameObject egg;
    private static List<GameObject> warriors;
    private static List<GameObject> flags;
    
    public static TeamType team;

    public static Client LocalClient {
        get;
        private set;
    }

    private void Update() {

    }

    public void BoostUnit(TeamType teamType) {
        Debug.Log("Send boost to server");
        CmdBoostUnitServer(teamType);
    }

    public static void GameOver(TeamType winningTeam) {
        GameManager.GameOver = true;
        for (int i = 0; i < warriors.Count; i++) {
            if(warriors[i].GetComponentInChildren<Humanoid>().team == winningTeam) {
                warriors[i].GetComponentInChildren<Humanoid>().pointText.text = "YOU WON";
            }
            else {
                warriors[i].GetComponentInChildren<Humanoid>().pointText.text = "YOU LOST";
            }
        }
    }

    [ClientRpc]
    private void RpcGameOver(TeamType winningTeam) {
        GameManager.GameOver = true;
        for (int i = 0; i < warriors.Count; i++) {
            if (warriors[i].GetComponentInChildren<Humanoid>().team == winningTeam) {
                warriors[i].GetComponentInChildren<Humanoid>().pointText.text = "YOU WON";
            }
            else {
                warriors[i].GetComponentInChildren<Humanoid>().pointText.text = "YOU LOST";
            }
        }
    }

    [Command]
    public void CmdBoostUnitServer(TeamType teamType) {
        Debug.Log("Boost arrived at server for "+ teamType);

        for (int i = 0; i < warriors.Count; i++) {
            if (warriors[i].GetComponentInChildren<Humanoid>().team == teamType) {
                warriors[i].GetComponentInChildren<Humanoid>().SetSpeed(5.0f, 2.0f);
                Debug.Log("New speed has been set");
            }
        }
    }

    public static void SetEggParent(TeamType team, Vector3 position) {
        foreach (ClientConnection client in clientConnection.clients) {
            client.client.SetEggParentServer(team, position);
        }
    }

    private void SetEggParentServer(TeamType team, Vector3 position) {
        RpcSetEggParent(team, position);
    }

    [ClientRpc]
    private void RpcSetEggParent(TeamType teamType, Vector3 position) {
        if (teamType == TeamType.Null) {
            egg.transform.parent = baseCore.transform;
            egg.transform.localPosition = position;
        }
        else {
            for (int i = 0; i < warriors.Count; i++) {
                if (warriors[i].GetComponentInChildren<Humanoid>().team == teamType) {
                    egg.transform.parent = warriors[i].GetComponentInChildren<Humanoid>().transform;
                    egg.transform.localPosition = position;
                }
            }
        }

        
    }

    public static void SyncUnits() {
        if (warriors != null) {
            for (int i = 0; i < warriors.Count; i++) {
                if (warriors[i].transform.position != flags[i].transform.position) {
                    if (warriors[i].GetComponentInChildren<Humanoid>().stunTimer <= 0.0f) {
                        if (Vector3.Distance(warriors[i].transform.position, flags[i].transform.position) > 0.01f) {
                            warriors[i].transform.LookAt(flags[i].transform.position);
                            warriors[i].transform.position = Vector3.MoveTowards(warriors[i].transform.position, flags[i].transform.position, warriors[i].GetComponentInChildren<Humanoid>().speed * Time.deltaTime);
                            foreach (ClientConnection client in clientConnection.clients) {
                                client.client.SyncUnitsServer(i);
                            }
                        }
                    }
                }
            }
        }
    }

    public static void SyncUnitOnce(TeamType teamType) {
        for (int i = 0; i < warriors.Count; i++) {
            if (warriors[i].GetComponentInChildren<Humanoid>().team == teamType) {
                foreach (ClientConnection client in clientConnection.clients) {
                    client.client.SyncUnitsServer(i);
                }
            }
        }
    }

    public void SyncUnitsServer(int i) {
        RpcSyncGameObject(i, warriors[i].transform.localPosition, warriors[i].transform.localRotation);
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

    public static void SyncUnitPoints(TeamType teamtype, float points) {
        for (int i = 0; i < warriors.Count; i++) {
            if(warriors[i].GetComponentInChildren<Humanoid>().team == teamtype) {
                foreach (ClientConnection client in clientConnection.clients) {
                    client.client.RpcSyncUnitPoints(i, points);
                }
            }
        }
    }

    [ClientRpc]
    private void RpcSyncUnitPoints(int index, float newpoints) {
        Debug.Log(index);
        Debug.Log(newpoints);
        warriors[index].GetComponentInChildren<Humanoid>().points = newpoints;
    }

    // Create custom persistance instance
    [Command]
    private void CmdGetConnection(string adress) {
        //Debug.Log("Player IP posted by player: " + adress);
        NetworkConnection conn = connectionToClient;
        conn.address = adress;
        clientConnection = clientConnectionBase.Get(conn, this);
    }

    /*
    public void SpawnUnitClient(Vector3 pos, UnitType type) {
        if (!isLocalPlayer) { return; }
        CmdSpawnUnit(pos, type);
        CmdSpawnFlag(pos);
    }*/

    public void SpawnUnitClient(TeamType team) {
        Client.team = team;
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
        egg = baseCore.GetComponentInChildren<Egg>().gameObject;
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
        playground = baseCore.GetComponent<Playground>();
        egg = baseCore.GetComponentInChildren<Egg>().gameObject;
        OnBasePlaced.Invoke(go);
    }

    /*
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
    }*/

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
                foreach (ClientConnection clientConnection in clientConnection.clients) {
                    clientConnection.client.SetClientUnitServer(unit.transform.localPosition, unit.transform.localRotation, unit, baseCore);
                }

                if (warriors == null) {
                    warriors = new List<GameObject>();
                }
                warriors.Add(unit);
                Debug.Log("Team unit spawned");

                break;
            }
        }
    }

    private void SetClientUnitServer(Vector3 localPos, Quaternion localRot, GameObject go, GameObject parent) {
        RpcSyncUnitOnce(localPos, localRot, go, parent);
    }

    public static void RespawnUnitServer(TeamType team) {
        for (int i = 0; i < warriors.Count; i++) {
            if(warriors[i].GetComponentInChildren<Humanoid>().team == team) {
                for (int j = 0; j < playground.teamToSpawnPoint.Length; j++) {
                    if(playground.teamToSpawnPoint[j].team == team) {
                        warriors[i].transform.position = playground.teamToSpawnPoint[j].prefab.transform.position;
                    }
                }
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
        warriors[index].GetComponentInChildren<Humanoid>().animator.SetTrigger("Walk");
    }

    [ClientRpc]
    private void RpcSyncEgg(Vector3 localPos, Quaternion rotation) {
        if(!isLocalPlayer) { return; }
        egg.transform.position = baseCore.transform.position + localPos;
        egg.transform.rotation = rotation;
    }

    // Called when Client loses connection, Clientside
    public void OnConnectionLost()
    {
        
    }
}
