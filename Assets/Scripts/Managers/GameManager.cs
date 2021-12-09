using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Header("World Settings")]
    public int m_Width = 64;
    public int m_Height = 64;

    public GameObject m_BackgroundTile;


    public float m_Cash;
    public float m_TimeLeft;
    public Clickable m_CurrentSelection;
    public float m_GiftsPerSecond;
    public int m_TotalGiftsSent;


    public Vector3 m_HalfExtentBG;


    // Start is called before the first frame update
    void Start()
    {
        BoxCollider2D collider = m_BackgroundTile.GetComponent<BoxCollider2D>();

        m_HalfExtentBG.x = 0.32f;
        m_HalfExtentBG.y = 0.32f;

        GenerateBackground();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void GenerateBackground()
    {
        for (int i = 0; i < m_Width; i++)
        {
            for (int j = 0; j < m_Height; j++)
            {
                GameObject newBg = GameObject.Instantiate(m_BackgroundTile);
                Vector2 newPos;
                newPos.x = i * m_HalfExtentBG.x;
                newPos.y = j * m_HalfExtentBG.y;
                newBg.transform.position = newPos;

            }
        }
    }
}
