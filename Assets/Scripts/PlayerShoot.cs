using UnityEngine;
using Mirror;

/// <summary>
/// Manages the shooting mechanics of the player, including firing bullets and handling hit effects.
/// </summary>
[RequireComponent(typeof(WeaponManager))]
public class PlayerShoot : NetworkBehaviour
{
    [SerializeField]
    private Camera cam;

    [SerializeField]
    private LayerMask mask;

    [SerializeField]
    private GameObject bulletPrefab; // Référence au prefab de la balle

    private WeaponData currentWeapon;
    private WeaponManager weaponManager;

    void Start()
    {
        if (cam == null)
        {
            Debug.LogError("Pas de caméra renseignée sur le système de tir.");
            this.enabled = false;
        }

        weaponManager = GetComponent<WeaponManager>();
    }

    private void Update()
    {
        currentWeapon = weaponManager.GetCurrentWeapon();

        if (PauseMenu.isOn)
        {
            return;
        }

        if (Input.GetKeyDown(KeyCode.R) && weaponManager.currentMagazineSize < currentWeapon.magazineSize)
        {
            StartCoroutine(weaponManager.Reload());
            return;
        }

        if (currentWeapon.fireRate <= 0f)
        {
            if (Input.GetButtonDown("Fire1"))
            {
                Shoot();
            }
        }
        else
        {
            if (Input.GetButtonDown("Fire1"))
            {
                InvokeRepeating("Shoot", 0f, 1f / currentWeapon.fireRate);
            }
            else if (Input.GetButtonUp("Fire1"))
            {
                CancelInvoke("Shoot");
            }
        }
    }

    /// <summary>
    /// Command to notify the server of a hit.
    /// </summary>
    /// <param name="pos">The position of the hit.</param>
    /// <param name="normal">The normal vector at the hit point.</param>
    [Command]
    void CmdOnHit(Vector3 pos, Vector3 normal)
    {
        RpcDoHitEffect(pos, normal);
    }

    /// <summary>
    /// Client RPC to perform hit effects on all clients.
    /// </summary>
    /// <param name="pos">The position of the hit.</param>
    /// <param name="normal">The normal vector at the hit point.</param>
    [ClientRpc]
    void RpcDoHitEffect(Vector3 pos, Vector3 normal)
    {
        GameObject hitEffect = Instantiate(weaponManager.GetCurrentGraphics().hitEffectPrefab, pos, Quaternion.LookRotation(normal));
        Destroy(hitEffect, 2f);
    }

    /// <summary>
    /// Command to notify the server of a shot.
    /// </summary>
    /// <param name="targetPosition">The target position of the shot.</param>
    [Command]
    void CmdOnShoot(Vector3 targetPosition)
    {
        RpcDoShootEffect(targetPosition);
    }

    /// <summary>
    /// Client RPC to perform shooting effects on all clients.
    /// </summary>
    /// <param name="targetPosition">The target position of the shot.</param>
    [ClientRpc]
    void RpcDoShootEffect(Vector3 targetPosition)
    {
        // Utilisez bulletSpawn pour positionner la balle correctement
        Transform bulletSpawn = weaponManager.GetCurrentGraphics().bulletSpawn;
        GameObject bullet = Instantiate(bulletPrefab, bulletSpawn.position, bulletSpawn.rotation);
        BulletController bulletController = bullet.GetComponent<BulletController>();
        bulletController.Initialize(targetPosition);

        weaponManager.GetCurrentGraphics().muzzleFlash.Play();

        AudioSource audioSource = GetComponent<AudioSource>();
        audioSource.PlayOneShot(currentWeapon.shootSound);
    }

    /// <summary>
    /// Handles the shooting mechanics on the client side.
    /// </summary>
    [Client]
    private void Shoot()
    {
        if (!isLocalPlayer || weaponManager.isReloading)
        {
            return;
        }

        if (weaponManager.currentMagazineSize <= 0)
        {
            StartCoroutine(weaponManager.Reload());
            return;
        }

        weaponManager.currentMagazineSize--;

        Debug.Log("Il nous reste " + weaponManager.currentMagazineSize + " balles dans le chargeur.");

        RaycastHit hit;
        Vector3 targetPosition;

        if (Physics.Raycast(cam.transform.position, cam.transform.forward, out hit, currentWeapon.range, mask))
        {
            targetPosition = hit.point;

            if (hit.collider.tag == "Player")
            {
                CmdPlayerShot(hit.collider.name, currentWeapon.damage, transform.name);
            }

            CmdOnHit(hit.point, hit.normal);
        }
        else
        {
            targetPosition = cam.transform.position + cam.transform.forward * currentWeapon.range;
        }

        CmdOnShoot(targetPosition);
    }

    /// <summary>
    /// Command to notify the server that a player has been shot.
    /// </summary>
    /// <param name="playerId">The ID of the player who was shot.</param>
    /// <param name="damage">The amount of damage dealt.</param>
    /// <param name="sourceID">The ID of the player who shot.</param>
    [Command]
    private void CmdPlayerShot(string playerId, float damage, string sourceID)
    {
        Debug.Log(playerId + " a été touché.");

        Player player = GameManager.GetPlayer(playerId);
        if (player != null)
        {
            player.RpcTakeDamage(damage, sourceID);
        }
        else
        {
            Debug.LogError($"Le joueur avec l'ID '{playerId}' n'a pas été trouvé dans GameManager.");
        }
    }
}
