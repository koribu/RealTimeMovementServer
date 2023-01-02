using Newtonsoft.Json.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;

public class GameLogic : MonoBehaviour
{

    Dictionary<int,Player> _players = new Dictionary<int, Player>();
    public List<int> _onMovePlayerIDs = new List<int>();

    bool isAnyPlayerUpdated;
    void Start()
    {
        NetworkedServerProcessing.SetGameLogic(this);
    }



    void FixedUpdate()
    {
        UpdatePlayerPositions();
    }

    void UpdatePlayerPositions()
    {
        foreach (int activePlayerID in _onMovePlayerIDs)
        {
            /* if (player.Value.velocity != Vector2.zero)
             {*/

                Player p = _players[activePlayerID];

                p.posInPercent += _players[activePlayerID].velocity;

                _players[activePlayerID] = p;

                Debug.Log("New Pos = " + p.posInPercent);
                string msg = ServerToClientSignifiers.UpdatePlayersWithPositionChange + "," +
                             p.posInPercent.x + "," + p.posInPercent.y + "," + +activePlayerID;

                UpdatePlayers(msg);
            //}
        }
    }

    public void UpdatePlayers(string msg)
    {
        foreach (KeyValuePair<int, Player> player in _players)
        {
            NetworkedServerProcessing.SendMessageToClient( msg,player.Key);
        }
    }

    public void UpdatePlayerMovement( Vector2 velocity, int id)
    {
        Player player = _players[id];
        player.velocity = velocity;

        if(velocity != Vector2.zero) // Keep track for active players for improve performance
        {
            if(!_onMovePlayerIDs.Contains(id))
                 _onMovePlayerIDs.Add(id);
        }
        else if(_onMovePlayerIDs.Contains(id))
        {
            _onMovePlayerIDs.Remove(id);
        }


        _players[id] = player;

        /*     player.posInPercent += velocity;

             _players[id] = player;

             string msg = ServerToClientSignifiers.UpdatePlayersWithPositionChange + "," +
                 player.posInPercent.x + "," + player.posInPercent.y + "," +  + id;

             UpdatePlayers(msg);*/
    }


    public void CreateNewPlayer(int ID)
    {
        Player newPlayer = new Player();
        newPlayer.posInPercent = Vector2.zero;
        newPlayer.velocity = Vector2.zero;

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