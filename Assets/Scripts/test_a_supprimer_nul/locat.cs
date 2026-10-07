using UnityEngine;

public class locat : MonoBehaviour
{
    public GameObject missionManager;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        missionManager = GameObject.FindWithTag("missionManager");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("collided");
        if (other.CompareTag("Player"))
        {
            Debug.Log("fin");
            missionManager.GetComponent<MissionManager>().endMission();
        }
    }






}
