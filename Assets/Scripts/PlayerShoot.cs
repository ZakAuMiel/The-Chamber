using UnityEngine;
using Mirror;

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

    [Command]
    void CmdOnHit(Vector3 pos, Vector3 normal)
    {
        RpcDoHitEffect(pos, normal);
    }

    [ClientRpc]
    void RpcDoHitEffect(Vector3 pos, Vector3 normal)
    {
        GameObject hitEffect = Instantiate(weaponManager.GetCurrentGraphics().hitEffectPrefab, pos, Quaternion.LookRotation(normal));
        Destroy(hitEffect, 2f);
    }

    [Command]
    void CmdOnShoot(Vector3 targetPosition)
    {
        RpcDoShootEffect(targetPosition);
    }

    [ClientRpc]
    void RpcDoShootEffect(Vector3 targetPosition)
    {
        // Utilisez muzzlePoint pour positionner la balle correctement
        Transform muzzlePoint = weaponManager.GetCurrentGraphics().bulletSpawn;
        GameObject bullet = Instantiate(bulletPrefab, muzzlePoint.position, muzzlePoint.rotation);
        BulletController bulletController = bullet.GetComponent<BulletController>();
        bulletController.Initialize(targetPosition);

        weaponManager.GetCurrentGraphics().muzzleFlash.Play();

        AudioSource audioSource = GetComponent<AudioSource>();
        audioSource.PlayOneShot(currentWeapon.shootSound);
    }

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
