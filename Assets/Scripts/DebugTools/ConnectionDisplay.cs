using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class ConnectionDisplay : MonoBehaviour
{

    public TextMeshProUGUI m_NorthText;
    public TextMeshProUGUI m_SouthText;
    public TextMeshProUGUI m_EastText;
    public TextMeshProUGUI m_WestText;

    public Structure m_Structure;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        m_NorthText.text = m_Structure.m_ConnectionArray.m_Connections[0].ToString();
        m_SouthText.text = m_Structure.m_ConnectionArray.m_Connections[1].ToString();
        m_EastText.text = m_Structure.m_ConnectionArray.m_Connections[2].ToString();
        m_WestText.text = m_Structure.m_ConnectionArray.m_Connections[3].ToString();
   
    }
}
