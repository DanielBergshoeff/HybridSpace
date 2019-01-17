#define MATCHMAKER

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Networking.Match;
using UnityEngine.SceneManagement;

public class CustomNetworkManager : NetworkManager {

    public ClientConnection clientConnection;

    private bool isConnected;
    private string myMatchName = "GodsMatch";

    //   private string generatedMatchName = "testMatchRuben";

    private void Start()
    {
#if MATCHMAKER
        StartMatchMaker();
#endif
    }

    // Call this method to start Server (I called this from buttons)
    public void TryStartServer()
    {
#if MATCHMAKER
        //generatedMatchName = data.school.schoolName + "#" + data.group.groupName;
        matchMaker.CreateMatch(myMatchName, 10, true, "", "", "", 0, 0, OnInternetMatchCreate);
#else
        StartServer();
#endif
    }

    // Call this method to connect to Server from Client (I called this from buttons)
    public void TryConnectToServer()
    {

        isConnected = false;
#if MATCHMAKER
        if (matchMaker == null)
        {
            StartMatchMaker();
        }
        matchMaker.ListMatches(0, 10, myMatchName, true, 0, 0, OnInternetMatchList);
#else
        StartClient();
#endif
    }

    // Method when Server is Created without Matchmaker (?)
    public override void OnStartServer()
    {
        Debug.Log("Server creation succeeded");
        connectionConfig.NetworkDropThreshold = 45;
        connectionConfig.OverflowDropThreshold = 45;
        connectionConfig.AckDelay = 200;
        connectionConfig.AcksType = ConnectionAcksType.Acks128;
        connectionConfig.MaxSentMessageQueueSize = 300;
        //onServerStarted.Raise();
    }

    // Method when Server is Stopped without Matchmaker (?)
    public override void OnStopServer()
    {
        Debug.Log("Server stopped");
    }

    // Callback for creating match through Unity Matchmaker, Serverside
    private void OnInternetMatchCreate(bool success, string extendedInfo, MatchInfo responseData)
    {
        if (success)
        {
            NetworkServer.Listen(responseData, responseData.port);
            Debug.Log(responseData.port);

            var serverStarted = StartServer(responseData);
            if (!serverStarted)
            {
                Debug.Log("Server creation failed");
            }
        }
        else
        {
            Debug.Log("Could not create match on Unity Matchmaker");
        }
    }

    // Callback for requesting existing matches through Unity Matchmaker, Clientside
    private void OnInternetMatchList(bool success, string extendedInfo, List<MatchInfoSnapshot> matches)
    {
        if (success)
        {
            if (matches.Count != 0)
            {
                matchMaker.JoinMatch(matches[matches.Count - 1].networkId, "", "", "", 0, 0, OnJoinInternetMatch);
            }
            else
            {
                Debug.Log("No running matches found");
                //onConnectionLost.Raise();
            }
        }
        else
        {
            Debug.Log("Couldn't connect to Unity Matchmaker");
            //onConnectionLost.Raise();
        }
    }

    // Callback for joining match through Unity Matchmaker, Clientside
    private void OnJoinInternetMatch(bool success, string extendedInfo, MatchInfo responseData)
    {
        if (success)
        {
            StartClient(responseData);
            Debug.Log("Joining match");
        }
        else
        {
            Debug.Log("Join match failed");
        }
    }

    // When Client disconnects from Server, executes on Server
    public override void OnServerDisconnect(NetworkConnection conn)
    {
        base.OnServerDisconnect(conn);
        foreach (var connection in clientConnection.clients)
        {
            if (connection.networkConnection == conn)
            {
                connection.Disconnected();
                return;
            }
        }
    }

    // When Client connects to Server, executes on Server
    public override void OnServerConnect(NetworkConnection conn)
    {
        base.OnServerConnect(conn);
        Debug.Log("Client connected with IP: " + conn.address);
    }

    // When Client disconnects from Server, executes on Client
    public override void OnClientDisconnect(NetworkConnection conn)
    {
        base.OnClientDisconnect(conn);
        // was never connected
        if (conn.lastError == NetworkError.Timeout && !isConnected)
        {
            //BuildDebugger.Log("Couldn't connect to server, is server running?");
        }
        // was connected to server but lost connection
        else
        {
            //BuildDebugger.Log("Lost connection to server");
            //sceneManagement.Reset();
            SceneManager.LoadScene(0);
        }
    }

    // When Client connects to Server, executes on Client
    public override void OnClientConnect(NetworkConnection conn) {
        base.OnClientConnect(conn);
        isConnected = true;
    }
}
