using UnityEngine;
using System;
//using Vuforia;
using System.IO;
using HoloToolkit;
using System.Collections;

public class ButterflyPrefabPopulator : MonoBehaviour {

    public GameObject prefab;
    public Vector3 originButterflyCloud = new Vector3(0, -1, 8);
    public static int nButterflies = 30;
    public int minX = 0;
    public int maxX = 1;
    public int minY = -1;
    public int maxY = 1;
    public int minZ = 2;
    public int maxZ = 4;
    System.Random rnd = new System.Random();


    public float m_Speed = 1;
    public float m_XScale = 1;
    public float m_YScale = 1;
    public float m_ZScale = 1;

    private GameObject[] butterflies = new GameObject[nButterflies];
    private Vector3[] m_Pivot = new Vector3[nButterflies];
    private Vector3[] m_PivotOffset = new Vector3[nButterflies];
    private float[] m_Phase = new float[nButterflies];
    private bool[] m_Invert = new bool[nButterflies];
    private float m_2PI = Mathf.PI * 2;

    private Color highlightColor = new Color(255, 0, 0, 1);

    private int heartRate;
    private int movementMode = 2; // THIS CONTROLS WHICH movement code we use below (1=A, 2=B, ...)

    private long updateIter = 0;

    private float glowDistance = 0.9f; // The distance a user's hand has to be from a butterfly to make it glow



    // Use this for initialization
    void Start() {
        InstantiateButterflies();        
    }

    private void InstantiateButterflies()
    {
        for (int i = 0; i < nButterflies; i++)
        {
            butterflies[i] = (GameObject)Instantiate(prefab, originButterflyCloud, Quaternion.identity);
            Behaviour halo = (Behaviour)butterflies[i].GetComponent("Halo");
            Behaviour innerHalo = (Behaviour)butterflies[i].transform.GetChild(0).gameObject.GetComponent("Halo");
            halo.enabled = false;
            innerHalo.enabled = false;

            //butterflies[i].transform.position = new Vector3(0.001f*i*rnd.Next(-5,5), 0.001f*i*rnd.Next(-5,5), -0.001f*i*rnd.Next(-5,5));

            int x = rnd.Next(minX, maxX);
            int y = rnd.Next(minY, maxY);
            int z = rnd.Next(minZ, maxZ);
            m_Pivot[i] = new Vector3(x, y, z);
            m_Phase[i] = rnd.Next(0, 6);
            m_Invert[i] = m_Phase[i] > 3;
            butterflies[i].transform.position = m_Pivot[i];
        }
    }


    // http://ec2-54-165-135-172.compute-1.amazonaws.com/get_hr

    IEnumerator WaitForRequest(WWW www)
    {
        yield return www;

        // check for errors
        if (www.error == null)
        {
            Debug.Log("WWW Ok!: " + www.data);
            heartRate = Int32.Parse(www.text);
        }
        else
        {
            Debug.Log("WWW Error: " + www.error);
        }
    }

    Vector3 movementA(long iter)
    {
        //double periodInIter = heartRate
        double periodInIter = 100;
        double amplitude = 0.08;
        double movement = amplitude*Math.Sin(2 * Math.PI * iter / periodInIter);
        return new Vector3(0, (float)movement, 0);
    }

    Vector3 movementB(long iter)
    {
        //double periodInIter = heartRate
        double periodInIter = 100;
        double amplitude = 0.08;
        double movement = amplitude * Math.Sin(2 * Math.PI * iter / periodInIter);
        double xMovement = amplitude * 0.5 * Math.Cos(2 * Math.PI * iter / periodInIter);
        return new Vector3((float)xMovement, (float)movement, 0);
    }

    Vector3 movementC(long iter)
    {
        //double periodInIter = heartRate
        int periodInIter = 100;
        double pseudoPhase = ((int)iter % periodInIter)/(double)periodInIter;
        double amplitude = 0.08;

        double xMovement = 0;
        double yMovement = 0;
        double zMovement = 0;

        if (pseudoPhase < 0.5)
        {
            xMovement = amplitude * 0.4 * Math.Cos(2 * Math.PI * pseudoPhase);
            yMovement = amplitude * Math.Cos(2 * Math.PI * pseudoPhase);
            zMovement = amplitude * 0.2 * Math.Cos(2 * Math.PI * pseudoPhase);
        } else if (pseudoPhase < 0.75)
        {
            xMovement = 0;
            yMovement = 0;
            zMovement = amplitude * 0.1;
        } else
        {
            xMovement = 0;
            yMovement = 0;
            zMovement = amplitude * -0.1;
        }

        return new Vector3((float)xMovement, (float)yMovement, (float)zMovement);
    }

