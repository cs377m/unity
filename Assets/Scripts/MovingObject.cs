using UnityEngine;
using System.Collections;
using System.Net;
using System;

public class MovingObject : MonoBehaviour {

    // TODO: THESE VALUES NEED TO BE UPDATED BASED ON BIOFEEDBACK FROM THE BAND!!!
    public float m_Speed = 1;
    public float m_XScale = 1;
    public float m_YScale = 1;
 
    private Vector3 m_Pivot;
    private Vector3 m_PivotOffset;
    private float m_Phase;
    private bool m_Invert = false;
    private float m_2PI = Mathf.PI* 2;

    // THIS IS THE USER'S CURRENT HEART RATE
    private int heartRate = 0;
    private int updateCount = 0;
 
    void Start()
    {
        m_Pivot = transform.position;
    }

    void Update()
    {
        Debug.Log("yoooooo");
        updateCount++;
        if (updateCount == 100)
        {
            updateCount = 0;

            //WebRequest request = WebRequest.Create("http://ec2-54-165-135-172.compute-1.amazonaws.com/get_hr");


            
        }


        m_PivotOffset = Vector3.up * 2 * m_YScale;

        m_Phase += m_Speed * Time.deltaTime;
        if (m_Phase > m_2PI)
        {
            m_Invert = !m_Invert;
            m_Phase -= m_2PI;
        }
        if (m_Phase < 0) m_Phase += m_2PI;

        transform.position = m_Pivot + (m_Invert ? m_PivotOffset : Vector3.zero);
        transform.position = transform.position + new Vector3(Mathf.Sin(m_Phase) * m_XScale, Mathf.Cos(m_Phase) * (m_Invert ? -1 : 1) * m_YScale, 0);
        //transform.position.x += Mathf.Sin(m_Phase) * m_XScale;
        //transform.position.y += Mathf.Cos(m_Phase) * (m_Invert ? -1 : 1) * m_YScale;
    }

}
