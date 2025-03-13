using UnityEngine;
using TMPro;
using UnityEngine.Video;
using UnityEngine.UI;

public class CubeButton2 : MonoBehaviour 
{
    [Header("Panels")]
    public GameObject mainPanel;
    public GameObject detailPanel;
    public GameObject videoPanel;
    
    [Header("Video Settings")]
    public VideoPlayer videoPlayer;
    public RawImage videoRawImage;
    public RenderTexture videoRenderTexture;
    
    [Header("UI Elements")]
    public TMP_Text detailText;
    
    private bool videoPlaying = false;
    
    void Start()
    {
        // Désactiver le videoPanel et detailPanel au démarrage
        if (videoPanel != null)
            videoPanel.SetActive(false);
        
        if (detailPanel != null)
            detailPanel.SetActive(false);
        
        // Vérifier et configurer les éléments vidéo
        if (videoPlayer != null)
        {
            // S'assurer que la vidéo utilise le bon RenderTexture
            videoPlayer.targetTexture = videoRenderTexture;
            
            // S'abonner à l'événement de fin de vidéo
            videoPlayer.loopPointReached += OnVideoFinished;
            
            // Important: Désactiver autoPlay pour éviter que la vidéo se lance au démarrage
            videoPlayer.playOnAwake = false;
            
            // Couper le son de la vidéo au démarrage
            videoPlayer.SetDirectAudioMute(0, true);
            
            // Ne pas préparer la vidéo tout de suite pour éviter le chargement de la première frame
            // videoPlayer.Prepare();  // Cette ligne est commentée
        }
        else
        {
            Debug.LogError("VideoPlayer n'est pas assigné dans l'inspecteur!");
        }
        
        // Si on utilise un RenderTexture existant, le nettoyer au démarrage
        if (videoRenderTexture != null)
        {
            RenderTexture.active = videoRenderTexture;
            GL.Clear(true, true, Color.black); // Nettoie avec du noir transparent
            RenderTexture.active = null;
        }
        
        // Vérifier que le RawImage a bien le RenderTexture mais le cacher initialement
        if (videoRawImage != null)
        {
            if (videoRenderTexture != null)
            {
                videoRawImage.texture = videoRenderTexture;
            }
            
            // S'assurer que le RawImage est invisible au départ
            Color c = videoRawImage.color;
            c.a = 0; // Alpha à 0 pour le rendre invisible
            videoRawImage.color = c;
        }
    }
    
    void OnMouseDown()
    {
        Debug.Log("Bouton cliqué - Lancement de la séquence vidéo");
        
        // Vérifier que tout est bien configuré
        if (videoPlayer == null)
        {
            Debug.LogError("VideoPlayer n'est pas assigné!");
            ShowDetailPanel();
            return;
        }
        
        // Cacher le panneau principal
        if (mainPanel != null)
            mainPanel.SetActive(false);
        
        // Activer le panneau vidéo et lancer la vidéo
        if (videoPanel != null)
        {
            videoPanel.SetActive(true);
            
            // Rendre le RawImage visible à nouveau
            if (videoRawImage != null)
            {
                Color c = videoRawImage.color;
                c.a = 1; // Alpha à 1 pour le rendre visible
                videoRawImage.color = c;
                
                RectTransform rectTransform = videoRawImage.GetComponent<RectTransform>();
                if (rectTransform != null)
                {
                    // Configurer le RawImage pour qu'il occupe tout l'espace disponible
                    rectTransform.anchorMin = Vector2.zero;
                    rectTransform.anchorMax = Vector2.one;
                    rectTransform.sizeDelta = Vector2.zero;
                    rectTransform.anchoredPosition = Vector2.zero;
                }
            }
            
            // Réactiver le son avant de jouer la vidéo
            videoPlayer.SetDirectAudioMute(0, false);
            
            // Maintenant on prépare la vidéo seulement quand on en a besoin
            if (videoPlayer.isPrepared)
            {
                PlayVideo();
            }
            else
            {
                videoPlayer.prepareCompleted += PrepareCompleted;
                videoPlayer.Prepare();
            }
        }
        else
        {
            Debug.LogError("videoPanel n'est pas assigné!");
            ShowDetailPanel();
        }
    }
    
    private void PrepareCompleted(VideoPlayer vp)
    {
        // La vidéo est prête, on peut la jouer
        videoPlayer.prepareCompleted -= PrepareCompleted;
        PlayVideo();
    }
    
    private void PlayVideo()
    {
        videoPlayer.Play();
        videoPlaying = true;
        Debug.Log("Vidéo lancée");
    }
    
    void OnVideoFinished(VideoPlayer vp)
    {
        Debug.Log("Vidéo terminée");
        
        // Cette méthode est appelée quand la vidéo se termine
        // Utiliser le thread principal pour manipuler l'UI
        UnityMainThreadDispatcher.Instance().Enqueue(() => {
            if (videoPlaying)
            {
                // Désactiver le panneau vidéo
                if (videoPanel != null)
                    videoPanel.SetActive(false);
                
                videoPlaying = false;
                
                // Couper à nouveau le son
                videoPlayer.SetDirectAudioMute(0, true);
                
                // Rendre le RawImage invisible à nouveau
                if (videoRawImage != null)
                {
                    Color c = videoRawImage.color;
                    c.a = 0; // Alpha à 0 pour le rendre invisible
                    videoRawImage.color = c;
                }
                
                // Afficher le panneau de détail
                ShowDetailPanel();
            }
        });
    }
    
    void ShowDetailPanel()
    {
        // Activer le panneau de détail
        if (detailPanel != null)
            detailPanel.SetActive(true);
        else
            Debug.LogError("detailPanel n'est pas assigné!");
    }
    
    void OnDestroy()
    {
        // Nettoyer les événements
        if (videoPlayer != null)
        {
            videoPlayer.loopPointReached -= OnVideoFinished;
            videoPlayer.prepareCompleted -= PrepareCompleted;
        }
    }
}