using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.UIElements;
using static UnityEngine.LowLevelPhysics2D.PhysicsShape;

public class ColisManager : MonoBehaviour
{
    
    public int Life = 100;
    public int Index;
    private int max;
    private MeshRenderer meshRenderer;
    public Material[] _Materials;
    public Material outline;
    public Material outlineMission;

    public AnimationCurve curveDMG;
    public bool isChosen = false;
    public GameObject particlePrefab;

    public float minSize = 0.2f;
    public float maxSize = 1.7f;
    public float maxDMG = 17f;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        max = _Materials.Length;
        Material[] mats = meshRenderer.sharedMaterials;
        outline = mats[1];
        outlineMission = mats[2];

        mats[1] = null;
        mats[2] = null;

        meshRenderer.sharedMaterials = mats;

    }

    public void Degat(int dmg)
    {
        Life -= dmg;
        if (Life <= 0 )
        {
            GameObject.FindWithTag("missionManager").GetComponent<MissionManager>().updateColisList(gameObject);            
            return;
        }
        Index = ((100 - Life) * max) / 100;
        Material[] mats = meshRenderer.sharedMaterials;
        mats[0] = _Materials[Index];
        meshRenderer.sharedMaterials = mats;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player")) return;
        float forceChoc = collision.impulse.magnitude;
        float ratio = forceChoc / maxDMG;
        //
        ContactPoint contact = collision.contacts[0];

        float size = Mathf.Lerp(
            minSize,
            maxSize,
            Mathf.Clamp01(forceChoc / maxDMG)
        );

        GameObject particles = Instantiate(
        particlePrefab,
            contact.point,
            Quaternion.LookRotation(contact.normal)
        );

        // Modifie la taille globale du système
        particles.transform.localScale = Vector3.one * size;



        //
        if (ratio > 1) Degat(101);
        float dmg = curveDMG.Evaluate(ratio); 
        
        //Debug.Log("Le colis a subi un choc de " + dmg);
        if(forceChoc>=4)Degat((int)(100f * dmg));
    }


    public int getMax(){ return max; }
    public void SetOutline(bool active, bool isPickedup)
    {      
        Material[] mats = meshRenderer.sharedMaterials;
        if (!isPickedup)
        {
            Debug.Log("afficher");
            if (!isChosen) mats[1] = active ? outline : null;
            else mats[1] = active ? outline : outlineMission;
            gameObject.transform.GetChild(0).gameObject.SetActive(true);
        }
        else
        {
            mats[1] = null;
            Debug.Log("pick");
            gameObject.transform.GetChild(0).gameObject.SetActive(false);


        }
        meshRenderer.sharedMaterials = mats;
           
    }
    public void MissionOutline(bool active)
    {
        isChosen = active;
        SetOutline(false, false);
    }

    public void enterCar() { }

   

}
