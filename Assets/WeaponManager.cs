using UnityEngine;
using Mirror;

public class WeaponManager : NetworkBehaviour
{
    [SerializeField]
    private PlayerWeapon primaryWeapon;
    [SerializeField]
    private string weaponLayerName = "Weapon";
    [SerializeField]
    private Transform weaponHolder;

    private PlayerWeapon currentWeapon;



    void Start()
    {
        EquipWeapon(primaryWeapon);
    }


    void EquipWeapon(PlayerWeapon _weapon)
    {
        currentWeapon = _weapon;
        GameObject weaponIns = Instantiate(currentWeapon.graphics, weaponHolder.position, weaponHolder.rotation, weaponHolder);
        weaponIns.transform.SetParent(weaponHolder);

        if(isLocalPlayer)
        {
            weaponIns.layer = LayerMask.NameToLayer(weaponLayerName);
        }
        else
        { return;}
    }

    public PlayerWeapon GetCurrentWeapon()
    {
        return currentWeapon;
    }


}
