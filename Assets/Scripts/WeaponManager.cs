using UnityEngine;
using Mirror;
using System.Collections;
using System.Collections.Generic;

public class WeaponManager : NetworkBehaviour
{
    [SerializeField]
    private string primaryWeaponID;

    [SyncVar]
    private string currentWeaponID;

    private WeaponData currentWeapon;
    private WeaponGraphics currentGraphics;

    [SerializeField]
    private Transform weaponHolder;

    [SerializeField]
    private string weaponLayerName = "Weapon";

    [HideInInspector]
    public int currentMagazineSize;

    public bool isReloading = false;

    [SerializeField]
    private WeaponData[] weaponDataArray; // Ajoutez cette référence

    private WeaponDatabase weaponDatabase;

    /// <summary>
    /// Initialisation du WeaponManager. Équipe l'arme primaire par défaut.
    /// </summary>
    void Start()
    {
        weaponDatabase = new WeaponDatabase(weaponDataArray);
        EquipWeapon(primaryWeaponID);
    }

    /// <summary>
    /// Obtient l'arme actuellement équipée.
    /// </summary>
    /// <returns>Les données de l'arme actuellement équipée.</returns>
    public WeaponData GetCurrentWeapon()
    {
        return currentWeapon;
    }

    /// <summary>
    /// Obtient les graphiques de l'arme actuellement équipée.
    /// </summary>
    /// <returns>Le composant WeaponGraphics de l'arme actuellement équipée.</returns>
    public WeaponGraphics GetCurrentGraphics()
    {
        return currentGraphics;
    }

    /// <summary>
    /// Commande envoyée au serveur pour équiper une nouvelle arme.
    /// </summary>
    /// <param name="weaponID">L'ID de la nouvelle arme à équiper.</param>
    [Command]
    public void CmdEquipWeapon(string weaponID)
    {
        RpcEquipWeapon(weaponID);
    }

    /// <summary>
    /// RPC appelé sur tous les clients pour équiper une nouvelle arme.
    /// </summary>
    /// <param name="weaponID">L'ID de la nouvelle arme à équiper.</param>
    [ClientRpc]
    void RpcEquipWeapon(string weaponID)
    {
        EquipWeapon(weaponID);
    }

    /// <summary>
    /// Équipe une nouvelle arme pour le joueur local.
    /// </summary>
    /// <param name="weaponID">L'ID de la nouvelle arme à équiper.</param>
    public void EquipWeapon(string weaponID)
    {
        currentWeapon = weaponDatabase.GetWeaponDataByID(weaponID);
        if (currentWeapon == null)
        {
            Debug.LogError("Weapon with ID " + weaponID + " not found in the database.");
            return;
        }
        
        currentWeaponID = weaponID;
        currentMagazineSize = currentWeapon.magazineSize;

        if (currentGraphics != null)
        {
            Destroy(currentGraphics.gameObject);
        }

        GameObject weaponIns = Instantiate(currentWeapon.graphics, weaponHolder.position, weaponHolder.rotation);
        weaponIns.transform.SetParent(weaponHolder);

        currentGraphics = weaponIns.GetComponent<WeaponGraphics>();

        if (currentGraphics == null)
        {
            Debug.LogError("Pas de script WeaponGraphics sur l'arme : " + weaponIns.name);
        }

        if (isLocalPlayer)
        {
            Util.SetLayerRecursively(weaponIns, LayerMask.NameToLayer(weaponLayerName));
        }
    }

    /// <summary>
    /// Coroutine pour recharger l'arme actuelle.
    /// </summary>
    /// <returns>IEnumerator pour la coroutine.</returns>
    public IEnumerator Reload()
    {
        if (isReloading)
        {
            yield break;
        }

        Debug.Log("Reloading ...");

        isReloading = true;

        CmdOnReload();
        yield return new WaitForSeconds(currentWeapon.reloadTime);
        currentMagazineSize = currentWeapon.magazineSize;

        isReloading = false;

        Debug.Log("Reloading finished");
    }

    /// <summary>
    /// Commande envoyée au serveur pour informer les autres clients que le rechargement commence.
    /// </summary>
    [Command]
    void CmdOnReload()
    {
        RpcOnReload();
    }

    /// <summary>
    /// RPC appelé sur tous les clients pour déclencher les animations et sons de rechargement.
    /// </summary>
    [ClientRpc]
    void RpcOnReload()
    {
        Animator animator = currentGraphics.GetComponent<Animator>();
        if (animator != null)
        {
            animator.SetTrigger("Reload");
        }

        PlayReloadSound();
    }

    /// <summary>
    /// Joue le son de rechargement localement.
    /// </summary>
    void PlayReloadSound()
    {
        AudioSource audioSource = GetComponent<AudioSource>();
        if (audioSource != null && currentWeapon.reloadSound != null)
        {
            audioSource.PlayOneShot(currentWeapon.reloadSound);
        }
    }

    /// <summary>
    /// Classe interne pour gérer la base de données des armes.
    /// </summary>
    private class WeaponDatabase
    {
        private Dictionary<string, WeaponData> weaponDataByID;

        public WeaponDatabase(WeaponData[] weaponDataArray)
        {
            InitializeDatabase(weaponDataArray);
        }

        private void InitializeDatabase(WeaponData[] weaponDataArray)
        {
            weaponDataByID = new Dictionary<string, WeaponData>();
            foreach (var weaponData in weaponDataArray)
            {
                if (!weaponDataByID.ContainsKey(weaponData.weaponID))
                {
                    weaponDataByID.Add(weaponData.weaponID, weaponData);
                }
            }
        }

        public WeaponData GetWeaponDataByID(string id)
        {
            weaponDataByID.TryGetValue(id, out var weaponData);
            return weaponData;
        }
    }
}
