using UnityEngine;
using AgaveCaseStudy.Services;

namespace AgaveCaseStudy.Containers
{
    [DefaultExecutionOrder(-100)]
    /// <summary>
    /// Oyun boyunca kalıcı olacak global servisleri kaydeder ve kendisini sahneler arası taşıtır.
    /// </summary>
    public class GlobalContainer : MonoBehaviour
    {
        [SerializeField] private SceneLoader sceneLoader;

        private void Awake()
        {
            DontDestroyOnLoad(gameObject);
            // Önceki tüm servis kayıtlarını temizleyelim
            ServiceLocator.Clear();
            // Global servisleri kaydedelim
            ServiceLocator.Register<ISceneLoader>(sceneLoader);
        }
    }
} 