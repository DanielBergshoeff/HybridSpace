using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Networking;

[CreateAssetMenu(fileName = "NewClientConnection", menuName = "Backend/Client Connection", order = 100)]
public class ClientConnection : ScriptableObject
{
    private static List<ClientConnection> Clients;

    public List<ClientConnection> clients
    {
        get
        {
            if (Clients == null)
            {
                Clients = new List<ClientConnection>();
            }
            return Clients;
        }
        set
        {
            Clients = value;
        }
    }

    // new functionality
    public delegate void ConnectionChange(ClientConnection connection);
    public ConnectionChange OnClientAdded;
    public ConnectionChange OnGameRequested;
    public ConnectionChange OnDisconnected;
    public ConnectionChange OnReconnected;
    public ConnectionChange OnGameFinished;
    
    public NetworkConnection networkConnection;
    public Client client;
    public bool isConnected;
    public int connectionID;
    public string address;
    
    public int teamScore { get; private set; }


    public ClientConnection Get(NetworkConnection clientConnection, Client client)
    {
        Debug.Log("connections available: " + clients.Count);
        foreach (var connection in clients)
        {
            if (connection.address == clientConnection.address && !connection.isConnected)
            {
                Debug.Log("reconnected client connection " + clientConnection.connectionId + " from ip: " + clientConnection.address);
                connection.networkConnection = clientConnection;
                connection.isConnected = true;
                connection.client = client;

                if (connection.OnReconnected != null)
                {
                    connection.OnReconnected(connection);
                }
                return connection;
            }
        }
        Debug.Log("added client connection " + clientConnection.connectionId + " from ip: " + clientConnection.address);
        ClientConnection newConnection = Instantiate(this);
        newConnection.networkConnection = clientConnection;
        newConnection.address = clientConnection.address;
        newConnection.connectionID = clientConnection.connectionId;
        newConnection.isConnected = true;
        newConnection.client = client;

        clients.Add(newConnection);
        if (OnClientAdded != null)
        {
            OnClientAdded(newConnection);
        }
        return newConnection;
    }

    // Called by CustomNetworkManager
    public void Disconnected()
    {
        //BuildDebugger.Log("lost client connection " + connectionID + " from ip: " + address);
        isConnected = false;
        /*
        foreach (Team temp in data.teams.teams)
        {
            if (temp.teamName == currentTeam.teamName)
            {
                temp.chosen = false;
                teamJoined.Raise();
            }
        }*/

        if (OnDisconnected != null)
        {
            OnDisconnected(this);
        }
    }

    // Call to permanently remove client from server
    public void Remove()
    {
        //Debug.Log("removed client connection for team: " + currentTeam.teamName);
        clients.Remove(this);
        Destroy(this);
    }

    public void SetPersistantScore(int score) {
        teamScore = score;
    }
}