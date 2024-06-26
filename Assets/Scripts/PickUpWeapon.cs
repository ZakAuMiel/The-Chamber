using UnityEngine;
using Mirror;
using System.Collections;

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

        if (pickUpGraphics != null)
        {
            Destroy(pickUpGraphics);
        }

        pickUpGraphics = Instantiate(theWeapon.graphics, transform);
        pickUpGraphics.transform.position = transform.position;
        canPickUp = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && canPickUp)
        {
            Player player = other.GetComponent<Player>();
            if (player != null && player.isLocalPlayer)
            {
                player.CmdPickUpWeapon(theWeapon.weaponID);
                canPickUp = false;
                Destroy(pickUpGraphics);
                StartCoroutine(DelayResetWeapon());
            }
        }
    }

    IEnumerator DelayResetWeapon()
    {
        yield return new WaitForSeconds(respawnDelay);
        ResetWeapon();
    }
}
