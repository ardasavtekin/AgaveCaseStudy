using System.Collections.Generic;
using AgaveCaseStudy.Game;

namespace AgaveCaseStudy.Services
{
    public interface IBoardService
    {
        event System.Action<List<Tile>> OnMatchFound;
        void Initialize(int width, int height);
        Tile GetTile(int x, int y);
        List<Tile> FindMatch(List<Tile> selectedTiles);
        void RemoveTiles(List<Tile> tiles);
        void FillEmptyTiles();
        bool HasPossibleMoves();
        void Shuffle();
    }
} 