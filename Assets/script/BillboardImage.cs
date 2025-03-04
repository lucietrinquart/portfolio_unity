using UnityEngine;

public class BillboardImage : MonoBehaviour
{
    // Différents modes de billboard
    public enum BillboardMode
    {
        AlwaysFacingCamera,
        RotateAroundY
    }

    public BillboardMode mode = BillboardMode.AlwaysFacingCamera;
    public float minimumVisibleSize = 0.1f; // Taille minimale visible

    void Update()
    {
        // Assurez-vous d'avoir une référence à la caméra principale
        Camera mainCamera = Camera.main;

        if (mainCamera != null)
        {
            switch (mode)
            {
                case BillboardMode.AlwaysFacingCamera:
                    // Mode 1 : Toujours face à la caméra
                    transform.LookAt(mainCamera.transform);
                    transform.Rotate(0, 180, 0); // Rotation pour corriger l'orientation
                    break;

                case BillboardMode.RotateAroundY:
                    // Mode 2 : Rotation uniquement autour de l'axe Y
                    Vector3 cameraPosition = mainCamera.transform.position;
                    Vector3 direction = cameraPosition - transform.position;
                    direction.y = 0; // Ignore la différence de hauteur
                    transform.rotation = Quaternion.LookRotation(direction);
                    break;
            }

            // Gestion de la visibilité basée sur la distance
            float distanceToCamera = Vector3.Distance(transform.position, mainCamera.transform.position);
            float imageScale = Mathf.Clamp(1f / distanceToCamera, minimumVisibleSize, 1f);
            transform.localScale = Vector3.one * imageScale;
        }
    }
}