using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager s_Instance = null;

    public Camera m_Camera;


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
    public Clickable m_CurrentPlaceable;

    public float m_GiftsPerSecond;
    public int m_TotalGiftsSent;


    public Vector3 m_HalfExtentBG;

    // Storing information for the world map.
    public List<List<Clickable>> m_WorldGrid;

    public GameObject m_TestItemPrefab;


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
        if (Input.GetKeyDown(KeyCode.P))
        {
            // Place a present on the conveyer belt that the mouse is currently over.
            if (m_CurrentSelection is Conveyor)
            {
                Conveyor conveyorSelected = (Conveyor)m_CurrentSelection;

                GameObject newItem = GameObject.Instantiate(m_TestItemPrefab);
                conveyorSelected.AddItem(newItem.GetComponent<Item>());
            }
        }
    }

   
    public void GenerateBackground()
    {
        m_WorldGrid = new List<List<Clickable>>();
        for (int i = 0; i < m_Width; i++)
        {
            m_WorldGrid.Add(new List<Clickable>());
            for (int j = 0; j < m_Height; j++)
            {
                GameObject newBg = GameObject.Instantiate(m_BackgroundTilePrefab);
                Vector2 newPos;
                newPos.x = i * m_HalfExtentBG.x;
                newPos.y = j * m_HalfExtentBG.y;
                newBg.transform.position = newPos;

                Clickable bgClickable = newBg.GetComponent<Clickable>();

                // Initialising world grid.
                m_WorldGrid[i].Add(bgClickable);

                bgClickable.m_WorldIndex.x = i;
                bgClickable.m_WorldIndex.y = j;
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

    public void SelectTile(Clickable tileToSelect)
    {
        m_CurrentSelection = (Clickable)tileToSelect;

        if (tileToSelect.m_Type == TileTypes.CONVEYOR)
        {
            Debug.Log("test");
        }
    }

    public Clickable GetTile(int x, int y)
    {
        if (x < m_Width && x > -1)
        {
            if (y < m_Height && y > -1)
            {
                // Passed in a valid location on the grid.
                return m_WorldGrid[x][y];

            }
        }

        return null;
        
    }

    public void UpdateTile(int xIndex, int yIndex, Clickable newTile)
    {
        m_WorldGrid[xIndex][yIndex] = newTile;
    }
}
