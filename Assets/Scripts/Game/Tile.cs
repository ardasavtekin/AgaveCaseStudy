using UnityEngine;
using UnityEngine.UI;
using AgaveCaseStudy.Services;

namespace AgaveCaseStudy.Game
{
    public class Tile : MonoBehaviour
    {
        public int X { get; private set; }
        public int Y { get; private set; }
        public ChipColor Color { get; private set; }

        private Image backgroundImage;
        private Image chipImage;
        private Vector3 chipBaseScale;

        private void Awake()
        {
            backgroundImage = GetComponent<Image>();
            // Arka planı sürekli görünür yapmak için alpha değeri 1 olsun
            backgroundImage.color = new Color(backgroundImage.color.r, backgroundImage.color.g, backgroundImage.color.b, 1f);
            // Yalnızca chip tıklanabilir olsun, arka plan tıklamaları engelle
            backgroundImage.raycastTarget = false;

            // Chip Sprite için alt nesne oluştur
            GameObject chipGO = new GameObject("Chip", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
            chipGO.transform.SetParent(transform, false);
            RectTransform rt = chipGO.GetComponent<RectTransform>();
            rt.anchorMin = Vector2.zero;
            rt.anchorMax = Vector2.one;
            rt.offsetMin = Vector2.zero;
            rt.offsetMax = Vector2.zero;
            chipImage = chipGO.GetComponent<Image>();
            // Yeni: chip tıklanabilir olsun
            chipImage.raycastTarget = true;
            // Chiplerin biraz daha küçük gözükmesi için ölçeklendiriyorum
            chipGO.transform.localScale = Vector3.one * 0.75f;
            // Chip için base scale değerini kaydet
            chipBaseScale = chipGO.transform.localScale;
        }

        public void Initialize(int x, int y, ChipColor color, Sprite sprite)
        {
            X = x;
            Y = y;
            SetColorAndSprite(color, sprite);
        }

        public void SetColorAndSprite(ChipColor color, Sprite sprite)
        {
            Color = color;
            chipImage.sprite = sprite;
        }

        public Sprite CurrentSprite => chipImage.sprite;

        public void Highlight(bool flag)
        {
            // Sadece chip büyüsün veya orijinal boyuta dönsün
            chipImage.transform.localScale = flag ? chipBaseScale * 1.2f : chipBaseScale;
        }
    }
} 