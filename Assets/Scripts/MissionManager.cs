using System.Collections.Generic;
using Unity.VisualScripting;
using Unity.VisualScripting.Dependencies.Sqlite;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;
using UnityEngine.Audio;


public class MissionManager : MonoBehaviour
{

    public List<GameObject> _Locations = new List<GameObject>();
    public Dictionary<int, GameObject> _waitingLocations = new Dictionary<int, GameObject>();

    public GameObject goPlayer;

    private GameObject goCurrentLocation;

    public float timeByDistance = 0;
    public InputActionReference missionActionRef;

    public float missionTimer = 0f;

    bool isSearching = false;

    int nbMission = 0;
    public int limitRepetition = 3;




    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        missionActionRef.action.performed += CreateMission;
        missionActionRef.action.Enable();
    }

    private void OnDestroy()
    {
        //debranche
        missionActionRef.action.performed -= CreateMission;

    }

    // Update is called once per frame
    void Update()
    {
        if (isSearching)
        {
            missionTimer -= Time.deltaTime;
            Debug.Log(missionTimer);
            if (missionTimer <= 0f) isSearching = false;
        }
    }




    public void CreateMission(InputAction.CallbackContext callbackContext)
    {
        Debug.Log("nique ta mere");
        if (_Locations.Count == 0) return;
        else
        {
            Renderer tRenderer;
            if (goCurrentLocation != null) {
                 tRenderer = goCurrentLocation.GetComponent<Renderer>();

                // Use SetColor to set the main color shader property
                tRenderer.material.SetColor("_BaseColor", Color.white);
            }
            int rand = Random.Range(0, _Locations.Count);
            goCurrentLocation = _Locations[rand];
            float distance = Vector3.Distance(goPlayer.transform.position, goCurrentLocation.transform.position);
            Debug.Log(distance);
            missionTimer = distance * timeByDistance;
            isSearching = true;
            Debug.Log("---------------------------------------");
            _waitingLocations.Add(nbMission+limitRepetition, goCurrentLocation);
            _Locations.Remove(goCurrentLocation);
            if (nbMission >= limitRepetition)
            {
                _Locations.Add(_waitingLocations[nbMission]);
                _waitingLocations.Remove(nbMission);
            }

            // pour phase de test

            tRenderer = goCurrentLocation.GetComponent<Renderer>();

            // Use SetColor to set the main color shader property
            tRenderer.material.SetColor("_BaseColor", Color.red);

            nbMission++;

        }
    }


    public void endMission()
    {
        isSearching = false;
        Debug.Log("nique ta mere");
  
        
    }




}
