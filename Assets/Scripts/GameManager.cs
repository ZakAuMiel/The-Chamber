using System.Collections.Generic;
using UnityEngine;
using System.Linq;

/// <summary>
/// Manages the registration and tracking of players within the game.
/// </summary>
public class GameManager : MonoBehaviour
{
    private static Dictionary<string, Player> players = new Dictionary<string, Player>();
    private static int playerCounter = 0;

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
        }
        else
        {
            Debug.LogError("Plus d'une instance de GameManager dans la scène");
            Destroy(gameObject);
        }
    }

    public void SetSceneCameraActive(bool isActive)
    {
        if (sceneCamera == null)
        {
            return;
        }

        sceneCamera.SetActive(isActive);
    }

    public static void RegisterPlayer(Player player)
    {
        string playerId = "Player" + playerCounter;
        playerCounter++;

        if (!players.ContainsKey(playerId))
        {
            players.Add(playerId, player);
            player.transform.name = playerId;
            Debug.Log("Player registered: " + playerId);
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
            Debug.Log("Player unregistered: " + playerId);
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
            Debug.Log("Player found: " + playerId);
            return players[playerId];
        }
        else
        {
            Debug.LogError($"L'ID du joueur '{playerId}' n'a pas été trouvé dans le dictionnaire.");
            return null;
        }
    }

    public static Player[] GetAllPlayers()
    {
        return players.Values.ToArray();
    }
}
