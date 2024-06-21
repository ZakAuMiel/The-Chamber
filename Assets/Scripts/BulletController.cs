using UnityEngine;

public class BulletController : MonoBehaviour
{
    public float speed = 100f; // Vitesse de la balle
    private Vector3 targetPosition;

    public void Initialize(Vector3 target)
    {
        targetPosition = target;
        Destroy(gameObject, 5f); // Détruire la balle après 5 secondes pour éviter les fuites de mémoire
    }

    void Update()
    {
        if (targetPosition != Vector3.zero)
        {
            float step = speed * Time.deltaTime;
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, step);

            if (Vector3.Distance(transform.position, targetPosition) < 0.001f)
            {
                // La balle a atteint la cible
                Destroy(gameObject); // Détruire la balle
                // Vous pouvez également ajouter des effets de collision ici
            }
        }
    }
}
