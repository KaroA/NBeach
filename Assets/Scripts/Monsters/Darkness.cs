﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

/*************Darkness Enemy Script**********
 * Base script for mini-darkness.  very basic movement and AI
 */
public class Darkness : MonoBehaviour {

    public enum AggresionRating {Attacking = 1, CatchingUp, Idling, Wandering}

    [HideInInspector]
    public AggresionRating agRatingCurrent, agRatingPrevious;
    public Dark_State previousState, currentState, queuedState;

    [HideInInspector]
    public Collider darkHitBox;
    public GameObject deathFX;
    public Dark_State DeathState;
    
    public bool moving, updateStates, attacked, timedState, timedStateExiting;
    public int creationID;
    public float playerDist, targetDistance, swtichDist, stopDistance, pathUpdateTime;

    public Vector3 playerDirection;
    private Vector3 prevPos;
    private bool reachedEndOfPath, wandering, lookAtPlayer;
    
    public NavigationTarget navTarget;

    private Seeker sekr;
    private AIPath aIPath;
    private Path navPath;
    public Rigidbody rigidbod;
    public Vector3 nextPosition;
    public Quaternion nextRotation;

    private Animator animeController;

    [HideInInspector]
    public int attackHash = Animator.StringToHash("Attack"),
                chaseHash = Animator.StringToHash("Chase"),
                idleHash = Animator.StringToHash("Idle"),
                deathHash = Animator.StringToHash("Death");

    public enum NavTargetTag {Attack, Patrol}

    ///<summary>NavigationTarget is used by Darkness for pathfinding purposes. </summary>
	public struct NavigationTarget
	{
		public int weight;
		//public bool active;
		private float groundElavation;

		public Vector3 position;
		private Vector3 positionOffset;
		//public Transform locationInfo { 
		//	get {return transform; }}

		private NavTargetTag targetTag;
		public NavTargetTag navTargetTag { get{ return targetTag; }}

		///<param name="iD">Used in AI_Manager to keep track of the Attack points. Arbitrary for the Patrol points.</param>
		///<parem name="offset">Only used on targets that will be used for attacking. If non-attack point set to Vector3.Zero</param>
		public NavigationTarget(Vector3 loc, Vector3 offset, float elavation, NavTargetTag ntTag)//, bool act)
		{
			position = loc;
			groundElavation = elavation;
			//if(parent != null)
			//	transform.parent = parent;
			positionOffset = offset;
			targetTag = ntTag;
			weight = 0;
			//active = false;
			//assignedDarknessIDs = new int[assignmentLimit];
		}

		public void UpdateLocation(Vector3 loc)
		{
			//if(!applyOffset)
			//	position = new Vector3(loc.x, groundElavation, loc.y);
			//else 
			position = new Vector3(loc.x, groundElavation, loc.y) + positionOffset;
		}
	}

    void Awake()
    {
        //attackInitiationRange = 2.5f;
        stopDistance = 3;
        swtichDist = 3; 
        creationID = 0;
        pathUpdateTime = 1.6f;
        updateStates = true;
        agRatingCurrent = agRatingPrevious = AggresionRating.Idling;
        attacked = moving = wandering = reachedEndOfPath = lookAtPlayer = false;
        animeController = GetComponentInChildren<Animator>();
        darkHitBox = GetComponent<CapsuleCollider>();
        sekr = GetComponent<Seeker>();
        aIPath = GetComponent<AIPath>();
        rigidbod = gameObject.GetComponentInChildren<Rigidbody>();
    }

    void Start () {
        //patrolNavTarget = new Darkness_Manager.NavigationTarget(this.transform.position, Vector3.zero, Darkness_Manager.Instance.ground, 10, Darkness_Manager.NavTargetTag.Patrol);
        Darkness_Manager.OnDarknessAdded(this); //Called in Start instead of at instantiation becuase the Darkness needs to be fully setup before the Manager approves behavior changes.
        Darkness_Manager.UpdateDarkStates += UpdateCurrentState;
        Darkness_Manager.DistanceUpdate += UpdateDistanceEvaluation;
        currentState.InitializeState(this);
        sekr.pathCallback += PathComplete;
        darkHitBox.enabled = false;
        playerDirection = new Vector3();
        //patrolNavTarget.UpdateLocation(Vector3.zero, false);
	}

    public void ChangeState(Dark_State nextState)
    {
        /*previousState = currentState;
        if(!timedState) //if the timer on a state is still active don't switch yet
        {
            if(queuedState != null && queuedState != nextState)
            {
                if(nextState.statePriority > queuedState.statePriority)
                {
                    currentState = nextState;
                    previousState.ExitState(this);
                    currentState.InitializeState(this);
                }
                else if(queuedState.statePriority > nextState.statePriority)
                {
                    currentState = queuedState;
                    previousState.ExitState(this);
                    currentState.InitializeState(this);
                }
            }
            else currentState = nextState;
        }
        else
        {
            if(!timedStateExiting)
            {
                StartCoroutine(StateTransitionTimer(currentState.exitTime));
                timedState = false;
            }            
        }  //Check to see if this state has initiated it's timer to exit bevavior*/
    }


