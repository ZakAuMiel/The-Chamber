using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class PickUpWeapon : NetworkBehaviour
{
    [SerializeField]
    private WeaponData theWeapon;

    [SerializeField]
    private float respawnDelay = 30f;

    private GameObject pickUpGraphics;
    private bool canPickUp;

    void Start()
    {
        ResetWeapon();
    }

    void ResetWeapon()
    {
        if (theWeapon == null || theWeapon.graphics == null)
        {
            Debug.LogError("WeaponData or Weapon graphics not assigned.");
            return;
        }

        pickUpGraphics = Instantiate(theWeapon.graphics, transform);
        pickUpGraphics.transform.position = transform.position;
        canPickUp = true;
    }

    [ServerCallback]
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && canPickUp)
        {
            WeaponManager weaponManager = other.GetComponent<WeaponManager>();
            if (weaponManager != null)
            {
                EquipNewWeapon(weaponManager);
            }
        }
    }

    [Server]
    void EquipNewWeapon(WeaponManager weaponManager)
    {
        // Détruit l'arme actuelle du joueur
        if (weaponManager.GetCurrentGraphics() != null)
        {
            Destroy(weaponManager.GetCurrentGraphics().gameObject);
        }

        // Equipe la nouvelle arme
        weaponManager.EquipWeapon(theWeapon);

        canPickUp = false;
        if (pickUpGraphics != null)
        {
            Destroy(pickUpGraphics);
        }

        StartCoroutine(DelayResetWeapon());
    }

    IEnumerator DelayResetWeapon()
    {
        yield return new WaitForSeconds(respawnDelay);
        ResetWeapon();
    }
}
