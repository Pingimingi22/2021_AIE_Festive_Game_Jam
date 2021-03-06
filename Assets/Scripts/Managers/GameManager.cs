using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public enum SelectedButton
{ 
    NONE,
    CONVEYOR_BUTTON,
    TEST_WORKSHOP_BUTTON,
    TEST_FACTORY_BUTTON,
    CANON_BUTTON
}
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
    public int m_UnusedElves = 10;
    


    public Clickable m_CurrentSelection;
    public Clickable m_CurrentPlaceable;
    public Clickable m_CurrentClicked;

    public float m_GiftsPerSecond;
    public int m_TotalGiftsSent;


    public Vector3 m_HalfExtentBG;


    // References to game prefabs.
    [Header("Game Prefabs")]
    public Clickable m_ConveyorPrefab;
    public Clickable m_TestWorkshopPrefab;
    public Clickable m_TestFactoryPrefab;
    public Clickable m_CanonFactoryPrefab;

    [Header("UI References")]
    public Image m_ConveyorButtonImg;
    public Image m_TestWorkshopButtonImg;
    public Image m_TestFactoryButtonImg;
    public Image m_CanonFactoryButtonImg;

    public Text m_CashText;
    public Text m_CurrentlySelectedText;
    public Text m_TotalElvesText;

    public Button m_AddElfButton;
    public Button m_RemoveElfButton;

    [Header("Building UI References")]
    public Text m_CurrentlyProducingText;
    public Image m_CurrentlyProducingImage;
    public Text m_CurrentElvesText;

	// Storing information for the world map.
	public List<List<Clickable>> m_WorldGrid;

    [Header("Item Prefabs")]
    public GameObject m_TestItemPrefab;
    public GameObject m_GiftItemPrefab;
    public GameObject m_WoodItemPrefab;


    public Transform m_StartConveyor = null;
    public bool m_IsPlacingConveyor = false;
    public Vector2 testpoint = Vector2.zero;
    public Vector2 testpoint2 = Vector2.zero;
    public HANDLE_TYPE m_HeldHandleDirection;

    public List<GameObject> m_PlannedConveyors = new List<GameObject>();


    public Sprite m_ConveyorSprite;

    public int m_TotalPlacedSegments = 0;

    public float m_CurrentTempSpriteRotation = 0;



    // ui stuff
    SelectedButton m_SelectedButton;

    float m_CameraZoom = 0;


    bool m_IsHoveringOverUI = false;


    public RectTransform m_HUDRect;
    

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
        // Moving camera stuff.

        Vector3 mov = Vector3.zero;
        if (/*Input.mousePosition.x > Screen.width - (Screen.width / 8)*/Input.GetKey(KeyCode.D))
        {
            mov.x += 2 * Time.deltaTime;
            Camera.main.transform.position += mov;
        }
        if (/*Input.mousePosition.x < 0 + (Screen.width / 8)*/Input.GetKey(KeyCode.A))
        {
            mov.x -= 2 * Time.deltaTime;
            Camera.main.transform.position += mov;
        }
        if (/*Input.mousePosition.y < 0 + (Screen.height / 8)*/Input.GetKey(KeyCode.S))
        {
            mov.y -= 2 * Time.deltaTime;
            Camera.main.transform.position += mov;
        }
        if (/*Input.mousePosition.y > Screen.height - (Screen.width / 8)*/Input.GetKey(KeyCode.W))
        {
            mov.y += 2 * Time.deltaTime;
            Camera.main.transform.position += mov;
        }
        m_CameraZoom = Input.mouseScrollDelta.y * 0.1f;

        Camera.main.orthographicSize -= m_CameraZoom;



        if (Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero))
        {
            RaycastHit2D testHit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);

            if (Input.mousePosition.x > (0 + (Screen.width - m_HUDRect.rect.width)))
            {
                m_IsHoveringOverUI = true;
                Debug.Log("Hovering over UI");
            }
            else
                m_IsHoveringOverUI = false;
            

            if (testHit.transform && testHit.transform.tag == "Background" && !m_IsHoveringOverUI)
            {
                //Debug.Log("Testing mouse over function.");
                Clickable clickableThing = testHit.transform.GetComponent<Clickable>();
                if (m_CurrentClicked == null || m_CurrentClicked == clickableThing)
                {
                    clickableThing.DrawSelectionOverlay(1, 1, clickableThing.transform.position);
                }
                SelectTile(clickableThing);
                //Debug.Log("Selected tile is: " + this.gameObject.name);
            }
            //else if(testHit.transform && test)

        }



        if (Input.GetKeyDown(KeyCode.P))
        {
            // Place a present on the conveyer belt that the mouse is currently over.
            if (m_CurrentSelection is Conveyor)
            {
                Conveyor conveyorSelected = (Conveyor)m_CurrentSelection;

                GameObject newItem = GameObject.Instantiate(m_TestItemPrefab);
                conveyorSelected.AddItem(newItem.GetComponent<Item>());

                Item newItemItem = newItem.GetComponent<Item>();
                newItemItem.m_CurrentConveyor = conveyorSelected;
            }
        }

        if (Input.GetMouseButtonDown(1) && !m_IsPlacingConveyor)
        {
            ClearPlaceable();
            Unclick();
        }


        if (m_IsPlacingConveyor)
        {
            Vector2 mousePos = Input.mousePosition;

            RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(mousePos), Vector2.zero);

            testpoint = hit.point;

            // If X is greater, draw horizontally, otherwise draw vertically.
            float xDistance = hit.point.x - m_StartConveyor.position.x;
            float yDistance = hit.point.y - m_StartConveyor.position.y;

            Vector2 toHit = (Vector2)hit.point - (Vector2)m_StartConveyor.position;
            testpoint = hit.point;

            //Debug.Log("xDistance: " + Mathf.Abs(xDistance) + ", yDistance: " + Mathf.Abs(yDistance));



            if (Mathf.Abs(xDistance) > Mathf.Abs(yDistance))
            {
                int requiredSegments = (int)(Mathf.Abs(xDistance) / 0.32f);
                testpoint2.y = m_StartConveyor.position.y;

                // Set conveyor plans based on handle type.
                if (m_HeldHandleDirection == HANDLE_TYPE.RIGHT)
                {
                    testpoint2.x = /*m_StartConveyor.position.x + 0.32f / 2) + */ m_StartConveyor.position.x + requiredSegments * 0.32f;
                }

                if (xDistance < 0)
                {
                    testpoint2.x = /*m_StartConveyor.position.x + 0.32f / 2) + */ m_StartConveyor.position.x + -requiredSegments * 0.32f;
                }
                else
                    testpoint2.x = /*m_StartConveyor.position.x + 0.32f / 2) + */ m_StartConveyor.position.x + requiredSegments * 0.32f;

                if (m_TotalPlacedSegments > requiredSegments)
                {
                    // We need to delete some of the planned conveyor belts.
                    //int amountOfExtra = m_PlannedConveyors.Count - requiredSegments;
                    //for (int i = amountOfExtra; i > 0; i--)
                    //{
                    //    GameObject.Destroy(m_PlannedConveyors[amountOfExtra + m_PlannedConveyors.Count]);
                    //    m_PlannedConveyors.RemoveAt(amountOfExtra + m_PlannedConveyors.Count);
                    //}

                    int amountOfExtra = m_TotalPlacedSegments - requiredSegments;
                    for (int i = m_PlannedConveyors.Count - 1; i > requiredSegments - 1; i--)
                    {
                        m_PlannedConveyors[i].GetComponent<Clickable>().ClearTempSprite();
                        m_PlannedConveyors.RemoveAt(i);
                        m_TotalPlacedSegments--;
                    }
                    //m_PlannedConveyors.Clear();

                    int hi = 5;
                }

                for (int i = 0; i < requiredSegments; i++)
                {
                    if (xDistance < 0)
                    {
                        if (m_HeldHandleDirection == HANDLE_TYPE.LEFT)
                        {
                            for (int j = 0; j < requiredSegments; j++)
                            {
                                Clickable conveyorTile = m_StartConveyor.GetComponent<Clickable>();
                                Clickable newTile = GetTile((int)conveyorTile.m_WorldIndex.x - requiredSegments, (int)conveyorTile.m_WorldIndex.y);
                                newTile.SetTempSprite(m_ConveyorSprite, 180);
                                m_CurrentTempSpriteRotation = 180;

                                if (m_TotalPlacedSegments < requiredSegments)
                                {
                                    m_PlannedConveyors.Add(newTile.gameObject);
                                    //Debug.Log("==================================== Added conveyor to list =========================================");
                                    m_TotalPlacedSegments++;
                                }

                            }

                            //GameObject newPlannedConveyor = GameObject.Instantiate(m_CurrentPlaceable).gameObject;
                            //newPlannedConveyor.transform.position = new Vector3(m_StartConveyor.position.x + -requiredSegments * 0.32f, m_StartConveyor.position.y, 0);
                            //m_PlannedConveyors.Add(newPlannedConveyor);
                        }
                    }
                    else
                    {
                        if (m_HeldHandleDirection == HANDLE_TYPE.RIGHT)
                        {
                            for (int j = 0; j < requiredSegments; j++)
                            {
                                Clickable conveyorTile = m_StartConveyor.GetComponent<Clickable>();
                                Clickable newTile = GetTile((int)conveyorTile.m_WorldIndex.x + requiredSegments, (int)conveyorTile.m_WorldIndex.y);
                                newTile.SetTempSprite(m_ConveyorSprite, 0);
                                m_CurrentTempSpriteRotation = 0;

                                if (m_TotalPlacedSegments < requiredSegments)
                                {
                                    m_PlannedConveyors.Add(newTile.gameObject);
                                    //Debug.Log("==================================== Added conveyor to list =========================================");
                                    m_TotalPlacedSegments++;
                                }

                            }

                            //GameObject newPlannedConveyor = GameObject.Instantiate(m_CurrentPlaceable).gameObject;
                            //newPlannedConveyor.transform.position = new Vector3(m_StartConveyor.position.x + requiredSegments * 0.32f, m_StartConveyor.position.y, 0);
                            //m_PlannedConveyors.Add(newPlannedConveyor);
                        }

                    }
                }
            }
            else
            {
                int requiredSegments = (int)(Mathf.Abs(yDistance) / 0.32f);
                testpoint2.x = m_StartConveyor.position.x;

                if (yDistance < 0)
                {
                    testpoint2.y = /*m_StartConveyor.position.x + 0.32f / 2) + */ m_StartConveyor.position.y + -requiredSegments * 0.32f;
                }
                else
                    testpoint2.y = /*m_StartConveyor.position.x + 0.32f / 2) + */ m_StartConveyor.position.y + requiredSegments * 0.32f;



                if (m_TotalPlacedSegments > requiredSegments)
                {
                    // We need to delete some of the planned conveyor belts.
                    //int amountOfExtra = m_PlannedConveyors.Count - requiredSegments;
                    //for (int i = amountOfExtra; i > 0; i--)
                    //{
                    //    GameObject.Destroy(m_PlannedConveyors[amountOfExtra + m_PlannedConveyors.Count]);
                    //    m_PlannedConveyors.RemoveAt(amountOfExtra + m_PlannedConveyors.Count);
                    //}

                    int amountOfExtra = m_TotalPlacedSegments - requiredSegments;
                    for (int i = m_PlannedConveyors.Count - 1; i > requiredSegments - 1; i--)
                    {
                        m_PlannedConveyors[i].GetComponent<Clickable>().ClearTempSprite();
                        m_PlannedConveyors.RemoveAt(i);
                        m_TotalPlacedSegments--;
                    }
                    //m_PlannedConveyors.Clear();

                    int hi = 5;
                }


                for (int i = 0; i < requiredSegments; i++)
                {


                    if (yDistance < 0)
                    {
                        if (m_HeldHandleDirection == HANDLE_TYPE.DOWN)
                        {
                            for (int j = 0; j < requiredSegments; j++)
                            {
                                Clickable conveyorTile = m_StartConveyor.GetComponent<Clickable>();
                                Clickable newTile = GetTile((int)conveyorTile.m_WorldIndex.x, (int)conveyorTile.m_WorldIndex.y - requiredSegments);
                                newTile.SetTempSprite(m_ConveyorSprite, -90);
                                m_CurrentTempSpriteRotation = -90;

                                if (m_TotalPlacedSegments < requiredSegments)
                                {
                                    m_PlannedConveyors.Add(newTile.gameObject);
                                    //Debug.Log("==================================== Added conveyor to list =========================================");
                                    m_TotalPlacedSegments++;
                                }
                            }



                            //GameObject newPlannedConveyor = GameObject.Instantiate(m_CurrentPlaceable).gameObject;
                            //newPlannedConveyor.transform.position = new Vector3(m_StartConveyor.position.x + -requiredSegments * 0.32f, m_StartConveyor.position.y, 0);
                            //m_PlannedConveyors.Add(newPlannedConveyor);
                        }

                    }
                    else
                    {

                        if (m_HeldHandleDirection == HANDLE_TYPE.UP)
                        {
                            for (int j = 0; j < requiredSegments; j++)
                            {
                                Clickable conveyorTile = m_StartConveyor.GetComponent<Clickable>();
                                Clickable newTile = GetTile((int)conveyorTile.m_WorldIndex.x, (int)conveyorTile.m_WorldIndex.y + requiredSegments);
                                newTile.SetTempSprite(m_ConveyorSprite, 90);

                                m_CurrentTempSpriteRotation = 90;

                                if (m_TotalPlacedSegments < requiredSegments)
                                {
                                    m_PlannedConveyors.Add(newTile.gameObject);
                                    //Debug.Log("==================================== Added conveyor to list =========================================");
                                    m_TotalPlacedSegments++;
                                }
                            }



                            //GameObject newPlannedConveyor = GameObject.Instantiate(m_CurrentPlaceable).gameObject;
                            //newPlannedConveyor.transform.position = new Vector3(m_StartConveyor.position.x + requiredSegments * 0.32f, m_StartConveyor.position.y, 0);
                            //m_PlannedConveyors.Add(newPlannedConveyor);
                        }
                    }
                }


            }
            //else
            //{
            //    int requiredSegments = (int)(yDistance / 0.32f);
            //    testpoint2.x = m_StartConveyor.position.x;
            //    testpoint2.y = requiredSegments * 0.32f;
            //}


        }
        else if (Input.GetMouseButton(0))
        {
            Vector2 mousePos = Input.mousePosition;

            RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(mousePos), Vector2.zero);

            testpoint = hit.point;

            if (hit.transform && hit.transform.tag == "CraftingBuilding")
            {
                Click(hit.transform.GetComponent<Clickable>());
            }

        }


        UpdateGenericUI();
        UpdateCraftingBuildingUI();
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawSphere(testpoint, 0.1f);

        if (m_StartConveyor)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(m_StartConveyor.position, testpoint2);
            Gizmos.DrawSphere(m_StartConveyor.position, 0.05f);
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
    public void PlaceOverlay(Vector3 position)
    {
        m_SelectionOverlayObj.transform.position = position;
    }

    public void SelectTile(Clickable tileToSelect)
    {
        
        m_CurrentSelection = (Clickable)tileToSelect;

        if (tileToSelect.m_Type == TileTypes.CONVEYOR)
        {
            //Debug.Log("test");
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

    public void ResizeSelectionOverlay(float width, float height)
    {
        SpriteRenderer selectionRenderer = m_SelectionOverlayObj.GetComponent<SpriteRenderer>();
        selectionRenderer.transform.localScale = new Vector3(width, height, 1);
    }

    public void ClearAllPlannedConveyor()
    {
        for (int i = m_PlannedConveyors.Count - 1; i > -1; i--)
        {
            m_PlannedConveyors[i].GetComponent<Clickable>().ClearTempSprite();
            m_PlannedConveyors.RemoveAt(i);
            m_TotalPlacedSegments--;
        }

        if (m_PlannedConveyors.Count > 0)
        {
            //Debug.LogError("ClearAllPlannedConveyor() did not clear all!");
        }
    }

    public void StopPlanning()
    {
        m_IsPlacingConveyor = false;
        ClearAllPlannedConveyor();
    }

    public void ConfirmConveyorPlan()
    {
        for (int i = 0; i < m_PlannedConveyors.Count; i++)
        {
            Clickable planClickable = m_PlannedConveyors[i].gameObject.GetComponent<Clickable>();

            Clickable newRealConveyor = GameObject.Instantiate(m_ConveyorPrefab);
            newRealConveyor.transform.position = planClickable.transform.position;
            newRealConveyor.ClearTempSprite(); // Newly instantiated conveyors have their temporary renderered enabled so I hide it like this.
            newRealConveyor.m_WorldIndex = m_PlannedConveyors[i].GetComponent<Clickable>().m_WorldIndex; // Assign world index to new conveyor.

            // Have to disable collider of background tile since we've placed something on top of it.
            m_WorldGrid[(int)newRealConveyor.m_WorldIndex.x][(int)newRealConveyor.m_WorldIndex.y].GetComponent<BoxCollider2D>().enabled = false;

            // Have to tell the world that this new conveyor belt should replace the current world index reference.
            m_WorldGrid[(int)newRealConveyor.m_WorldIndex.x][(int)newRealConveyor.m_WorldIndex.y] = newRealConveyor;

            //SpriteRenderer newConveyorRenderer = newRealConveyor.gameObject.GetComponentInChildren<SpriteRenderer>();
            //newConveyorRenderer.transform.eulerAngles = new Vector3(0, 0, m_CurrentTempSpriteRotation);

            
            
            
            // Setting inputs/outputs and connections for this confirmed choice of conveyor belts.
            Conveyor newConveyorConveyor = (Conveyor)newRealConveyor;
            newConveyorConveyor.RotateSprite(m_HeldHandleDirection);
            int connectionNum = GetNumDirFromHandleDir(m_HeldHandleDirection);

            if(i != m_PlannedConveyors.Count - 1) // Don't want to link the last conveyor because it might fall off of the line.
                newConveyorConveyor.m_ConnectionArray.m_Connections[connectionNum] = 1;

            Conveyor startConveyor = m_StartConveyor.GetComponent<Conveyor>();
            if (startConveyor == null)
            {
                int hi = 5;
            }
            m_StartConveyor.GetComponent<Conveyor>().SetConnection(GetNumDirFromHandleDir(m_HeldHandleDirection), 1);
            startConveyor.RotateSprite(m_HeldHandleDirection);

            // We have to account for the fact that factory conveyor belts are not truly there. So to get around this we will place a conveyor belt manually there.
            //if (m_StartConveyor == null)
            //{ 
            //    ManuallyPlaceConveyor(
            //}

            if (i == m_PlannedConveyors.Count - 1)
            {
                // If this is the last conveyor in the plans, we want to check it's surrounding conveyors and connect with them if any
                // exist.
                int oppositeNum = GetOppositeNumDirFromHandleDir(m_HeldHandleDirection);
                int dir = GetNumDirFromHandleDir(m_HeldHandleDirection);
                List<Conveyor> surroundingConveyors = newConveyorConveyor.GetSurroundingConveyors(dir, oppositeNum);
                if (surroundingConveyors.Count > 0)
                {
                    // We have surrounding conveyors.
                    int hi = 5;

                    int leftDirNum = GetNumDirFromHandleDir(HANDLE_TYPE.LEFT);
                    int rightDirNum = GetNumDirFromHandleDir(HANDLE_TYPE.RIGHT);
                    int upDirNum = GetNumDirFromHandleDir(HANDLE_TYPE.UP);
                    int downDirNum = GetNumDirFromHandleDir(HANDLE_TYPE.DOWN);
                    surroundingConveyors[0].SetConnection((int)oppositeNum, -1);

                    // If we have something to connect to, connect.
                    newConveyorConveyor.m_ConnectionArray.m_Connections[connectionNum] = 1;
                }
            }

        }
        StopPlanning();        
    }

    public int GetNumDirFromHandleDir(HANDLE_TYPE handleDir)
    {
        switch (handleDir)
        {
            case HANDLE_TYPE.RIGHT:
                return 0;
            case HANDLE_TYPE.DOWN:
                return 1;
            case HANDLE_TYPE.LEFT:
                return 2;
            case HANDLE_TYPE.UP:
                return 3;

            default:
                Debug.LogError("GetNumDirFromHandleDir has broken!");
                return -1;
        }
    }
    public int GetOppositeNumDirFromHandleDir(HANDLE_TYPE handleDir)
    {
        switch (handleDir)
        {
            case HANDLE_TYPE.RIGHT:
                return 2;
            case HANDLE_TYPE.DOWN:
                return 3;
            case HANDLE_TYPE.LEFT:
                return 0;
            case HANDLE_TYPE.UP:
                return 1;

            default:
                Debug.LogError("GetNumDirFromHandleDir has broken!");
                return -1;
        }
    }


    public void SelectPlaceable(Clickable placeablePrefab)
    {
        Unclick();

        m_CurrentPlaceable = placeablePrefab;

        if (placeablePrefab == m_ConveyorPrefab)
        {
            m_SelectedButton = SelectedButton.CONVEYOR_BUTTON;
        }
        else if (placeablePrefab == m_TestFactoryPrefab)
        {
            m_SelectedButton = SelectedButton.TEST_FACTORY_BUTTON;
        }
        else if (placeablePrefab == m_TestWorkshopPrefab)
        {
            m_SelectedButton = SelectedButton.TEST_WORKSHOP_BUTTON;
        }
        else if (placeablePrefab == m_CanonFactoryPrefab)
        {
            m_SelectedButton = SelectedButton.CANON_BUTTON;
        }
        else
            m_SelectedButton = SelectedButton.NONE;


        SelectButton();
    }
    public void ClearPlaceable()
    {
        m_CurrentPlaceable = null;
        m_SelectedButton = SelectedButton.NONE;

        m_ConveyorButtonImg.color = Color.white;
        m_TestFactoryButtonImg.color = Color.white;
        m_TestWorkshopButtonImg.color = Color.white;
        m_CanonFactoryButtonImg.color = Color.white;
    }

    public void SelectButton()
    {
        m_ConveyorButtonImg.color = Color.white;
        m_TestFactoryButtonImg.color = Color.white;
        m_TestWorkshopButtonImg.color = Color.white;
        m_CanonFactoryButtonImg.color = Color.white;


        switch (m_SelectedButton)
        {
            case SelectedButton.CONVEYOR_BUTTON:
                m_ConveyorButtonImg.color = Color.gray;
                break;
            case SelectedButton.TEST_FACTORY_BUTTON:
                m_TestFactoryButtonImg.color = Color.gray;
                break;
            case SelectedButton.TEST_WORKSHOP_BUTTON:
                m_TestWorkshopButtonImg.color = Color.gray;
                break;
            case SelectedButton.CANON_BUTTON:
                m_CanonFactoryButtonImg.color = Color.gray;
                break;

        }
    }

    public void UpdateGenericUI()
    {
        m_CashText.text = "Cash: " + m_Cash.ToString();
        if (m_CurrentClicked)
        {
            m_CurrentlySelectedText.text = "Clicked: " + m_CurrentClicked.name;
        }
        else if (m_CurrentSelection)
        {
            m_CurrentlySelectedText.text = "Hovering: " + m_CurrentSelection.name;
        }
        else
        {
            m_CurrentlySelectedText.text = "Hovering: N/A";
        }

        m_TotalElvesText.text = "Lazy Elves: " + m_UnusedElves.ToString();

        m_CurrentElvesText.enabled = false;

        m_CurrentlyProducingText.enabled = false;
        m_CurrentlyProducingImage.enabled = false;

        m_AddElfButton.gameObject.active = false;
        m_RemoveElfButton.gameObject.active = false;
       
    }

    public void UpdateCraftingBuildingUI()
    {
        if (m_CurrentClicked && m_CurrentClicked.tag == "CraftingBuilding")
        {
            ProductionBuilding building = (ProductionBuilding)m_CurrentClicked;
            m_CurrentElvesText.text = "Elves: " + building.m_CurrentElves.ToString();
            m_CurrentlyProducingText.enabled = true;
            m_CurrentlyProducingImage.sprite = building.m_ProductionItem.GetComponent<SpriteRenderer>().sprite;
            m_CurrentlyProducingImage.enabled = true;

            m_AddElfButton.gameObject.active = true;
            m_RemoveElfButton.gameObject.active = true;

            m_CurrentElvesText.enabled = true;
        }
        else if (m_CurrentClicked && m_CurrentClicked.tag == "Canon")
        {
            GiftCanon canon = (GiftCanon)m_CurrentClicked;

            m_CurrentElvesText.text = "Elves: N/A"; //+ canon.m_CurrentElves.ToString();
            m_CurrentlyProducingText.enabled = false;
            //m_CurrentlyProducingImage.sprite = building.m_ProductionItem.GetComponent<SpriteRenderer>().sprite;
            m_CurrentlyProducingImage.enabled = false;

            //m_AddElfButton.gameObject.active = true;
            //m_RemoveElfButton.gameObject.active = true;

            m_CurrentElvesText.enabled = true;

        }
    }

    public void Click(Clickable clickable)
    {
        m_CurrentClicked = clickable;
        //m_CurrentSelection = null;
        m_CurrentSelection = clickable;
    }
    public void Unclick()
    {
        m_CurrentClicked = null;
    }

    public void BuyElf()
    {
        if (m_Cash - 5 > 0)
        {
            m_UnusedElves++;
            m_Cash -= 5;
        }
    }
    public void SellElf()
    {
        m_UnusedElves--;
        m_Cash += 3.5f;
    }

    public void AddElf()
    {
        ProductionBuilding building = (ProductionBuilding)m_CurrentClicked;
        building.AddElf();
    }
    public void RemoveElf()
    {
        ProductionBuilding building = (ProductionBuilding)m_CurrentClicked;
        building.RemoveElf();
    }
}
