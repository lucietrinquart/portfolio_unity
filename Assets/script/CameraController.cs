using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

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
        public Vector3 cameraPosition;           // Position absolue de la caméra (au lieu d'un offset)
        public Vector3 cameraRotation;           // Rotation de la caméra
        public bool useAbsolutePosition = false; // Utiliser position absolue ou relative
    }
    
    [Header("Paramètres des machines")]
    public MachineViewSettings[] machineSettings;    // Tableau de configurations pour différentes machines
    
    private bool isLookingAtMachine = false;
    private Transform currentMachine;
    private Quaternion initialRotation;
    private Vector3 initialPosition;
    private MachineViewSettings currentSettings;
    
    private void Start()
    {
        // Sauvegarde de la position et rotation initiale de la caméra
        initialPosition = transform.position;
        initialRotation = transform.rotation;
    }
    
    private void LateUpdate()
    {
        if (isLookingAtMachine && currentSettings != null)
        {
            Vector3 targetPosition;
            
            // Utiliser soit la position absolue soit la position relative à la machine
            if (currentSettings.useAbsolutePosition || currentMachine == null)
            {
                targetPosition = currentSettings.cameraPosition;
            }
            else
            {
                targetPosition = currentMachine.position + currentSettings.cameraPosition;
            }
            
            transform.position = Vector3.Lerp(transform.position, targetPosition, smoothSpeed * Time.deltaTime);
            
            // Calcul de la rotation souhaitée spécifique à la machine
            Quaternion targetRotation = Quaternion.Euler(currentSettings.cameraRotation);
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
    
    // Méthode pour définir la vue sur une machine
    public void SetMachineView(Transform machine, string machineName, bool activate)
    {
        isLookingAtMachine = activate;
        currentMachine = machine;
        
        if (activate)
        {
            // Sauvegarde de la rotation et position initiale si ce n'est pas déjà fait
            if (initialRotation == Quaternion.identity)
            {
                initialRotation = transform.rotation;
                initialPosition = transform.position;
            }
            
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
            // Retour à la position et rotation initiales
            transform.position = initialPosition;
            transform.rotation = initialRotation;
            currentSettings = null;
        }
    }
    
    // Méthode pour faciliter les tests dans l'éditeur
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

    // Maintenir également l'ancienne méthode pour la compatibilité
    public void SetDistributeurView(Transform distributeur, bool activate)
    {
        // Rediriger vers la nouvelle méthode
        SetMachineView(distributeur, "Distributeur", activate);
    }
    
    // Ajouter cette méthode pour capturer la position et rotation actuelles
    public void CaptureCurrentTransform(string machineName)
    {
        for (int i = 0; i < machineSettings.Length; i++)
        {
            if (machineSettings[i].machineName == machineName)
            {
                machineSettings[i].cameraPosition = transform.position;
                machineSettings[i].cameraRotation = transform.eulerAngles;
                machineSettings[i].useAbsolutePosition = true;
                break;
            }
        }
    }
}