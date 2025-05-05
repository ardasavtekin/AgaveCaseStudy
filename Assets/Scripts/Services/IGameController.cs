using System.Collections.Generic;
using AgaveCaseStudy.Game;

namespace AgaveCaseStudy.Services
{
    public interface IGameController
    {
        void StartGame();
        void ProcessSelection(List<Tile> selectedTiles);
    }
} 