using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Header("Oyun Durumu")]
    public int moveCount;         // Kalan hamle sayısı
    public int score;             // Mevcut skor
    public int scoreLimit;        // Kazanmak için gerekli skor sınırı

    [Header("UI Panelleri")]
    public GameObject winPanel;   // Kazanma paneli
    public GameObject losePanel;  // Kaybetme paneli

    private bool gameEnded = false;

    void Start()
    {
        // Başlangıçta panelleri kapalı tut
        winPanel.SetActive(false);
        losePanel.SetActive(false);
    }

    // Bir hamle yapıldığında çağırılacak metod
    public void DecreaseMove()
    {
        if (gameEnded) return;

        moveCount--;
        CheckEndGame();
    }

    // Skor eklemek için çağırılacak metod
    public void AddScore(int amount)
    {
        if (gameEnded) return;

        score += amount;
    }

    // Hamle sayısı 0 olduğunda oyunu bitir ve sonucu göster
    private void CheckEndGame()
    {
        // Oyun zaten bitmişse devam etme
        if (gameEnded)
            return;
        // Hamle kaldıysa oyunu bitirme
        if (moveCount > 0)
            return;

        // Hamleler bittiğinde oyunu bitir
        gameEnded = true;
        if (score >= scoreLimit)
            ShowWinPanel();
        else
            ShowLosePanel();
    }

    // Kazanma panelini aç
    private void ShowWinPanel()
    {
        winPanel.SetActive(true);
        Debug.Log("Oyun kazandı. Win panel gösterildi.");
        // Gerekiyorsa ek işlemler
    }

    // Kaybetme panelini aç
    private void ShowLosePanel()
    {
        losePanel.SetActive(true);
        Debug.Log("Oyun kaybedildi. Lose panel gösterildi.");
        // Gerekiyorsa ek işlemler
    }
}