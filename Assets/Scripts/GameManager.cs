using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private const string playerIdPrefix = "Player ";

    private static Dictionary<string, Player> players = new Dictionary<string, Player>();

    public MatchSettings matchSettings;

    public static GameManager instance;

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
            return;
        }

        Debug.LogError("Plus d'une instance de GameManager dans la scène");
    }

    public static void RegisterPlayer(string netID, Player player)
    {
        string playerId = playerIdPrefix + netID;
        players.Add(playerId, player);
        player.transform.name = playerId;
        Debug.Log("Ajout du joueur " + playerId);
    }

    public static void UnregisterPlayer(string playerId)
    {
        players.Remove(playerId);
        Debug.Log("Suppression du joueur " + playerId);
    }

    public static Player GetPlayer(string playerId)
    {
        Debug.Log("Récupération du joueur " + playerId);

        foreach (KeyValuePair<string, Player> player in players)
        {
            Debug.Log("Joueur " + player.Key + " trouvé.");
        }

        return players[playerId];
    }
}