using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;
using System.Collections;

namespace AgaveCaseStudy.Services
{
    public class SceneLoader : MonoBehaviour, ISceneLoader
    {
        [SerializeField] private Slider progressBar;
        public event Action<float> OnProgressChanged = delegate { };

        private void Awake()
        {
            DontDestroyOnLoad(gameObject);
            OnProgressChanged += UpdateProgressBar;
        }

        private void UpdateProgressBar(float value)
        {
            if (progressBar != null)
                progressBar.value = value;
        }

        public void LoadScene(string sceneName)
        {
            StartCoroutine(LoadRoutine(sceneName));
        }

        private IEnumerator LoadRoutine(string sceneName)
        {
            AsyncOperation op = SceneManager.LoadSceneAsync(sceneName);
            op.allowSceneActivation = false;
            while (!op.isDone)
            {
                float progress = Mathf.Clamp01(op.progress / 0.9f);
                OnProgressChanged(progress);
                if (op.progress >= 0.9f)
                {
                    OnProgressChanged(1f);
                    op.allowSceneActivation = true;
                }
                yield return null;
            }
        }
    }
} 