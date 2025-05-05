using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using AgaveCaseStudy.Services;
using AgaveCaseStudy.Game;

namespace AgaveCaseStudy.Game
{
    public class GameController : MonoBehaviour, IGameController
    {
        [Header("UI Elements")]
        [SerializeField] private TMP_Text movesText;
        [SerializeField] private TMP_Text scoreText;
        [SerializeField] private TMP_Text scoreLimitText;
        [SerializeField] private GameObject winPanel;
        [SerializeField] private GameObject losePanel;
        [SerializeField] private Button backToMenuButton;
        [SerializeField] private Button quitButton;
        [SerializeField] private Button restartButton;

        private IBoardService boardService;
        private IGameConfig config;
        private int movesRemaining;
        private int score;

        
        public bool GameEnded => winPanel.activeSelf || losePanel.activeSelf;

        private void Awake()
        {
            backToMenuButton.onClick.AddListener(BackToMenu);
            quitButton.onClick.AddListener(QuitGame);
            restartButton.onClick.AddListener(RestartGame);
        }

        public void StartGame()
        {
            boardService = ServiceLocator.Resolve<IBoardService>();
            config = ServiceLocator.Resolve<IGameConfig>();
            movesRemaining = config.MoveCount;
            score = 0;

            movesText.text = $"Moves: {movesRemaining}";
            scoreText.text = $"Score: {score}";
            scoreLimitText.text = $"Score Limit: {config.ScoreLimit}";

            winPanel.SetActive(false);
            losePanel.SetActive(false);

            boardService.OnMatchFound += OnMatchFound;
            boardService.Initialize(config.Width, config.Height);
        }

        public void ProcessSelection(List<Tile> selectedTiles)
        {
            var matched = boardService.FindMatch(selectedTiles);
            if (matched.Count > 0)
            {
                boardService.RemoveTiles(matched);
            }
        }

        private void OnMatchFound(List<Tile> matched)
        {
            score += matched.Count;
            scoreText.text = $"Score: {score}";

            movesRemaining--;
            movesText.text = $"Moves: {movesRemaining}";

            boardService.FillEmptyTiles();

            if (movesRemaining <= 0)
            {
                if (score >= config.ScoreLimit)
                    winPanel.SetActive(true);
                else
                    losePanel.SetActive(true);
            }
        }

        private void BackToMenu()
        {
            var loader = ServiceLocator.Resolve<ISceneLoader>();
            loader.LoadScene("Menu");
        }

        private void QuitGame()
        {
            var loader = ServiceLocator.Resolve<ISceneLoader>();
            loader.LoadScene("Menu");
        }

        private void RestartGame()
        {
            var loader = ServiceLocator.Resolve<ISceneLoader>();
            loader.LoadScene("Game");
        }
    }
} 