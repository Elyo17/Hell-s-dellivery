using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class ColisManager : MonoBehaviour
{
    
    public int Life = 100;
    public int Index;
    private int max;
    private MeshRenderer meshRenderer;
    public Material[] _Materials;

    public AnimationCurve curveDMG;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        max = _Materials.Length;
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
        meshRenderer.material = _Materials[Index];
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player")) return;
        float forceChoc = collision.impulse.magnitude;
        float maxDMG = 17f;
        float ratio = forceChoc / maxDMG;
        if (ratio > 1) Degat(101);
        float dmg = curveDMG.Evaluate(ratio); 
        
        //Debug.Log("Le colis a subi un choc de " + dmg);
        if(forceChoc>=4)Degat((int)(100f * dmg));
    }


    public int getMax(){ return max; }
}
