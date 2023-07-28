using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyMovement : MonoBehaviour
{
	[Header("Sensors")]
	public float TouchDistance;
	//How far this enemy can detect something without seeing or hearing them//
	public float VisionDistance;
	//How far this enemy can see//
	public float FieldOfView;
	//Field of view, represents the angle this enemy can see a player//
	public float Audition;
	//How strong a sound has to be so this enemy can detect it//
	public float SmellDistance;
	//How far this enemy can smell//
	public float SmellFactor;
	//How strong a smell has to be so this enemy can detect it//

    [Header("Inteligence")]
    public bool AIActive;
	public enum EnemyState
	{
		Spawning, Idle, Alert, Pursuing, Attacking, Flinching, Falling
	}
	public EnemyState EnemyStatus;
	private int CurrentPlayerTarget;
	//Indicates the number of the player this enemy is currently targetting//
	public float ForgetTime;
	//Time it takes for an enemy to forget about what it was chasing//
	private float CurrentForgetTime;
    //The current time it will take for an enemy to forget about what it was chasing//
	public bool Jumper;
	//Determines if this enemy can jump//
	public float JumpDistance;
	//How far this enemy can jump//
	private float CurrentActionDelay;
	//The enemy can't attack, search or pursue players while this delay is active//
	public float KnockbackDelay;
    //The enemy can't move while this delay is active, but can still attack and search//
    public float FallDelay;
	//Delay for falling off cliffs. The enemy can't attack, search or pursue players while this delay is active//
	private float CurrentKnockbackDelay;
	//The enemy can't move while this delay is active, but can still attack and search//

	[Header("Animations")]
	public string TPoseAnimationName = "TPose";
	public string [] StandingAnimationNameArray = { "InactiveStanding" };
	public string [] SpecialStandingAnimationNameArray = { "SpecialStanding" };
	public string [] AttackAnimationNameArray = { "Attack1", "Attack2" };
	public string [] WalkAnimationNameArray = { "Walk1", "Walk2" };
	public string [] MeleeHitAnimationNameArray = { "MeleeHit1" };
	public string [] ShotHitAnimationNameArray = { "ShotHit1", "ShotHit2", "ShotHit3" };
	public string [] SpawnAnimationNameArray = { "ComingOutGround" };
	public string [] FallingAnimationNameArray = { "Falling" };
	private string CurrentAnimationName =  "TPose";
	private int RandomStandingAnimationNum = 0;
	private int RandomSpecialStandingAnimationNum = 0;
	private int RandomAttackAnimationNum = 0;
	private int RandomWalkAnimationNum = 0;
	private int RandomMeleeHitAnimationNum = 0;
	private int RandomShotHitAnimationNum = 0;
	private int RandomSpawnAnimationNum = 0;
	private int RandomFallingAnimationNum = 0;
	[HideInInspector]
	public Animator AnimatorController;

	[Header("Debugging")]
	public bool PrintPlayerDetection = false;

	[Header("Main References")]
	[Space(50)]
	public Transform AttackTransform;
	private Collider MainCollider;
	public NavMeshAgent Agent;
	public EnemyStats Enemy;

	private GameManager GameManagerScript;
	public PlayerManager PlayerManagerScript;
	private List<Transform> Player = new List<Transform>();
	[HideInInspector]
	public List<PlayerStats> TargetStats = new List<PlayerStats>();
	private int PlayerAmount;

	void Awake ()
	{
        GameManagerScript = FindObjectOfType<GameManager>();
		PlayerManagerScript = GameManagerScript.GetComponent<PlayerManager> ();
		Enemy = GetComponent<EnemyStats> ();
		Agent = GetComponent<NavMeshAgent> ();
		AnimatorController = GetComponentInChildren<Animator>();
	}

    void Start()
    {
		FindPlayers();

		RandomStandingAnimationNum = Random.Range(0, StandingAnimationNameArray.Length);
		RandomSpecialStandingAnimationNum = Random.Range(0, SpecialStandingAnimationNameArray.Length);
		RandomAttackAnimationNum = Random.Range(0, AttackAnimationNameArray.Length);
		RandomWalkAnimationNum = Random.Range(0, WalkAnimationNameArray.Length);
		RandomMeleeHitAnimationNum = Random.Range(0, MeleeHitAnimationNameArray.Length);
		RandomShotHitAnimationNum = Random.Range(0, ShotHitAnimationNameArray.Length);
		RandomSpawnAnimationNum = Random.Range(0, SpawnAnimationNameArray.Length);
		RandomFallingAnimationNum = Random.Range(0, FallingAnimationNameArray.Length);

		CurrentPlayerTarget = -1;
		CurrentForgetTime = ForgetTime;
		CurrentActionDelay = Enemy.SpawnDelay;
		CurrentAnimationName = TPoseAnimationName;
		EnemyStatus = EnemyState.Spawning;

		if (AttackTransform == null)
        {
			AttackTransform = transform;

			if (MainCollider != null)
			{
				Vector3 AttackPosition = new Vector3(transform.position.x, transform.position.y + MainCollider.bounds.size.y / 2f, transform.position.z);
				AttackTransform.position = AttackPosition;
			}
		}
	}

	public void FindPlayers ()
    {
		Player.Clear();
		TargetStats.Clear();

		PlayerAmount = PlayerManagerScript.PlayerList.Count;

		for (int i = 0; i < PlayerAmount; i++)
		{
			if (PlayerManagerScript.PlayerList [i] != null)
			{
				Player.Insert(i, PlayerManagerScript.PlayerList [i]);
				TargetStats.Insert(i, Player [i].GetComponent<PlayerStats>());
			}
		}

		Enemy.TargetStats = TargetStats.ToArray();
	}

	void Update ()
	{
        if (GameManagerScript.GameStatus != GameManager.GameState.InGame)
        {
            return;
        }

		if (Enemy.Dead)
        {
			if (AnimatorController != null)
			{
				AnimatorController.enabled = false;
			}

			return;
		}

		if (Agent.isOnOffMeshLink)
        {
			EnemyStatus = EnemyState.Falling;
			Enemy.Stunned = true;
			CurrentActionDelay = FallDelay;
		}

		if (CurrentKnockbackDelay <= 0f)
        {
			Agent.isStopped = false;
			Enemy.MainRigidbody.isKinematic = true;
			Enemy.MainRigidbody.freezeRotation = false;
		}
		else
        {
			CurrentKnockbackDelay -= Time.deltaTime;
		}

        if (!AIActive)
		{
			return;
		}

		if (CurrentActionDelay <= 0f)
		{
			Enemy.Stunned = false;

			Search();

			if (CurrentPlayerTarget >= 0)
			{
				if (CurrentForgetTime > 0f)
				{
					CurrentForgetTime -= Time.deltaTime;
					ChaseTarget(CurrentPlayerTarget);
				}
			}
			else
			{
				EnemyStatus = EnemyState.Idle;
			}

			CheckFront();
		}
        else
        {
			CurrentActionDelay -= Time.deltaTime;
        }

		if (AnimatorController != null)
		{
			switch (EnemyStatus)
			{
				case EnemyState.Spawning:
					if (CurrentAnimationName != "SpawnAnimationNameArray")
					{
						CurrentAnimationName = "SpawnAnimationNameArray";
						AnimatorController.CrossFadeInFixedTime(SpawnAnimationNameArray [RandomSpawnAnimationNum], 0f);
					}
				break;

				case EnemyState.Idle:
					if (CurrentAnimationName != "StandingAnimationNameArray")
					{
						CurrentAnimationName = "StandingAnimationNameArray";
						AnimatorController.SetTrigger("Idle");
					}
				break;

				case EnemyState.Alert:
					if (CurrentAnimationName != "SpecialStandingAnimationNameArray")
					{
						CurrentAnimationName = "SpecialStandingAnimationNameArray";
						AnimatorController.SetTrigger("Alert");
					}
				break;

				case EnemyState.Pursuing:
					if (CurrentAnimationName != "WalkAnimationNameArray")
					{
						CurrentAnimationName = "WalkAnimationNameArray";
						AnimatorController.SetInteger("PursueIndex", RandomWalkAnimationNum);
						AnimatorController.SetTrigger("Pursue");
					}
				break;

				case EnemyState.Attacking:
					if (CurrentAnimationName != "AttackAnimationNameArray")
					{
						CurrentAnimationName = "AttackAnimationNameArray";
						AnimatorController.SetInteger("AttackIndex", RandomAttackAnimationNum);
						AnimatorController.SetTrigger("Attack");
						RandomAttackAnimationNum = Random.Range(0, AttackAnimationNameArray.Length);
					}
					break;

				case EnemyState.Flinching:
					if (CurrentAnimationName != "MeleeHitAnimationNameArray")
					{
						CurrentAnimationName = "MeleeHitAnimationNameArray";
						AnimatorController.SetInteger("MeleeHitIndex", RandomMeleeHitAnimationNum);
						AnimatorController.SetTrigger("MeleeHit");
						RandomMeleeHitAnimationNum = Random.Range(0, MeleeHitAnimationNameArray.Length);
					}
				break;

				case EnemyState.Falling:
					if (CurrentAnimationName != "FallingAnimationNameArray")
					{
						CurrentAnimationName = "FallingAnimationNameArray";
						AnimatorController.SetInteger("FallingIndex", RandomFallingAnimationNum);
						AnimatorController.SetTrigger("Falling");
						RandomFallingAnimationNum = Random.Range(0, FallingAnimationNameArray.Length);
					}
				break;

				default:
					if (CurrentAnimationName != TPoseAnimationName)
					{
						CurrentAnimationName = TPoseAnimationName;
						AnimatorController.CrossFadeInFixedTime(TPoseAnimationName, 0f);
					}
				break;
			}
		}
	}

	bool Touch (int TargetNum, float TargetDistance)
	{
		if (TargetDistance <= TouchDistance)
		{
			return true;
		}
		else
		{
			return false;
		}
	}

	bool See (int TargetNum, float TargetDistance)
	{
		Vector3 TargetDirection = Player [TargetNum].position - transform.position;

		float Angle = Vector3.Angle (TargetDirection, transform.forward);
		float MaxAngle = FieldOfView / 2f;
		float MinAngle = 360 - (FieldOfView / 2f);

		bool Blocked = Physics.Raycast (transform.position, TargetDirection, TargetDistance - 1f);

		if ((Angle >= MinAngle || Angle <= MaxAngle) && TargetDistance <= VisionDistance && !Blocked)
		{
			return true;
		}
		else
		{
			return false;
		}
	}

	bool Hear (int TargetNum, float TargetDistance)
	{
		float Noise = TargetStats [TargetNum].CurrentNoise / TargetDistance;

		if (Noise >= Audition)
		{
			return true;
		}
		else
		{
			if (CurrentPlayerTarget < 0 && Noise > Audition / 2)
            {
				EnemyStatus = EnemyState.Alert;
			}

			return false;
		}
	}

	bool Smell (int TargetNum, float TargetDistance)
	{
		if (TargetStats [TargetNum].CurrentSmelliness >= SmellFactor && TargetDistance <= SmellDistance)
		{
			return true;
		}
		else
		{
			if (CurrentPlayerTarget < 0 && TargetStats [TargetNum].CurrentSmelliness >= SmellFactor / 2f && TargetDistance <= SmellDistance)
			{
				EnemyStatus = EnemyState.Alert;
			}

			return false;
		}
	}

	void Search ()
	{
		for (int TargetNum = 0; TargetNum < PlayerAmount; TargetNum++)
		{
			if (TargetStats [TargetNum] != null && !TargetStats[TargetNum].Dead)
			{
				float DistanceFromPlayer = Vector3.Distance (Player [TargetNum].position, transform.position);

				if (PrintPlayerDetection)
                {
					Debug.Log(gameObject.name + " can hear " + TargetStats [TargetNum].name + ":" + Hear(TargetNum, DistanceFromPlayer));
					Debug.Log(gameObject.name + " can see " + TargetStats [TargetNum].name + ":" + See(TargetNum, DistanceFromPlayer));
					Debug.Log(gameObject.name + " can smell " + TargetStats [TargetNum].name + ":" + Smell(TargetNum, DistanceFromPlayer));
					Debug.Log(gameObject.name + " can touch " + TargetStats [TargetNum].name + ":" + Touch(TargetNum, DistanceFromPlayer));
				}

				if (Hear(TargetNum, DistanceFromPlayer) || See(TargetNum, DistanceFromPlayer) || Touch(TargetNum, DistanceFromPlayer) || Smell(TargetNum, DistanceFromPlayer))
				{
					if (CurrentPlayerTarget < 0)
					{
                        CurrentForgetTime = ForgetTime;
                        CurrentPlayerTarget = TargetNum;
					}
                    else if (CurrentPlayerTarget == TargetNum)
                    {
                        CurrentForgetTime = ForgetTime;
                    }
				}
				else if (CurrentForgetTime <= 0)
                {
					ForgetTarget();
				}
			}
		}
	}

	void ChaseTarget (int TargetNum)
	{
        if (!TargetStats[TargetNum].Dead)
        {
            float DistanceFromPlayer = Vector3.Distance(Player [TargetNum].position, transform.position);
            Agent.SetDestination(Player [TargetNum].position);

			if (Agent.hasPath && !Agent.isPathStale)
			{
				Agent.isStopped = false;
				EnemyStatus = EnemyState.Pursuing;

				if (DistanceFromPlayer < Enemy.AttackReach + 0.1f)
				{
					FaceTarget(TargetNum);
				}
			}
			else
            {
				ForgetTarget();
			}
		}
	}

	void FaceTarget (int TargetNum)
	{
        if (!TargetStats [TargetNum].Dead)
        {
			Vector3 Direction = (Player [TargetNum].position - transform.position).normalized;
            Quaternion LookRotation = Quaternion.LookRotation(new Vector3(Direction.x, 0f, Direction.z));
            transform.rotation = Quaternion.Slerp(transform.rotation, LookRotation, Time.deltaTime * 5f);
		}
	}

	void CheckFront ()
    {
		RaycastHit [] ObjectsInFront = Physics.RaycastAll(AttackTransform.position, transform.forward, Enemy.AttackReach, Enemy.AttackingMask);

		foreach (RaycastHit Object in ObjectsInFront)
		{
			PlayerStats Player = Object.transform.GetComponentInParent<PlayerStats>();
			Destructible DestructibleObject = Object.transform.GetComponentInParent<Destructible>();

			if (Player != null || DestructibleObject != null)
			{
				float ObjectHeight = Object.transform.position.y;

				EnemyStatus = EnemyState.Attacking;
				Enemy.AttackStart(ObjectHeight);

				CurrentActionDelay = Enemy.AttackDelay;
			}
		}
	}

	public void Flinch ()
    {
		Agent.isStopped = true;
		CurrentActionDelay = Enemy.FlinchDelay;
		EnemyStatus = EnemyState.Flinching;
		Enemy.Stunned = true;

		CurrentAnimationName = "MeleeHitAnimationNameArray";
		AnimatorController.SetInteger("MeleeHitIndex", RandomMeleeHitAnimationNum);
		AnimatorController.SetTrigger("MeleeHit");
		RandomMeleeHitAnimationNum = Random.Range(0, MeleeHitAnimationNameArray.Length);
	}

	void ForgetTarget ()
    {
		Agent.isStopped = true;
		CurrentPlayerTarget = -1;
		EnemyStatus = EnemyState.Idle;
	}

    void OnTriggerEnter (Collider CurrentCollider)
    {
		print("colliding");

		Agent.isStopped = true;
		Enemy.MainRigidbody.isKinematic = false;
		Enemy.MainRigidbody.freezeRotation = true;

		CurrentKnockbackDelay = KnockbackDelay;
	}

    void OnDrawGizmosSelected ()
	{
		Gizmos.color = Color.white;
		Gizmos.DrawWireSphere (transform.position, TouchDistance);

		Gizmos.color = Color.blue;
		Gizmos.DrawWireSphere (transform.position, VisionDistance);

		Gizmos.color = Color.green;
		Gizmos.DrawWireSphere (transform.position, SmellDistance);
    }
}
