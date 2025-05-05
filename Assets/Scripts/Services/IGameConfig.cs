using System;

namespace AgaveCaseStudy.Services
{
    public interface IGameConfig
    {
        int Width { get; }
        int Height { get; }
        int MoveCount { get; }
        int ScoreLimit { get; }
    }
} 