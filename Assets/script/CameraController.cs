using UnityEngine;

public class CameraController : MonoBehaviour 
{
    public Transform target;           // Le joueur
    public Transform planet;           // La planète
    public float distance = 10f;       // Distance de la caméra par rapport au joueur
    public float smoothSpeed = 5f;     // Vitesse de suivi de la caméra
    
    [Header("Vue Distributeur")]
    public Vector3 distributeurOffset = new Vector3(3f, 6f, -2f); // Position relative au distributeur
    public Vector3 distributeurRotation = new Vector3(15f, -45f, 0f); // Rotation de la caméra
    
    private bool isLookingAtDistributeur = false;
    private Transform currentDistributeur;
    private Quaternion initialRotation;

    private void LateUpdate()
    {
        if (isLookingAtDistributeur && currentDistributeur != null)
        {
            // Position fixe devant le distributeur avec décalage
            Vector3 targetPosition = currentDistributeur.position + distributeurOffset;
            transform.position = Vector3.Lerp(transform.position, targetPosition, smoothSpeed * Time.deltaTime);

            // Calcul de la rotation souhaitée
            Quaternion targetRotation = Quaternion.Euler(distributeurRotation) * Quaternion.LookRotation(currentDistributeur.position - transform.position);
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, smoothSpeed * Time.deltaTime);
        }
        else if (target != null && planet != null)
        {
            // Comportement normal de suivi du joueur
            Vector3 upDirection = (target.position - planet.position).normalized;
            Vector3 targetPosition = target.position + upDirection * distance;
            transform.position = Vector3.Lerp(transform.position, targetPosition, smoothSpeed * Time.deltaTime);
            transform.LookAt(target.position, upDirection);
        }
    }

    public void SetDistributeurView(Transform distributeur, bool activate)
    {
        isLookingAtDistributeur = activate;
        currentDistributeur = distributeur;
        
        if (activate)
        {
            // Sauvegarde de la rotation initiale
            initialRotation = transform.rotation;
        }
        else
        {
            // Retour à la rotation initiale
            transform.rotation = initialRotation;
        }
    }
}