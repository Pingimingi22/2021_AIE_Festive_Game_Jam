using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Clickable : MonoBehaviour
{
    public bool m_IsClicked;


    public void Click()
    { }

    private void Select()
    { }

    private void Deselect()
    { }


	// Start is called before the first frame update
	void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

	private void OnMouseOver()
	{
        Debug.Log("Testing mouse over function.");
	}
}
