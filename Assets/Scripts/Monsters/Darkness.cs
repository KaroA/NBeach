﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*************Darkness Enemy Script**********
 * Base script for mini-darkness.  very basic movement and AI
 */
 [RequireComponent(typeof(DarkStateController))]
public class Darkness : MonoBehaviour {

    public Transform target;
    public Pathfinding.AIDestinationSetter aIDestSetter;
    public Pathfinding.RichAI aIRichPath;
    public DarkStateController dsController;

    public int attackRange;

    public GameObject deathFX;



    // Use this for initialization
    void Start () {
        aIDestSetter = GetComponent<Pathfinding.AIDestinationSetter>();
        aIRichPath = GetComponent<Pathfinding.RichAI>();
        dsController = GetComponent<DarkStateController>();
        dsController.ChangeState(EnemyState.IDLE, this);
        aIDestSetter.target = target;
	}
	
	// Update is called once per frame
	void Update () {
        dsController.ExecuteCurrentState();
    }

    private void OnCollisionEnter(Collision collision)
    {
      //EventManager.TriggerEvent("DarknessDeath", gameObject.);
    }

    private void OnTriggerEnter(Collider collider)
    {
        if(collider.gameObject.tag == "Player")
        {
            Debug.Log("Darkness collided with Player");
        }
        else if (collider.gameObject.tag == "Projectile")
        {
            if (collider.gameObject.GetComponent<Projectile_Shell>().projectileFired == true)
            {
                Debug.Log("Darkness Destroyed");

                //AI_Manager.Instance.AddtoDarknessList(this);

                AI_Manager.Instance.AddtoDarknessList(this);
                GameObject newFX = Instantiate(deathFX.gameObject, transform.position, Quaternion.identity) as GameObject;
                //gameObject.GetComponent<MeshRenderer>().material.SetColor(Color.white);
                
                //change darkness back to idle to state to prevent moving & set to Kinematic to prevent any Physics effects
                dsController.ChangeState(EnemyState.IDLE, this);
                gameObject.GetComponentInChildren<Rigidbody>().isKinematic = true;
                StartCoroutine(deathRoutine());

                //EventManager.TriggerEvent("DarknessDeath", gameObject.name);
            }
        }
    }

    IEnumerator deathRoutine()
    {
        float fxTime = 1;
        //Slowly increase texture power over the FX lifetime to show the Darkness "Glowing" and explode!
        int maxPower = 10;
        MeshRenderer renderer = gameObject.GetComponentInChildren<MeshRenderer>();
        float curPower = renderer.material.GetFloat("_MainTexturePower");
        float curTime = 0;
        while(curTime < fxTime)
        {
            curPower = curTime * maxPower;
            renderer.material.SetFloat("_MainTexturePower", curPower);
            curTime += Time.deltaTime;
            yield return 0;
        }
       
        //yield return new WaitForSeconds(fxTime);
        Destroy(this.gameObject);
        yield return 0;
    }

}
