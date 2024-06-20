using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class GameManager : MonoBehaviour
{
    private const string playerIdPrefix = "Player";

    private static Dictionary<string, Player> players = new Dictionary<string, Player>();

    public MatchSettings matchSettings;

    public static GameManager instance;

    [SerializeField]
    private GameObject sceneCamera;

    public delegate void OnPlayerKilledCallback(string player, string source);
    public OnPlayerKilledCallback onPlayerKilledCallback;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            return;
        }

        Debug.LogError("Plus d'une instance de GameManager dans la scène");
    }

    public void SetSceneCameraActive(bool isActive)
    {
        if (sceneCamera == null)
        {
            return;
        }

        sceneCamera.SetActive(isActive);
    }

    public static void RegisterPlayer(string netID, Player player)
    {
        string playerId = playerIdPrefix + netID;
        if (!players.ContainsKey(playerId))
        {
            players.Add(playerId, player);
            player.transform.name = playerId;
        }
        else
        {
            Debug.LogError($"Le joueur avec l'ID '{playerId}' est déjà enregistré.");
        }
    }

    public static void UnregisterPlayer(string playerId)
    {
        if (players.ContainsKey(playerId))
        {
            players.Remove(playerId);
        }
        else
        {
            Debug.LogError($"Impossible de désenregistrer le joueur avec l'ID '{playerId}' car il n'existe pas.");
        }
    }

    public static Player GetPlayer(string playerId)
    {
        if (players.ContainsKey(playerId))
        {
            return players[playerId];
        }
        else
        {
            Debug.LogError($"L'ID du joueur '{playerId}' n'a pas été trouvé dans le dictionnaire.");
            return null; // ou gérez l'erreur de manière appropriée
        }
    }

    public static Player[] GetAllPlayers()
    {
        return players.Values.ToArray();
    }
}