    Vector3 movementD(long iter)
    {
        if (iter < 10000) return new Vector3(0, 0, 0.01f);
        else return new Vector3(0, 0.01f, 0);
    }

    Vector3 movementE(long iter)
    {
        //m_PivotOffset[i] = Vector3.up * 2 * m_YScale;

        //m_Phase[i] += m_Speed * Time.deltaTime;
        //if (m_Phase[i] > m_2PI)
        //{
        //    m_Invert[i] = !m_Invert[i];
        //    m_Phase[i] -= m_2PI;
        //}
        //if (m_Phase[i] < 0) m_Phase[i] += m_2PI;

        //butterflies[i].transform.position = m_Pivot[i] + (m_Invert[i] ? m_PivotOffset[i] : Vector3.zero);
        //butterflies[i].transform.position = butterflies[i].transform.position +
        //    new Vector3(Mathf.Sin(m_Phase[i]) * m_XScale,
        //                Mathf.Cos(m_Phase[i]) * (m_Invert[i] ? -1 : 1) * m_YScale,
        //                Mathf.Sin(m_Phase[i]*3) * m_ZScale);
        if (iter < 10000) return new Vector3(0, 0, 0.01f);
        else return new Vector3(0, 0.01f, 0);
    }

    private Vector3 getMovementIncrement(long updateIter)
    {
        switch (movementMode)
        {
            case 1:
                return movementA(updateIter);
            case 2:
                return movementB(updateIter);
            case 3:
                return movementC(updateIter);
            case 4:
                return movementD(updateIter);
            default:
                return movementE(updateIter);
        }
    }


    // Update is called once per frame
    void Update() {

        Debug.Log("hisies");

        WWW www = new WWW("http://ec2-54-165-135-172.compute-1.amazonaws.com/get_hr");
        StartCoroutine(WaitForRequest(www));
        Debug.Log("HEART RATE: " + heartRate);

        Vector3 currentHandPosition = GestureAction.gesturePosition;

        updateIter += 1;

        Debug.Log("CURRENT HAND POSITION: " + currentHandPosition.ToString()); 

        
        for (int i = 0; i < nButterflies; i++)
        {
            // This is the object that corresponds to the "real" butterfly, not the 
            // parent container object
            GameObject realButterfly = butterflies[i].transform.GetChild(0).gameObject;
            Behaviour halo = (Behaviour)realButterfly.GetComponent("Halo");
            if (Vector3.Distance(currentHandPosition, butterflies[i].transform.position) < glowDistance && Vector3.Distance(currentHandPosition, new Vector3(0,0,0)) > 0) 
            {
                // CHANGE BUTTERFLY SOMEHOW
                //butterflies[i].transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
                realButterfly.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);

                halo.enabled = true;

                Debug.Log("HAND POS: " + currentHandPosition.ToString());
                Debug.Log("BUTTERFLY POS: " + butterflies[i].transform.position.ToString());
                Debug.Log("DISTANCE: " + Vector3.Distance(currentHandPosition, butterflies[i].transform.position).ToString());
            }
            else
            {
                //butterflies[i].transform.localScale = new Vector3(1, 1, 1);
                realButterfly.transform.localScale = new Vector3(1f, 1f, 1f);
                halo.enabled = false;
            }

            //m_PivotOffset[i] = Vector3.up * 2 * m_YScale;

            //m_Phase[i] += m_Speed * Time.deltaTime;
            //if (m_Phase[i] > m_2PI)
            //{
            //    m_Invert[i] = !m_Invert[i];
            //    m_Phase[i] -= m_2PI;
            //}
            //if (m_Phase[i] < 0) m_Phase[i] += m_2PI;

            //butterflies[i].transform.position = m_Pivot[i] + (m_Invert[i] ? m_PivotOffset[i] : Vector3.zero);
            //butterflies[i].transform.position = butterflies[i].transform.position +
            //    new Vector3(Mathf.Sin(m_Phase[i]) * m_XScale,
            //                Mathf.Cos(m_Phase[i]) * (m_Invert[i] ? -1 : 1) * m_YScale,
            //                Mathf.Sin(m_Phase[i]*3) * m_ZScale);


            // Call Team PJE's custom movement function for the butterfly.
            //Vector3 movementTrajectory = movementA(updateIter);
            Vector3 movementTrajectory = getMovementIncrement(updateIter);
            butterflies[i].transform.position = butterflies[i].transform.position + movementTrajectory;
            
            
            


            
        }
	}

}
