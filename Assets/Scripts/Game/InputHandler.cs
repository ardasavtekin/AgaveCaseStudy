using System.Collections.Generic;
using UnityEngine;
using AgaveCaseStudy.Game;
using AgaveCaseStudy.Services;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace AgaveCaseStudy.Game
{
    public class InputHandler : MonoBehaviour
    {
        private IBoardService boardService;
        private IGameController gameController;
        private readonly List<Tile> selectedTiles = new List<Tile>();
        private Canvas uiCanvas;
        private RectTransform canvasRect;
        private List<RawImage> lineSegments = new List<RawImage>();
        [SerializeField, Tooltip("Çizgi kalınlığı (px)")] private float lineThickness = 15f;
        [Header("UI Raycast Ayarları")]
        [SerializeField] private GraphicRaycaster graphicRaycaster;
        [SerializeField] private EventSystem eventSystem;
        [SerializeField, Tooltip("Seçim için ekstra hitbox alanı (px)")] private float hitboxMargin = 3f;

        private void Awake()
        {
            boardService = ServiceLocator.Resolve<IBoardService>();
            gameController = ServiceLocator.Resolve<IGameController>();
            if (graphicRaycaster == null)
                graphicRaycaster = FindObjectOfType<GraphicRaycaster>();
            if (eventSystem == null)
                eventSystem = EventSystem.current != null ? EventSystem.current : FindObjectOfType<EventSystem>();

            // UI Canvas referanslarını al
            uiCanvas = graphicRaycaster.GetComponentInParent<Canvas>();
            canvasRect = uiCanvas.GetComponent<RectTransform>();
            // UI için çizgi segmentleri listesi başlat
            lineSegments = new List<RawImage>();
        }

        private void Update()
        {
            if (gameController is GameController controller && controller.GameEnded)
            {
                ClearSelection();
                return;
            }
            if (Input.GetMouseButtonDown(0))
                ClearSelection();

            if (Input.GetMouseButton(0))
                TrySelectTile();

            if (Input.GetMouseButtonUp(0))
            {
                gameController.GetType().GetMethod("ProcessSelection").Invoke(gameController, new object[] { selectedTiles });
                ClearSelection();
            }

            UpdateLineSegments();
        }

        private void TrySelectTile()
        {
            // Hitbox'u genişletmek için öncelikli offsetlerle tekli raycast yapıyoruz
            var offsets = new List<Vector2>
            {
                Vector2.zero,
                new Vector2(hitboxMargin, 0),
                new Vector2(-hitboxMargin, 0),
                new Vector2(0, hitboxMargin),
                new Vector2(0, -hitboxMargin),
                // Diagonal offsetler
                new Vector2(hitboxMargin, hitboxMargin),
                new Vector2(-hitboxMargin, hitboxMargin),
                new Vector2(hitboxMargin, -hitboxMargin),
                new Vector2(-hitboxMargin, -hitboxMargin)
            };
            Tile hitTile = null;
            foreach (var offset in offsets)
            {
                var pointerPos = new Vector2(Input.mousePosition.x, Input.mousePosition.y) + offset;
                var pointerData = new PointerEventData(eventSystem) { position = pointerPos };
                var results = new List<RaycastResult>();
                graphicRaycaster.Raycast(pointerData, results);
                foreach (var res in results)
                {
                    // Sadece chip child nesnesine tıklamaya izin ver (isim olarak "Chip")
                    if (res.gameObject.name != "Chip")
                        continue;
                    var tileComp = res.gameObject.GetComponentInParent<Tile>();
                    if (tileComp != null)
                    {
                        hitTile = tileComp;
                        break;
                    }
                }
                if (hitTile != null)
                    break;
            }
            if (hitTile == null) return;

            // Geri gitme: eğer zaten seçilen bir Tile'a dönülüyorsa seçim yolunu kısalt
            if (selectedTiles.Contains(hitTile))
            {
                int idx = selectedTiles.IndexOf(hitTile);
                int lastIndex = selectedTiles.Count - 1;
                // Aynı son seçilense atla
                if (idx == lastIndex)
                    return;
                // Bir önceki seçilmiş ise sadece sonuncuyu çıkar
                if (idx == lastIndex - 1)
                {
                    var last = selectedTiles[lastIndex];
                    last.Highlight(false);
                    selectedTiles.RemoveAt(lastIndex);
                    return;
                }
                // Daha geriye gidiliyorsa, idx sonrası tüm seçimleri çıkar
                for (int i = lastIndex; i > idx; i--)
                {
                    selectedTiles[i].Highlight(false);
                    selectedTiles.RemoveAt(i);
                }
                return;
            }

            // Renk kontrolü: tüm seçimler aynı renk olmalı
            if (selectedTiles.Count > 0 && hitTile.Color != selectedTiles[0].Color)
                return;

            // Komşuluk kontrolü: yeni Tile son seçilen ile Chebyshev mesafesi 1 içinde olmalı
            if (selectedTiles.Count > 0)
            {
                var last = selectedTiles[selectedTiles.Count - 1];
                int dx = Mathf.Abs(hitTile.X - last.X);
                int dy = Mathf.Abs(hitTile.Y - last.Y);
                // Aynı değilse ve komşu değilse atla
                if ((dx == 0 && dy == 0) || dx > 1 || dy > 1)
                    return;
            }

            // Yeni Tile'ı seç ve vurgula
            selectedTiles.Add(hitTile);
            hitTile.Highlight(true);
        }

        private void ClearSelection()
        {
            foreach (var t in selectedTiles)
                t.Highlight(false);
            selectedTiles.Clear();
            // Çizgi segmentlerini gizle
            foreach (var seg in lineSegments)
                seg.gameObject.SetActive(false);
        }

        /// <summary>
        /// Seçili tile'lar ve fare pozisyonu arasında UI tabanlı çizgi çizer.
        /// </summary>
        private void UpdateLineSegments()
        {
            int count = selectedTiles.Count;
            // Gerektiğinde segment oluştur veya gizle
            for (int i = 0; i < lineSegments.Count; i++)
                lineSegments[i].gameObject.SetActive(i < count);
            for (int i = lineSegments.Count; i < count; i++)
            {
                var go = new GameObject($"LineSegment_{i}", typeof(RectTransform), typeof(RawImage));
                // canvasRect Transform referansını kullan
                go.transform.SetParent(canvasRect.transform, false);
                var img = go.GetComponent<RawImage>();
                img.texture = Texture2D.whiteTexture;
                lineSegments.Add(img);
            }
            if (count == 0) return;
            // Renk
            Color col = GetColorFromChipColor(selectedTiles[0].Color);
            for (int i = 0; i < count; i++)
            {
                var img = lineSegments[i];
                // P0 dan P1'e
                Vector3 p0 = GetLocalPosition(selectedTiles[i]);
                Vector3 p1 = (i < count - 1) ? GetLocalPosition(selectedTiles[i + 1]) : GetMouseLocalPosition();
                Vector2 dir = new Vector2(p1.x - p0.x, p1.y - p0.y);
                float len = dir.magnitude;
                float ang = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
                var rt = img.rectTransform;
                // Segment orta noktası
                Vector2 mid = new Vector2(p0.x + dir.x * 0.5f, p0.y + dir.y * 0.5f);
                rt.anchoredPosition = mid;
                rt.sizeDelta = new Vector2(len, lineThickness);
                rt.localEulerAngles = new Vector3(0f, 0f, ang);
                img.color = col;
                img.gameObject.SetActive(true);
            }
        }

        /// <summary>
        /// Fare ekran pozisyonunu UI local koordinatına çevirir.
        /// </summary>
        private Vector3 GetMouseLocalPosition()
        {
            // UI local pozisyonu elde et
            Camera cam = uiCanvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : uiCanvas.worldCamera;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRect, Input.mousePosition, cam, out Vector2 lp);
            return new Vector3(lp.x, lp.y, 0f);
        }

        /// <summary>
        /// Tile'ın UI local pozisyonunu alır.
        /// </summary>
        private Vector3 GetLocalPosition(Tile tile)
        {
            // Tile'ın UI local pozisyonunu döndür
            Camera cam = uiCanvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : uiCanvas.worldCamera;
            Vector2 screenPoint = RectTransformUtility.WorldToScreenPoint(cam, tile.GetComponent<RectTransform>().position);
            RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRect, screenPoint, cam, out Vector2 lp);
            return new Vector3(lp.x, lp.y, 0f);
        }

        private Color GetColorFromChipColor(ChipColor chipColor)
        {
            switch (chipColor)
            {
                case ChipColor.Yellow: return Color.blue;
                case ChipColor.Blue:   return Color.green;
                case ChipColor.Green:  return Color.red;
                case ChipColor.Red:    return Color.yellow;
                default:               return Color.white;
            }
        }
    }
} 