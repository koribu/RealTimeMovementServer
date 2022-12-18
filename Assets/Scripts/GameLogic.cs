using System.Collections;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;

public class GameLogic : MonoBehaviour
{
    Dictionary<int,Player> _players = new Dictionary<int, Player>();
    void Start()
    {
        NetworkedServerProcessing.SetGameLogic(this);
    }

    void Update()
    {

    }



    public void UpdatePlayers(string msg)
    {
        foreach (KeyValuePair<int, Player> player in _players)
        {
            NetworkedServerProcessing.SendMessageToClientWithSimulatedLatency( msg,player.Key);
        }
    }

    public void UpdatePlayerMovement( Vector2 movement, int id)
    {
        Player player = _players[id];
        player.posInPercent += movement;

        _players[id] = player;

        string msg = ServerToClientSignifiers.UpdatePlayersWithPositionChange + "," +
            player.posInPercent.x + "," + player.posInPercent.y + "," +  + id;

        UpdatePlayers(msg);
    }


    public void CreateNewPlayer(int ID)
    {
        Player newPlayer = new Player();
        newPlayer.posInPercent = Vector2.zero;

        //send new player's Id to other existing players
        string msg = ServerToClientSignifiers.CreateNewOtherCharacter + "," + ID;
        UpdatePlayers(msg);
     
        //Send to the player of it's Id number and let the client create player
        msg = ServerToClientSignifiers.CreatePlayerCharacter + "," + ID;
        NetworkedServerProcessing.SendMessageToClient(msg, ID);

        //Send information of all the existing Player's Id and Position
        foreach (KeyValuePair<int, Player> player in _players)
        {
            msg = ServerToClientSignifiers.CreateExistOtherCharacter + "," + player.Value.posInPercent.x + "," + player.Value.posInPercent.x + "," + player.Key;

            NetworkedServerProcessing.SendMessageToClient(msg, ID);
        }

        _players.Add(ID, newPlayer);
    }

    public void DestroyPlayer(int ID)
    {
        _players.Remove(ID);

        string msg = ServerToClientSignifiers.DeletePlayerCharacter + "," + ID;

        UpdatePlayers(msg);

    }

}