using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class CameraController : MonoBehaviour
{
    [Header("Paramètres Principaux")]
    public Transform target;                // Le joueur
    public Transform planet;                // La planète
    public float distance = 12f;            // Distance de la caméra par rapport au joueur (augmentée)
    public float height = 7f;               // Hauteur de la caméra au-dessus du joueur (augmentée)
    public float lookDownAngle = 25f;       // Angle d'inclinaison de la caméra (nouveau)
    public float smoothSpeed = 3f;          // Vitesse de suivi
    public float rotationSmoothSpeed = 2f;  // Vitesse de rotation de la caméra
    
    [Header("Paramètres Avancés")]
    public float minDistance = 7f;          // Distance minimale de la caméra
    public float maxDistance = 15f;         // Distance maximale de la caméra
    public float collisionOffset = 0.2f;    // Marge pour éviter les collisions
    public LayerMask collisionLayers;       // Couches à considérer pour les collisions
    public bool useTargetYaw = true;        // Utiliser la rotation du joueur (nouveau)
    
    [System.Serializable]
    public class MachineViewSettings
    {
        public string machineName;               // Nom pour identifier la machine
        public Vector3 cameraPosition;           // Position absolue ou relative de la caméra
        public Vector3 cameraRotation;           // Rotation de la caméra
        public bool useAbsolutePosition = false; // Utiliser position absolue ou relative
        public float transitionSpeed = 5f;       // Vitesse de transition
    }
    
    [Header("Paramètres des machines")]
    public MachineViewSettings[] machineSettings;    // Tableau de configurations pour différentes machines
    
    // Variables privées
    private bool isLookingAtMachine = false;
    private Transform currentMachine;
    private Quaternion initialRotation;
    private Vector3 initialPosition;
    private MachineViewSettings currentSettings;
    private Vector3 currentVelocity = Vector3.zero;  // Pour le SmoothDamp
    private Quaternion targetRotation;
    private float currentRotationVelocity = 0f;      // Pour le SmoothDampAngle
    private float currentCameraYaw = 0f;             // Rotation actuelle autour du joueur
    private float targetCameraYaw = 0f;              // Rotation cible autour du joueur
    private bool wasMovingBackward = false;          // Pour éviter les effets bizarres lors du recul
    
    private void Start()
    {
        // Initialisation
        initialPosition = transform.position;
        initialRotation = transform.rotation;
        
        // Si les couches de collision ne sont pas définies, utiliser tout sauf le joueur
        if (collisionLayers.value == 0)
            collisionLayers = ~(1 << LayerMask.NameToLayer("Player"));
            
        // Initialisation de la rotation de la caméra
        if (target != null)
        {
            Vector3 dirToTarget = (target.position - transform.position).normalized;
            currentCameraYaw = Mathf.Atan2(dirToTarget.x, dirToTarget.z) * Mathf.Rad2Deg;
            targetCameraYaw = currentCameraYaw;
        }
    }
    
    private void LateUpdate()
    {
        if (target == null || planet == null) return;
        
        if (isLookingAtMachine && currentSettings != null)
        {
            // Gestion de la vue machine avec transition fluide
            HandleMachineView();
        }
        else
        {
            // Suivi du joueur style Mario Galaxy
            HandlePlayerFollow();
        }
    }
    
    private void HandlePlayerFollow()
    {
        // Direction "vers le haut" par rapport à la planète
        Vector3 upDirection = (target.position - planet.position).normalized;
        
        // Détection du mouvement arrière
        bool isMovingBackward = false;
        if (Input.GetKey(KeyCode.S) && !Input.GetKey(KeyCode.W) && 
            !Input.GetKey(KeyCode.A) && !Input.GetKey(KeyCode.D))
        {
            isMovingBackward = true;
        }
        
        // Mise à jour de la rotation de la caméra
        if (useTargetYaw && !isMovingBackward)
        {
            // Obtenir l'angle de rotation du joueur
            Vector3 playerForward = Vector3.ProjectOnPlane(target.forward, upDirection);
            targetCameraYaw = Mathf.Atan2(playerForward.x, playerForward.z) * Mathf.Rad2Deg;
        }
        
        // Pour éviter un effet bizarre quand on recule puis avance
        if (wasMovingBackward && !isMovingBackward)
        {
            // Si on vient de passer de reculer à ne pas reculer, on réaligne doucement
            Vector3 playerForward = Vector3.ProjectOnPlane(target.forward, upDirection);
            targetCameraYaw = Mathf.Atan2(playerForward.x, playerForward.z) * Mathf.Rad2Deg;
        }
        wasMovingBackward = isMovingBackward;
        
        // Lisser la rotation de la caméra
        currentCameraYaw = Mathf.SmoothDampAngle(currentCameraYaw, targetCameraYaw, 
                                               ref currentRotationVelocity, 1f/rotationSmoothSpeed);
        
        // Calcul de la position idéale (style Mario Galaxy)
        Vector3 forward = new Vector3(
            Mathf.Sin(currentCameraYaw * Mathf.Deg2Rad), 
            0, 
            Mathf.Cos(currentCameraYaw * Mathf.Deg2Rad)
        );
        
        // Ajustement de la position pour voir le personnage complètement
        Vector3 desiredPosition = target.position 
                                - forward * distance 
                                + upDirection * height;
        
        // Vérifier les collisions
        RaycastHit hit;
        if (Physics.Linecast(target.position, desiredPosition, out hit, collisionLayers))
        {
            // Si collision, ajuster la distance
            float adjustedDistance = Vector3.Distance(target.position, hit.point) - collisionOffset;
            adjustedDistance = Mathf.Clamp(adjustedDistance, minDistance, distance);
            
            desiredPosition = target.position 
                            - forward * adjustedDistance 
                            + upDirection * height;
        }
        
        // Déplacement fluide avec SmoothDamp
        transform.position = Vector3.SmoothDamp(
            transform.position, 
            desiredPosition, 
            ref currentVelocity, 
            1f / smoothSpeed
        );
        
        // Créer un quaternion qui regarde vers le joueur mais avec une inclinaison vers le bas
        Vector3 lookDirection = (target.position - transform.position).normalized;
        Vector3 lookOffset = upDirection * Mathf.Tan(lookDownAngle * Mathf.Deg2Rad);
        Vector3 lookTarget = target.position - lookOffset;
        
        // Orientation vers le joueur avec inclinaison
        Quaternion lookRotation = Quaternion.LookRotation(
            (lookTarget - transform.position).normalized, 
            upDirection
        );
        
        transform.rotation = Quaternion.Slerp(
            transform.rotation, 
            lookRotation, 
            rotationSmoothSpeed * Time.deltaTime
        );
    }
    
    // Le reste du code reste le même...
    
    private void HandleMachineView()
    {
        Vector3 targetPosition;
        
        // Calculer la position cible selon les paramètres
        if (currentSettings.useAbsolutePosition || currentMachine == null)
        {
            targetPosition = currentSettings.cameraPosition;
        }
        else
        {
            targetPosition = currentMachine.position + currentSettings.cameraPosition;
        }
        
        // Transition fluide avec SmoothDamp
        transform.position = Vector3.SmoothDamp(
            transform.position, 
            targetPosition, 
            ref currentVelocity, 
            1f / currentSettings.transitionSpeed
        );
        
        // Rotation fluide
        Quaternion targetRotation = Quaternion.Euler(currentSettings.cameraRotation);
        transform.rotation = Quaternion.Slerp(
            transform.rotation, 
            targetRotation, 
            currentSettings.transitionSpeed * Time.deltaTime
        );
    }
    
    // Méthode pour définir la vue sur une machine
    public void SetMachineView(Transform machine, string machineName, bool activate)
    {
        // Réinitialiser la vélocité pour éviter les mouvements étranges
        currentVelocity = Vector3.zero;
        
        isLookingAtMachine = activate;
        currentMachine = machine;
        
        if (activate)
        {
            // Sauvegarde de la position actuelle si ce n'est pas déjà fait
            if (initialRotation == Quaternion.identity)
            {
                initialRotation = transform.rotation;
                initialPosition = transform.position;
            }
            
            // Trouver les paramètres pour cette machine
            currentSettings = null;
            foreach (MachineViewSettings settings in machineSettings)
            {
                if (settings.machineName == machineName)
                {
                    currentSettings = settings;
                    break;
                }
            }
            
            // Paramètres par défaut si aucun n'est trouvé
            if (currentSettings == null && machineSettings.Length > 0)
            {
                Debug.LogWarning("Aucun paramètre trouvé pour la machine: " + machineName + ". Utilisation des paramètres par défaut.");
                currentSettings = machineSettings[0];
            }
        }
        else
        {
            // Transition fluide vers la position normale
            currentSettings = null;
        }
    }
    
    // Méthode de test pour l'éditeur
    public void TestMachineView(string machineName)
    {
        foreach (MachineViewSettings settings in machineSettings)
        {
            if (settings.machineName == machineName)
            {
                Vector3 targetPosition = settings.useAbsolutePosition ? 
                    settings.cameraPosition : 
                    transform.position + settings.cameraPosition;
                    
                transform.position = targetPosition;
                transform.rotation = Quaternion.Euler(settings.cameraRotation);
                break;
            }
        }
    }

    // Méthode de compatibilité
    public void SetDistributeurView(Transform distributeur, bool activate)
    {
        SetMachineView(distributeur, "Distributeur", activate);
    }
    
    // Capturer la position et rotation actuelles
    public void CaptureCurrentTransform(string machineName)
    {
        for (int i = 0; i < machineSettings.Length; i++)
        {
            if (machineSettings[i].machineName == machineName)
            {
                machineSettings[i].cameraPosition = transform.position;
                machineSettings[i].cameraRotation = transform.eulerAngles;
                machineSettings[i].useAbsolutePosition = true;
                Debug.Log("Paramètres capturés pour: " + machineName);
                break;
            }
        }
    }
    
#if UNITY_EDITOR
    // Utilitaire d'édition pour visualiser la caméra
    private void OnDrawGizmosSelected()
    {
        if (target != null && planet != null)
        {
            Vector3 upDirection = (target.position - planet.position).normalized;
            
            // Direction de la caméra
            Vector3 forward = new Vector3(
                Mathf.Sin(currentCameraYaw * Mathf.Deg2Rad), 
                0, 
                Mathf.Cos(currentCameraYaw * Mathf.Deg2Rad)
            );
            forward = Quaternion.FromToRotation(Vector3.up, upDirection) * forward;
            
            // Dessiner la ligne de vue idéale
            Gizmos.color = Color.blue;
            Vector3 idealPosition = target.position - forward * distance + upDirection * height;
            Gizmos.DrawLine(target.position, idealPosition);
            Gizmos.DrawWireSphere(idealPosition, 0.5f);
            
            // Dessiner le rayon de collision
            Gizmos.color = Color.red;
            RaycastHit hit;
            if (Physics.Linecast(target.position, idealPosition, out hit, collisionLayers))
            {
                Gizmos.DrawLine(target.position, hit.point);
                Gizmos.DrawWireSphere(hit.point, 0.3f);
            }
            
            // Dessiner le point d'inclinaison de la caméra
            Vector3 lookOffset = upDirection * Mathf.Tan(lookDownAngle * Mathf.Deg2Rad);
            Vector3 lookTarget = target.position - lookOffset;
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(transform.position, lookTarget);
            Gizmos.DrawWireSphere(lookTarget, 0.3f);
        }
    }
#endif
}