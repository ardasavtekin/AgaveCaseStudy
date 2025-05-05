using UnityEngine;
using AgaveCaseStudy.Services;
using AgaveCaseStudy.Game;

namespace AgaveCaseStudy.Containers
{
    public class GameContainer : MonoBehaviour
    {
        [SerializeField] private BoardManager boardManager;
        [SerializeField] private InputHandler inputHandler;
        [SerializeField] private GameController gameController;

        private void Awake()
        {
            // Sahne yüklendiğinde daha önce kaydedilen IGameConfig ve ISceneLoader servislerini kullanıyoruz
            ServiceLocator.Register<IBoardService>(boardManager);
            ServiceLocator.Register<IGameController>(gameController);
        }

        private void Start()
        {
            gameController.StartGame();
        }
    }
} 