using UnityEngine;
using TMPro;

public class MachineInteraction : MonoBehaviour
{
    [SerializeField] private float interactionRadius = 2f;
    [SerializeField] private GameObject interactionText;
    private Transform playerTransform;
    private bool isPlayerInRange = false;
    private CameraController cameraController;

    void Start()
    {
        interactionText.SetActive(false);
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        cameraController = Camera.main.GetComponent<CameraController>();
    }

    void Update()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, playerTransform.position);

        if (distanceToPlayer <= interactionRadius)
        {
            if (!isPlayerInRange)
            {
                isPlayerInRange = true;
                interactionText.SetActive(true);
            }

            if (Input.GetKeyDown(KeyCode.Y))
            {
                InteractWithMachine();
            }
            
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                ExitMachineView();
            }
        }
        else if (isPlayerInRange)
        {
            isPlayerInRange = false;
            interactionText.SetActive(false);
            ExitMachineView();
        }
    }

    void InteractWithMachine()
    {
        if (cameraController != null)
        {
            cameraController.SetDistributeurView(transform, true);
        }
    }

    void ExitMachineView()
    {
        if (cameraController != null)
        {
            cameraController.SetDistributeurView(null, false);
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, interactionRadius);
    }
}