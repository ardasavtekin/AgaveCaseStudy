using System;

namespace AgaveCaseStudy.Services
{
    public interface ISceneLoader
    {
        event Action<float> OnProgressChanged;
        void LoadScene(string sceneName);
    }
} 