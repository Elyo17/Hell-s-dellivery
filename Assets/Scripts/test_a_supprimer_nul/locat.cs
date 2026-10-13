using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class locat : MonoBehaviour
{
    private GameObject missionManager;

    void Start()
    {
        missionManager = GameObject.FindWithTag("missionManager");
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Colis"))
            StartCoroutine(WaitBeforeEndMission(other.gameObject));
    }

    private IEnumerator WaitBeforeEndMission(GameObject colis)
    {
        yield return new WaitForSeconds(2f);

        if (colis != null)
            missionManager.GetComponent<MissionManager>().endMission(colis, transform.root.gameObject);
    }
}
