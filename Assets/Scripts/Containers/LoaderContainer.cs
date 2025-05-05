using UnityEngine;
using AgaveCaseStudy.Services;

namespace AgaveCaseStudy.Containers
{
    /// <summary>
    /// Loader sahnesindeki servisleri kaydeden ve bir sonraki sahneye geçişi tetikleyen container.
    /// </summary>
    public class LoaderContainer : MonoBehaviour
    {
        [SerializeField] private SceneLoader sceneLoader;

        private void Awake()
        {
            // Önceki kayıtları temizleyelim
            ServiceLocator.Clear();

            // Gereken servisleri kaydedelim
            ServiceLocator.Register<ISceneLoader>(sceneLoader);

            // İleride ekleyeceğimiz diğer global servisler burada kaydedilebilir
        }

        private void Start()
        {
            // Menu sahnesine geçiş yap
            var loader = ServiceLocator.Resolve<ISceneLoader>();
            loader.LoadScene("Menu");
        }
    }
} 