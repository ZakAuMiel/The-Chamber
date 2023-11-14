using UnityEngine;
using Mirror;

public class PlayerSetup : NetworkBehaviour
{
   #pragma warning disable 04114
   [SerializeField] private Behaviour[] componentsToDisable;
   #pragma warning disable 0414
   [SerializeField] private string remoteLayerName = "RemotePlayer";
   #pragma warning restore 0414

   Camera sceneCamera;

   private void Start()
   {
       if (!isLocalPlayer)
       {
            DisableComponents();
            AssignRemoteLayer();
       }
       else
       {
            sceneCamera = Camera.main;
            if (sceneCamera != null)
            {
                sceneCamera.gameObject.SetActive(false);
            }
       }

       RegisterPlayer();
   }

   private void DisableComponents()
   {
       for (int i = 0; i < componentsToDisable.Length; i++)
       {
           componentsToDisable[i].enabled = false;
       }
   }

   private void AssignRemoteLayer()
   {
       gameObject.layer = LayerMask.NameToLayer("RemotePlayer");
   }

   private void OnDisable()
   {
       if (sceneCamera != null)
       {
           sceneCamera.gameObject.SetActive(true);
       }
   }

   private void RegisterPlayer()
   {
        string playerName = "Player " + GetComponent<NetworkIdentity>().netId;
        transform.name = playerName;
   }
}
