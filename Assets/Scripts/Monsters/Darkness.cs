﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

/*************Darkness Enemy Script**********
 * Base script for mini-darkness.  very basic movement and AI
 */
public class Darkness : MonoBehaviour {

    public Transform target;
    
    public int actionIdle, creationID, engIndex;
    public bool canAttack, attackRequested, updateStates;
    private float stateUpdateRate, attackInitiationRange;
    public Seeker sekr;
    public AIDestinationSetter aIDestSet;
    public GameObject deathFX;
    public Dark_State DeathState;
    public Animator animeController;
    public int attackHash = Animator.StringToHash("Attack"),
                attackAfterHash = Animator.StringToHash("AfterAttack"),
                chaseHash = Animator.StringToHash("Chase"),
                idleHash = Animator.StringToHash("Idle"),
                deathHash = Animator.StringToHash("Death");
    //public AI_Movement aIMovement;

    public Dark_State previousState, currentState;
    public RichAI aIRichPath;

    void Awake()
    {
        actionIdle = 3;
        attackInitiationRange = 3;
        canAttack = attackRequested = false;
        creationID = 0;
        engIndex = 500;
        updateStates = true;
        stateUpdateRate = 0.5f;
    }

    void Start () {
        AI_Manager.OnDarknessAdded(this);
        animeController = GetComponentInChildren<Animator>();
        //aIMovement = GetComponent<AI_Movement>();
        aIRichPath = GetComponent<RichAI>();
        sekr = GetComponent<Seeker>();
        aIDestSet = GetComponent<AIDestinationSetter>();
        currentState.InitializeState(this);
        StartCoroutine(ExecuteCurrentState());
        aIDestSet.target = target;
	}
	
	// Update is called once per frame
	void Update () {
        
    }

    public IEnumerator ExecuteCurrentState()
    {
        while(updateStates)
        {
            currentState.UpdateState(this);
            yield return new WaitForSeconds(stateUpdateRate);
        }
        yield return null;
    }

    public void ChangeState(Dark_State nextState)
    {
        if(currentState != nextState)
        {
            /*string memes = "Switching from current <color = red><b>" + currentState.stateType + "</b></color> state to next <color = green><b>" + nextState.stateType +  "</b></color> state";
            Debug.Log(memes);*/
            currentState = nextState;
            currentState.InitializeState(this);
            previousState = currentState;
        }
    }

    public bool TargetWithinDistance()
    {
        if(Vector3.Distance(transform.position, target.position) <= (float)attackInitiationRange && canAttack)
        {
            return true;
        }
        else return false;
    }

    private void OnCollisionEnter(Collision collision)
    {
      //EventManager.TriggerEvent("DarknessDeath", gameObject.);
    }

    private void OnTriggerEnter(Collider collider)
    {
        if (collider.gameObject.CompareTag("Projectile"))
        {
            if (collider.gameObject.GetComponent<Projectile_Shell>().projectileFired == true)
            {
                Debug.LogWarning("Darkness Destroyed");
                AI_Manager.OnDarknessRemoved(this);
                ChangeState(DeathState);
                //ChangeState(DeathState);
                //EventManager.TriggerEvent("DarknessDeath", gameObject.name);
            }
        }
        else if(collider.gameObject.CompareTag("Player"))
        {
            Debug.LogWarning("Darkness collided with Player");
        }
    }
}