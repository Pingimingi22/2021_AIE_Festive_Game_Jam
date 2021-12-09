using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager s_Instance = null;


    [Header("World Settings")]
    public int m_Width = 64;
    public int m_Height = 64;

    public GameObject m_BackgroundTilePrefab;
    public GameObject m_SelectionOverlayTilePrefab;

    [HideInInspector]
    public GameObject m_SelectionOverlayObj = null;


    public float m_Cash;
    public float m_TimeLeft;
    public Clickable m_CurrentSelection;
    public float m_GiftsPerSecond;
    public int m_TotalGiftsSent;


    public Vector3 m_HalfExtentBG;


	private void Awake()
	{
        if (s_Instance == null)
        {
            s_Instance = this;
        }
        else
        {
            Debug.Log("You have more than 1 GameManager in the scene!");
        }
	}

	// Start is called before the first frame update
	void Start()
    {
        BoxCollider2D collider = m_BackgroundTilePrefab.GetComponent<BoxCollider2D>();

        m_HalfExtentBG.x = 0.32f;
        m_HalfExtentBG.y = 0.32f;

        m_SelectionOverlayObj = GameObject.Instantiate(m_SelectionOverlayTilePrefab);
        m_SelectionOverlayObj.transform.position = Vector3.zero;

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
                GameObject newBg = GameObject.Instantiate(m_BackgroundTilePrefab);
                Vector2 newPos;
                newPos.x = i * m_HalfExtentBG.x;
                newPos.y = j * m_HalfExtentBG.y;
                newBg.transform.position = newPos;

            }
        }
    }

    /// <summary>
    /// Places selection overlay over clickable object.
    /// </summary>
    /// <param name="clickable"></param>
    public void PlaceOverlay(Clickable clickable)
    {
        m_SelectionOverlayObj.transform.position = clickable.transform.position;
    }

}
