using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform target;           // Le joueur
    public Transform planet;           // La planète
    public float distance = 10f;       // Distance de la caméra par rapport au joueur
    public float smoothSpeed = 5f;     // Vitesse de suivi de la caméra
    
    [System.Serializable]
    public class MachineViewSettings
    {
        public string machineName;               // Nom pour identifier la machine
        public Vector3 cameraOffset;             // Position relative à la machine
        public Vector3 cameraRotation;           // Rotation de la caméra
    }
    
    [Header("Paramètres des machines")]
    public MachineViewSettings[] machineSettings;    // Tableau de configurations pour différentes machines
    
    private bool isLookingAtMachine = false;
    private Transform currentMachine;
    private Quaternion initialRotation;
    private MachineViewSettings currentSettings;
    
    private void LateUpdate()
    {
        if (isLookingAtMachine && currentMachine != null && currentSettings != null)
        {
            // Position fixe devant la machine avec décalage spécifique
            Vector3 targetPosition = currentMachine.position + currentSettings.cameraOffset;
            transform.position = Vector3.Lerp(transform.position, targetPosition, smoothSpeed * Time.deltaTime);
            
            // Calcul de la rotation souhaitée spécifique à la machine
            Quaternion targetRotation = Quaternion.Euler(currentSettings.cameraRotation) * Quaternion.LookRotation(currentMachine.position - transform.position);
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
    
    // Assurez-vous que cette méthode est publique
    public void SetMachineView(Transform machine, string machineName, bool activate)
    {
        isLookingAtMachine = activate;
        currentMachine = machine;
        
        if (activate && machine != null)
        {
            // Sauvegarde de la rotation initiale
            initialRotation = transform.rotation;
            
            // Trouver les paramètres correspondant à cette machine
            currentSettings = null;
            foreach (MachineViewSettings settings in machineSettings)
            {
                if (settings.machineName == machineName)
                {
                    currentSettings = settings;
                    break;
                }
            }
            
            // Si aucun paramètre trouvé, utiliser le premier (ou logs d'erreur)
            if (currentSettings == null && machineSettings.Length > 0)
            {
                Debug.LogWarning("Aucun paramètre trouvé pour la machine: " + machineName + ". Utilisation des paramètres par défaut.");
                currentSettings = machineSettings[0];
            }
        }
        else
        {
            // Retour à la rotation initiale
            transform.rotation = initialRotation;
            currentSettings = null;
        }
    }
    
    // Maintenir également l'ancienne méthode pour la compatibilité (juste au cas où)
    public void SetDistributeurView(Transform distributeur, bool activate)
    {
        // Rediriger vers la nouvelle méthode
        SetMachineView(distributeur, "Distributeur", activate);
    }
}