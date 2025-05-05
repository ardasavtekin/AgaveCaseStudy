using AgaveCaseStudy.Services;

namespace AgaveCaseStudy.Services
{
    public class RuntimeGameConfig : IGameConfig
    {
        public int Width { get; }
        public int Height { get; }
        public int MoveCount { get; }
        public int ScoreLimit { get; }

        public RuntimeGameConfig(int width, int height, int moveCount, int scoreLimit)
        {
            Width = width;
            Height = height;
            MoveCount = moveCount;
            ScoreLimit = scoreLimit;
        }
    }
} 