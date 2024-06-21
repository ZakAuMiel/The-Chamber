using UnityEngine;
using Mirror;

/// <summary>
/// Manages the setup and initialization of the player, including UI and network registration.
/// </summary>
[RequireComponent(typeof(Player))]
[RequireComponent(typeof(PlayerController))]
public class PlayerSetup : NetworkBehaviour
{
    [SerializeField]
    Behaviour[] componentsToDisable;

    [SerializeField]
    private string remoteLayerName = "RemotePlayer";

    [SerializeField]
    private string dontDrawLayerName = "DontDraw";

    [SerializeField]
    private GameObject playerGraphics;

    [SerializeField]
    private GameObject playerNameplateGraphics;

    [SerializeField]
    private GameObject playerUIPrefab;

    [HideInInspector]
    public GameObject playerUIInstance;

    private string playerID;

    private void Start()
    {
        if (!isLocalPlayer)
        {
            DisableComponents();
            AssignRemoteLayer();
        }
        else
        {
            // Désactiver la partie graphique du joueur local
            Util.SetLayerRecursively(playerGraphics, LayerMask.NameToLayer(dontDrawLayerName));
            Util.SetLayerRecursively(playerNameplateGraphics, LayerMask.NameToLayer(dontDrawLayerName));

            // Création du UI du joueur local
            playerUIInstance = Instantiate(playerUIPrefab);

            // Configuration du UI
            PlayerUI ui = playerUIInstance.GetComponent<PlayerUI>();
            if (ui == null)
            {
                Debug.LogError("Pas de component PlayerUI sur playerUIInstance");
            }
            else
            {
                ui.SetPlayer(GetComponent<Player>());
            }

            GetComponent<Player>().Setup();

            // Activer la caméra de la scène
            GameManager.instance.SetSceneCameraActive(false);
        }
    }

    [Command]
    void CmdSetUsername(string playerID, string username)
    {
        Player player = GameManager.GetPlayer(playerID);
        if (player != null)
        {
            Debug.Log(username + " has joined !");
            player.username = username;
        }
    }

    public override void OnStartClient()
    {
        base.OnStartClient();
        RegisterPlayerAndSetUsername();
    }

    public override void OnStartServer()
    {
        base.OnStartServer();
    }

    private void RegisterPlayerAndSetUsername()
    {
        Player player = GetComponent<Player>();
        GameManager.RegisterPlayer(player);

        // Récupérer l'ID généré et l'affecter au joueur
        playerID = player.transform.name;

        if (isLocalPlayer)
        {
            CmdSetUsername(playerID, UserAccountManager.LoggedInUsername);
        }
    }

    private void AssignRemoteLayer()
    {
        gameObject.layer = LayerMask.NameToLayer(remoteLayerName);
    }

    private void DisableComponents()
    {
        for (int i = 0; i < componentsToDisable.Length; i++)
        {
            componentsToDisable[i].enabled = false;
        }
    }

    private void OnDisable()
    {
        Destroy(playerUIInstance);

        if (isLocalPlayer)
        {
            GameManager.instance.SetSceneCameraActive(true);
        }

        if (playerID != null)
        {
            GameManager.UnregisterPlayer(playerID);
        }
    }
}
