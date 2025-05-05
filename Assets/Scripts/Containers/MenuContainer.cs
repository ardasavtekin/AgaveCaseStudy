using UnityEngine;
using UnityEngine.UI;
using TMPro;
using AgaveCaseStudy.Services;

namespace AgaveCaseStudy.Containers
{
    /// <summary>
    /// Menu sahnesinde kullanıcı girdilerini alır, config servisini kaydeder ve oyun sahnesini yükler.
    /// </summary>
    public class MenuContainer : MonoBehaviour
    {
        [Header("Game Configuration")]
        [SerializeField] private TMP_InputField widthInput;
        [SerializeField] private TMP_InputField heightInput;
        [SerializeField] private TMP_InputField moveCountInput;
        [SerializeField] private TMP_InputField scoreLimitInput;
        [SerializeField] private Button startButton;

        private void Awake()
        {
            if (startButton != null)
                startButton.onClick.AddListener(OnStartClicked);
        }

        private void OnStartClicked()
        {
            // Değerleri parse et, hata alırsak default kullan
            int width = ParseOrDefault(widthInput.text, 8);
            int height = ParseOrDefault(heightInput.text, 8);
            int moves = ParseOrDefault(moveCountInput.text, 20);
            int scoreLimit = ParseOrDefault(scoreLimitInput.text, 30);

            // Config servisini kaydet
            var config = new RuntimeGameConfig(width, height, moves, scoreLimit);
            ServiceLocator.Register<IGameConfig>(config);

            // Game sahnesini yükle
            var loader = ServiceLocator.Resolve<ISceneLoader>();
            loader.LoadScene("Game");
        }

        private int ParseOrDefault(string input, int defaultValue)
        {
            if (int.TryParse(input, out int result))
                return result;
            return defaultValue;
        }
    }
} 