using System.Collections.Generic;
using Unity.VisualScripting;
using Unity.VisualScripting.Dependencies.Sqlite;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;
using UnityEngine.Audio;
using System.Security.Cryptography;
using UnityEditor.VersionControl;


public class MissionManager : MonoBehaviour
{

    public List<GameObject> _Colis = new List<GameObject>();
    public List<GameObject> _Locations = new List<GameObject>();
    public Dictionary<int, GameObject> _waitingLocations = new Dictionary<int, GameObject>();

    public GameObject goPlayer;

    private GameObject goCurrentLocation;
    public GameObject goCurrentColis  = null;

    public float timeByDistance = 0;
    public InputActionReference missionActionRef;

    public float missionTimer = 0f;
    public float missionTotalTimer = 0f;


    bool isSearching = false;

    int nbMission = 0;
    public int limitRepetition = 3;

    public AnimationCurve curveColis;

    public float scoreTotal = 0;

    // test
    public InputActionReference changeActionRef;
    // fin test


    public GameObject goScore;
    public GameObject goTime;

    private TMPro.TextMeshProUGUI _tScore;
    private TMPro.TextMeshProUGUI _tTime;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        missionActionRef.action.performed += CreateMission;
        missionActionRef.action.Enable();
        updateColisList(null);
        // test 
        changeActionRef.action.performed += Degat;
        changeActionRef.action.Enable();
        // fin test
        _tScore = goScore.GetComponent<TMPro.TextMeshProUGUI>();
        _tTime = goTime.GetComponent<TMPro.TextMeshProUGUI>();

    }

    private void OnDestroy()
    {
        missionActionRef.action.performed -= CreateMission;
        //test
        changeActionRef.action.performed -= Degat;
        //fin test
    }

    // Update is called once per frame
    void Update()
    {
        if (isSearching)
        {
            missionTimer -= Time.deltaTime;
            _tTime.text = (missionTimer.ToString("F2")) + " s";
            //Debug.Log(missionTimer);
            if (missionTimer <= 0f)
            {
                endMission(null, null);
            }
        }
    }




    public void CreateMission(InputAction.CallbackContext callbackContext)
    {
        if (_Locations.Count == 0 || isSearching || _Colis.Count == 0) return;
        else
        {
            int rand = Random.Range(0, _Locations.Count);
            goCurrentLocation = _Locations[rand];
            float distance = Vector3.Distance(goPlayer.transform.position, goCurrentLocation.transform.position);
            // gestion du timer
            missionTimer = distance * timeByDistance;
            missionTotalTimer = missionTimer;

            isSearching = true;
            getNewColis();
            // gestion des locations
            _waitingLocations.Add(nbMission+limitRepetition, goCurrentLocation);
            _Locations.Remove(goCurrentLocation);
            if (nbMission >= limitRepetition)
            {
                _Locations.Add(_waitingLocations[nbMission]);
                _waitingLocations.Remove(nbMission);
            }
            // test
            goCurrentLocation.GetComponentInChildren<locat>().SetOutline(true);
            // fin test
            nbMission++;
        }
    }


    public void endMission(GameObject colis, GameObject loc)
    {
        if ((isSearching) && (colis == goCurrentColis) && (loc == goCurrentLocation))
        {
            Debug.Log("ok");
            isSearching = false;
            // multiplicateur de l'etat du colis
            float ratio = (float)(goCurrentColis.GetComponent<ColisManager>().Index) / (float)(goCurrentColis.GetComponent<ColisManager>().getMax());
            // base de score par rapport a la durée
            float score;
            float pourcent = (missionTimer / missionTotalTimer) * 100;
            if (pourcent > 70) score = 1000;
            else if (pourcent > 30) score = 650;
            else if (pourcent > 0.1) score = 350;
            else score = 0;
            // calcule le score
            score *= curveColis.Evaluate(ratio);           
            scoreTotal += score;
            _tScore.text = scoreTotal.ToString("F0");
            // retire le colis
            _Colis.Remove(goCurrentColis);
            goCurrentColis.GetComponent<ColisManager>().MissionOutline(false);
            Destroy(goCurrentColis);
            goCurrentColis = null;
            goCurrentLocation.GetComponentInChildren<locat>().SetOutline(false);
            _tTime.text = "";


        }
        else if ((isSearching) && colis == null)
        {
            isSearching = false;
            goCurrentColis.GetComponent<ColisManager>().MissionOutline(false);
            goCurrentColis = null;
            goCurrentLocation.GetComponentInChildren<locat>().SetOutline(false);
            _tTime.text = "";

        }
    }

    public void updateColisList(GameObject colis)
    {
        if( colis != null)
        {
            if (colis == goCurrentColis) { endMission(null, null); }
            _Colis.Remove(colis);
            Destroy(colis);
        }
        StartCoroutine(UpdateColisListCoroutine());

    }

    private IEnumerator UpdateColisListCoroutine()
    {
        yield return null; // Attend la fin de la frame

        _Colis = new List<GameObject>(GameObject.FindGameObjectsWithTag("Colis"));
    }
    public void getNewColis()
    {
         int rand = Random.Range(0, _Colis.Count);
         goCurrentColis = _Colis[rand];
         goCurrentColis.GetComponent<ColisManager>().MissionOutline(true);
         goPlayer.GetComponent<PlayerMove>().setColisMission(_Colis[rand]);

    }


    //test degat
    public void Degat(InputAction.CallbackContext callbackContext)
    {
        if (goCurrentColis != null)
        {
            goCurrentColis.GetComponent<ColisManager>().Degat(10);
        }
    }
    //fin test


}
