using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class locat : MonoBehaviour
{
    private GameObject missionManager;

    private MeshRenderer meshRenderer;
    public Material outline;


    void Start()
    {
        missionManager = GameObject.FindWithTag("missionManager");
        meshRenderer = GetComponent<MeshRenderer>();
        Material[] mats = meshRenderer.sharedMaterials;
        outline = mats[1];
        mats[1] = null;
        meshRenderer.sharedMaterials = mats;
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
            missionManager.GetComponent<MissionManager>().endMission(colis, transform.parent.gameObject);
    }


    public void SetOutline(bool active)
    {
        Material[] mats = meshRenderer.sharedMaterials;
        mats[1] = active ? outline : null;
        meshRenderer.sharedMaterials = mats;
    }
}
