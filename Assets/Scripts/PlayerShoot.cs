using UnityEngine;
using Mirror;

public class PlayerShoot : NetworkBehaviour
{

    public PlayerWeapon weapon;

    [SerializeField]
    private Camera cam;

    [SerializeField]

    private LayerMask mask;

    void Start()
    {
        if (cam == null)
        {
            Debug.LogError("PlayerShoot: No camera referenced!");
            this.enabled = false;
        }
    }

    void Update()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            Shoot();
        }
    }

    [Client]
    private void Shoot()
    {
        RaycastHit hit;
        if (Physics.Raycast(cam.transform.position, cam.transform.forward, out hit, weapon.range, mask ))
        {
            if (hit.collider.tag == "Player")
            {
                CmdPlayerShot(hit.collider.name);
            }
        }
    }

    [Command]
    void CmdPlayerShot(string _playerName)
    {
        Debug.Log(_playerName + " has been shot.");        
    }

}