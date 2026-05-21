// GridAnimator.cs — Assets/Scripts/UI/GridAnimator.cs
using UnityEngine;
using UnityEngine.UI;

public class GridAnimator : MonoBehaviour
{
    public float speed = 0.008f;

    // Tiles a mostrar (1920/60 = 32 horizontal, 1080/60 = 18 vertical)
    public float tilesX = 32f;
    public float tilesY = 18f;

    RawImage ri;
    float ox, oy;

    void Awake() => ri = GetComponent<RawImage>();

    void Update()
    {
        ox = (ox + speed * Time.deltaTime) % 1f;
        oy = (oy + speed * Time.deltaTime) % 1f;
        if (ri != null) ri.uvRect = new Rect(ox, oy, tilesX, tilesY);
    }
}