using UnityEngine;

public class SphereTransparency : MonoBehaviour 
{
    private Material material;
    [Range(0f, 1f)]
    public float transparency = 0.3f; // Valeur par défaut plus basse pour plus de transparence
    
    void Start()
    {
        // Récupère le component Renderer et crée une instance unique du material
        Renderer renderer = GetComponent<Renderer>();
        material = new Material(renderer.material);
        renderer.material = material;
        
        // Configure le material pour la transparence
        material.SetFloat("_Mode", 3);
        material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
        material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
        material.SetInt("_ZWrite", 0);
        material.DisableKeyword("_ALPHATEST_ON");
        material.EnableKeyword("_ALPHABLEND_ON");
        material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
        material.renderQueue = 3000;
        
        // Définit la transparence initiale
        UpdateTransparency();
    }
    
    void Update()
    {
        UpdateTransparency();
    }
    
    private void UpdateTransparency()
    {
        // Met à jour la transparence
        Color currentColor = material.color;
        currentColor.a = transparency;
        material.color = currentColor;
        
        // S'assure que le material est bien en mode transparent
        material.SetFloat("_Mode", 3);
    }
}