using UnityEngine;
using Mirror;

public class PlayerSetup : NetworkBehaviour
{
   [SerializeField] private Behaviour[] componentsToDisable;

   private void Start()
   {
       if (!isLocalPlayer)
       {
           //On boucler les différents components pour les désactiver si ce n'est pas le joueur local
           for (int i = 0; i < componentsToDisable.Length; i++)
           {
               componentsToDisable[i].enabled = false;
           }
       }
   }
}
