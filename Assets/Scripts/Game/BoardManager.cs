using UnityEngine;
using System.Collections.Generic;
using AgaveCaseStudy.Services;
using UnityEngine.UI;

namespace AgaveCaseStudy.Game
{
    public class BoardManager : MonoBehaviour, IBoardService
    {
        public event System.Action<List<Tile>> OnMatchFound = delegate { };

        [SerializeField] private GameObject tilePrefab;
        [SerializeField] private List<Sprite> chipSprites;

        [SerializeField] private RectTransform boardContainer;
        [SerializeField] private GridLayoutGroup gridLayout;
        [SerializeField] [Tooltip("Board Horizontal Margin")] private float horizontalMargin = 20f;

        private int width, height;
        private Tile[,] tiles;

        public RectTransform BoardContainer => boardContainer;
        public GridLayoutGroup GridLayout => gridLayout;

        public void Initialize(int width, int height)
        {
            this.width = width;
            this.height = height;
            SetupBoardUI(width, height);
            if (tiles != null) ClearBoard();
            tiles = new Tile[width, height];
            for (int y = height - 1; y >= 0; y--)
            {
                for (int x = 0; x < width; x++)
                {
                    CreateTile(x, y);
                }
            }
            if (!HasPossibleMoves())
                Shuffle();
            ReorderBoardUI();
        }

        private void CreateTile(int x, int y)
        {
            var obj = Instantiate(tilePrefab);
            obj.transform.SetParent(boardContainer, false);
            obj.transform.localScale = Vector3.one;
            var tile = obj.GetComponent<Tile>();
            var color = (ChipColor)Random.Range(0, chipSprites.Count);
            tile.Initialize(x, y, color, chipSprites[(int)color]);
            tiles[x, y] = tile;
        }

        public Tile GetTile(int x, int y)
        {
            if (x >= 0 && x < width && y >= 0 && y < height)
                return tiles[x, y];
            return null;
        }

        public List<Tile> FindMatch(List<Tile> selectedTiles)
        {
            var matched = new List<Tile>();
            if (selectedTiles == null || selectedTiles.Count < 3)
                return matched;

            var color = selectedTiles[0].Color;
            foreach (var t in selectedTiles)
                if (t.Color == color && !matched.Contains(t))
                    matched.Add(t);

            return matched.Count >= 3 ? matched : new List<Tile>();
        }

        public void RemoveTiles(List<Tile> toRemove)
        {
            foreach (var t in toRemove)
            {
                tiles[t.X, t.Y] = null;
                Destroy(t.gameObject);
            }
            OnMatchFound(toRemove);
        }

        public void FillEmptyTiles()
        {
            for (int x = 0; x < width; x++)
            {
                List<Tile> columnTiles = new List<Tile>();
                for (int y = 0; y < height; y++)
                {
                    if (tiles[x, y] != null)
                    {
                        columnTiles.Add(tiles[x, y]);
                        tiles[x, y] = null;
                    }
                }
                for (int i = 0; i < columnTiles.Count; i++)
                {
                    var t = columnTiles[i];
                    int newY = i;
                    t.Initialize(x, newY, t.Color, t.CurrentSprite);
                    tiles[x, newY] = t;
                }
                for (int y = columnTiles.Count; y < height; y++)
                {
                    CreateTile(x, y);
                }
            }
            ReorderBoardUI();
        }

        public bool HasPossibleMoves()
        {
            for (int x = 0; x < width; x++)
                for (int y = 0; y < height; y++)
                    if (CheckSwap(x, y, x + 1, y) || CheckSwap(x, y, x, y + 1))
                        return true;
            return false;
        }

        private bool CheckSwap(int x1, int y1, int x2, int y2)
        {
            if (x2 < 0 || x2 >= width || y2 < 0 || y2 >= height) return false;
            Swap(x1, y1, x2, y2);
            bool hasMatch = HasMatchAt(x1, y1) || HasMatchAt(x2, y2);
            Swap(x1, y1, x2, y2);
            return hasMatch;
        }

        private bool HasMatchAt(int x, int y)
        {
            var t = tiles[x, y];
            if (t == null) return false;
            var color = t.Color;
            int count = 1;
            for (int i = x - 1; i >= 0 && tiles[i, y]?.Color == color; i--) count++;
            for (int i = x + 1; i < width && tiles[i, y]?.Color == color; i++) count++;
            if (count >= 3) return true;
            count = 1;
            for (int j = y - 1; j >= 0 && tiles[x, j]?.Color == color; j--) count++;
            for (int j = y + 1; j < height && tiles[x, j]?.Color == color; j++) count++;
            return count >= 3;
        }

        private void Swap(int x1, int y1, int x2, int y2)
        {
            var t1 = tiles[x1, y1];
            var t2 = tiles[x2, y2];
            tiles[x1, y1] = t2;
            tiles[x2, y2] = t1;
            if (t1 != null) t1.Initialize(x2, y2, t1.Color, t1.CurrentSprite);
            if (t2 != null) t2.Initialize(x1, y1, t2.Color, t2.CurrentSprite);
        }

        public void Shuffle()
        {
            var list = new List<Tile>();
            foreach (var t in tiles) if (t != null) list.Add(t);
            for (int i = 0; i < list.Count; i++)
            {
                int r = Random.Range(0, list.Count);
                var t1 = list[i];
                var t2 = list[r];
                Swap(t1.X, t1.Y, t2.X, t2.Y);
            }
        }

        private void ClearBoard()
        {
            foreach (Transform child in boardContainer)
                Destroy(child.gameObject);
        }

        private void SetupBoardUI(int width, int height)
        {
            Canvas.ForceUpdateCanvases();
            gridLayout.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
            gridLayout.constraintCount = width;
            RectTransform parentRT = boardContainer.parent.GetComponent<RectTransform>();
            float parentWidth = parentRT.rect.width - horizontalMargin * 2;
            float parentHeight = parentRT.rect.height;
            float spacingX = gridLayout.spacing.x;
            float spacingY = gridLayout.spacing.y;
            float cellWidth = (parentWidth - spacingX * (width - 1)) / width;
            float cellHeight = (parentHeight - spacingY * (height - 1)) / height;
            float cellSize = Mathf.Min(cellWidth, cellHeight);
            gridLayout.cellSize = new Vector2(cellSize, cellSize);
            float boardWidth = cellSize * width + spacingX * (width - 1);
            float boardHeight = cellSize * height + spacingY * (height - 1);
            boardContainer.sizeDelta = new Vector2(boardWidth, boardHeight);
        }

        private void ReorderBoardUI()
        {
            List<Transform> children = new List<Transform>();
            for (int y = height - 1; y >= 0; y--)
            {
                for (int x = 0; x < width; x++)
                {
                    var t = tiles[x, y];
                    if (t != null)
                        children.Add(t.transform);
                }
            }
            for (int i = 0; i < children.Count; i++)
            {
                children[i].SetSiblingIndex(i);
            }
            Canvas.ForceUpdateCanvases();
            LayoutRebuilder.ForceRebuildLayoutImmediate(boardContainer);
        }
    }
} 