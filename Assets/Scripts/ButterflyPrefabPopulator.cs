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
    public int maxX = 4;
    public int minY = -3;
    public int maxY = 3;
    public int minZ = 2;
    public int maxZ = 8;
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

    private long updateIter = 0;


    //private TrackableBehaviour mTrackableBehavior;

    // Use this for initialization
    void Start() {
        //mTrackableBehavior = GetComponent<TrackableBehaviour>();

        //if (mTrackableBehavior)
        //{
        //    mTrackableBehavior.RegisterTrackableEventHandler(this);
        //}
        InstantiateButterflies();        
    }

    private void InstantiateButterflies()
    {
        for (int i = 0; i < nButterflies; i++)
        {
            butterflies[i] = (GameObject)Instantiate(prefab, originButterflyCloud, Quaternion.identity);

            //if (mTrackableBehavior.transform)
            //{
            //    butterflies[i].transform.parent = mTrackableBehavior.transform;
            //}

            butterflies[i].transform.position = new Vector3(0.001f*i*rnd.Next(-5,5), 0.001f*i*rnd.Next(-5,5), -0.001f*i*rnd.Next(-5,5));

            int x = rnd.Next(minX, maxX);
            int y = rnd.Next(minY, maxY);
            int z = rnd.Next(minZ, maxZ);
            m_Pivot[i] = new Vector3(x, y, z);
            m_Phase[i] = rnd.Next(0, 6);
            m_Invert[i] = m_Phase[i] > 3;
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
        return new Vector3(0, 0.001f, 0);
    }

    Vector3 movementB(long iter)
    {
        if (iter < 10000) return new Vector3(0, 0, 0.01f);
        else return new Vector3(0, 0.01f, 0);
    }

    Vector3 movementC(long iter)
    {
        if (iter < 10000) return new Vector3(0, 0, 0.01f);
        else return new Vector3(0, 0.01f, 0);
    }

    Vector3 movementD(long iter)
    {
        if (iter < 10000) return new Vector3(0, 0, 0.01f);
        else return new Vector3(0, 0.01f, 0);
    }

    Vector3 movementE(long iter)
    {
        if (iter < 10000) return new Vector3(0, 0, 0.01f);
        else return new Vector3(0, 0.01f, 0);
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
            
            if (Vector3.Distance(currentHandPosition, butterflies[i].transform.position) < 3 && Vector3.Distance(currentHandPosition, new Vector3(0,0,0)) > 0) 
            {
                // CHANGE BUTTERFLY SOMEHOW
                butterflies[i].transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);

                Debug.Log("HAND POS: " + currentHandPosition.ToString());
                Debug.Log("BUTTERFLY POS: " + butterflies[i].transform.position.ToString());
                Debug.Log("DISTANCE: " + Vector3.Distance(currentHandPosition, butterflies[i].transform.position).ToString());
            }
            else
            {
                butterflies[i].transform.localScale = new Vector3(1, 1, 1);
            }


            m_PivotOffset[i] = Vector3.up * 2 * m_YScale;

            m_Phase[i] += m_Speed * Time.deltaTime;
            if (m_Phase[i] > m_2PI)
            {
                m_Invert[i] = !m_Invert[i];
                m_Phase[i] -= m_2PI;
            }
            if (m_Phase[i] < 0) m_Phase[i] += m_2PI;

            // Call Team PJE's custom movement function for the butterfly.
            Vector3 movementTrajectory = movementA(updateIter);
            butterflies[i].transform.position = butterflies[i].transform.position + movementTrajectory;
            
            
            //butterflies[i].transform.position = m_Pivot[i] + (m_Invert[i] ? m_PivotOffset[i] : Vector3.zero);
            //butterflies[i].transform.position = butterflies[i].transform.position +
            //    new Vector3(Mathf.Sin(m_Phase[i]) * m_XScale,
            //                Mathf.Cos(m_Phase[i]) * (m_Invert[i] ? -1 : 1) * m_YScale,
            //                Mathf.Sin(m_Phase[i]*3) * m_ZScale);


            
        }
	}

//    public void OnTrackableStateChanged(TrackableBehaviour.Status previousStatus,
//                                       TrackableBehaviour.Status newStatus)
//    {
//        Debug.Log("TRACKABLE STATE CHANGED!");
//        if (newStatus == TrackableBehaviour.Status.DETECTED) Debug.Log("DETECTED");
//        if (newStatus == TrackableBehaviour.Status.TRACKED) Debug.Log("TRACKED");
//        if (newStatus == TrackableBehaviour.Status.EXTENDED_TRACKED) Debug.Log("EXTENDED TRACKED");

//        if (newStatus == TrackableBehaviour.Status.DETECTED ||
//            newStatus == TrackableBehaviour.Status.TRACKED ||
//            newStatus == TrackableBehaviour.Status.EXTENDED_TRACKED)
//        {
//            OnTrackingFound();
//        }
//    }

//    private void OnTrackingFound()
//    {
//        //InstantiateButterflies();

//        Debug.Log("TRAVCKING FOUND");
//    }
}
