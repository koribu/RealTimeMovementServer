using UnityEngine;

static public class NetworkedServerProcessing
{

    #region Send and Receive Data Functions
    static public void ReceivedMessageFromClient(string msg, int clientConnectionID)
    {
        Debug.Log("msg received = " + msg + ".  connection id = " + clientConnectionID);

        string[] csv = msg.Split(',');
        int signifier = int.Parse(csv[0]);

        if (signifier == ClientToServerSignifiers.SendVelocityChangeSignifier)
        {
            Vector2 positionChange = new Vector2(float.Parse(csv[1]), float.Parse(csv[2])) ;
            Debug.LogError("****" + positionChange.x + " + " + positionChange.y);

            gameLogic.UpdatePlayerMovement(positionChange, clientConnectionID);
        }
        // else if (signifier == ClientToServerSignifiers.asd)
        // {

        // }

        //gameLogic.DoSomething();
    }
    static public void SendMessageToClient(string msg, int clientConnectionID)
    {
        networkedServer.SendMessageToClient(msg, clientConnectionID);
    }

    static public void SendMessageToClientWithSimulatedLatency(string msg, int clientConnectionID)
    {
        networkedServer.SendMessageToClientWithSimulatedLatency(msg, clientConnectionID);
    }

  
    
    #endregion

    #region Connection Events

    static public void ConnectionEvent(int clientConnectionID)
    {
        Debug.Log("New Connection, ID == " + clientConnectionID);


        gameLogic.CreateNewPlayer(clientConnectionID);
        
    }
    static public void DisconnectionEvent(int clientConnectionID)
    {
        Debug.Log("New Disconnection, ID == " + clientConnectionID);
        gameLogic.DestroyPlayer(clientConnectionID);
    }

    #endregion

    #region Setup
    static NetworkedServer networkedServer;
    static GameLogic gameLogic;

    static public void SetNetworkedServer(NetworkedServer NetworkedServer)
    {
        networkedServer = NetworkedServer;
    }
    static public NetworkedServer GetNetworkedServer()
    {
        return networkedServer;
    }
    static public void SetGameLogic(GameLogic GameLogic)
    {
        gameLogic = GameLogic;
    }

    #endregion
}

#region Protocol Signifiers
static public class ClientToServerSignifiers
{
    public const int SendVelocityChangeSignifier = 1;
}

static public class ServerToClientSignifiers
{
    public const int UpdatePlayersWithPositionChange = 1;
    public const int CreatePlayerCharacter = 2;
    public const int CreateNewOtherCharacter = 3;
    public const int CreateExistOtherCharacter = 4;
    public const int DeletePlayerCharacter = 5;
}

#endregion