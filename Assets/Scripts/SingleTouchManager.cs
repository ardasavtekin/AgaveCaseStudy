using UnityEngine;

public class SingleTouchManager : MonoBehaviour
{
    void Awake()
    {
        Input.multiTouchEnabled = false;
    }

    void Update()
    {
        if (Input.touchCount == 0)
            return;

        Touch t = Input.GetTouch(0);

        if (t.phase == TouchPhase.Began)
        {
            Vector3 wp = Camera.main.ScreenToWorldPoint(new Vector3(t.position.x, t.position.y, 10f));
           
        }
    }
} 