    private void UpdateCurrentState()
    {
        currentState.UpdateState(this);
    }

    void FixedUpdate()
    {
        if(moving && navPath != null)
        {
            aIPath.MovementUpdate(Time.deltaTime, out nextPosition, out nextRotation);

            aIPath.FinalizeMovement(nextPosition, nextRotation);
        }
    }   

    ///<summary>Called in state update loop to update path</summary>
    public void UpdatePath()
    {
        if(sekr.IsDone())
        {
            //if(navTarget.active)
            CreatePath(navTarget.position);
            //else if(patrolNavTarget.active)
            //    CreatePath(patrolNavTarget.position);
        }    
        //yield return new WaitForSeconds(pathUpdateTime);
    }

    private void PathComplete(Path p)
    {
        Debug.LogWarning("path callback complete");
        p.Claim(this);
        //BlockPathNodes(p);
        if(!p.error)
        {
            if(navPath != null) 
                navPath.Release(this);
            navPath = p;
        }
        else 
        {
            p.Release(this);
            Debug.LogError("Path failed calculation for " + this + " because " + p.errorLog);
        }
    }

    /*private void BlockPathNodes(Path p)
    {
        foreach(GraphNode n in p.path)
        {
            bProvider.blockedNodes.Add(n);
        }
        //Debug.Break();
    }*/

    private void CreatePath(Vector3 endPoint)
    {
        //bProvider.blockedNodes.Clear();
        Path p = ABPath.Construct(transform.position, endPoint);
        //p.traversalProvider = bProvider;
        sekr.StartPath(p, null);
        p.BlockUntilCalculated();
    }

    public void EndMovement()
    {
        if(!sekr.IsDone())
            sekr.CancelCurrentPathRequest();
        moving = false;
        sekr.pathCallback -= PathComplete;
    }

    public void UpdateAnimator(Dark_State.StateType stType) 
    {
        //Ensure the correct animation is played in the animator. 
        switch(stType)
        {
            case Dark_State.StateType.ATTACK:
                animeController.SetTrigger(attackHash);
                break;
            case Dark_State.StateType.CHASING:
                animeController.SetTrigger(chaseHash);
                break;
            case Dark_State.StateType.IDLE:
                animeController.SetTrigger(idleHash);
                break;
            case Dark_State.StateType.DEATH:
                animeController.SetTrigger(deathHash);
                break;
        }
    }

    public IEnumerator AttackCooldown(float idleTime)
    {
        darkHitBox.enabled = true;
        //animeController.SetTrigger(animationID);
        yield return new WaitForSeconds(idleTime);
        attacked = false;
        darkHitBox.enabled = false;
    }

    ///<summary>Called once per tick by the Darkness_Manager. Updates the playerDist, attackNavTarget/patrolNavTarget targetDistance values. If either NavTarget is inactive the distance is set to -1</summary>
    public void UpdateDistanceEvaluation(Vector3 playerLocation)
    {
        playerDist = Vector3.Distance(transform.position, playerLocation);
        playerDirection = playerLocation - transform.position;
        /*if(attackNavTarget.active)
        {
            attackNavTarget.targetDistance = Vector3.Distance(transform.position, attackNavTarget.position);
        }
        else attackNavTarget.targetDistance = -1;

        if(patrolNavTarget.active)
        {
            patrolNavTarget.targetDistance = Vector3.Distance(transform.position, patrolNavTarget.position);
        }
        else patrolNavTarget.targetDistance = -1;*/
    }

    public void AggressionChanged(AggresionRating agR)
    {
        if(agR != agRatingCurrent)
            agRatingPrevious = agRatingCurrent;
		agRatingCurrent = agR;
    }

    public IEnumerator WaitTimer(float timer)
	{
		yield return new WaitForSeconds(timer);
	}

    private IEnumerator StateTransitionTimer(float timer)
    {
        timedStateExiting = true;
        yield return new WaitForSeconds(timer);
        timedStateExiting = false;
    }

    private void OnTriggerEnter(Collider collider)
    {
        if (collider.gameObject.CompareTag("Projectile"))
        {
            if (collider.gameObject.GetComponent<Projectile_Shell>().projectileFired == true)
            {
                Debug.LogWarning("Darkness Destroyed");
                ChangeState(DeathState);
            }
        }
        else if(collider.gameObject.CompareTag("Player"))
        {
            //Debug.LogWarning("Darkness collided with Player");
        }
    }
}