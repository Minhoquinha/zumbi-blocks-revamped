using System.Collections;
using UnityEngine;

public class EnemyStats : MonoBehaviour
{
    [Header("Combat Stats")]
    public float Health;
    [HideInInspector]
    public float CurrentHealth;
    public float Armor;
    public float Speed;
    private float CurrentSpeed;
    public float AngularSpeed;
    private float CurrentAngularSpeed;
    public float Acceleration = 150f;
    private float CurrentAcceleration;
    public float SlowResistance; //Affects how hard it is for an enemy to be stunned or slowed down.//

    [Header("Passive Stats")]
    public float SpawnDelay; //Time it takes for this enemy to spawn.//
    public float FlinchDelay; //Time this enemy takes when flinching.//
    public float RagdollPartMass;
    [HideInInspector]
    public bool Dead;
    public bool Stunned;

    [Header("Attacking")]
    public float AttackDamage;
    public float AttackPenetration = 100f;
    public float AttackSpeed; //Number of attacks per second//
    private float CurrentAttackCooldown; //Time it takes to do the next attack//
    public float AttackDelay; //The time between starting an attack and hitting the target//
    public float AttackReach; //How far a forward attack can hit the player//
    public float SpeedWhileAttacking; //Enemy's movement speed while performing an attack//
    public float AngularSpeedWhileAttacking; //Enemy's angular movement speed while performing an attack//
    public float AccelerationWhileAttacking = 6f; //Enemy's movement acceleration while performing an attack//
    public LayerMask AttackingMask; //Objects that this enemy can melee attack//

    [Header("Debugging")]
    public bool PrintDamageFactors = false;
    public bool TestEnemy;

    [Header("Main References")]
    [Space(50)]
    public EnemyMovement AI;
    private Rigidbody MainRigidbody;
    private Rigidbody[] RagdollRigidbodies;
    private Collider MainCollider;
    private Collider[] RagdollColliders;
    private ParticleSystem BloodEruption;
    public Material AliveMaterial;
    public Material DeadMaterial;
    public GameObject Graphics;
    public GameObject AttackEffect;

    private GameManager GameManagerScript;
    private ScriptableObjectManager ScriptableObjectManagerScript;
    private PhysicMaterialCollection PhysicMaterialCollectionScript;
    private ImpactEffectCollection ImpactEffectCollectionScript;
    public WaveSpawner Spawner;
    [HideInInspector]
    public PlayerStats [] TargetStats;

    void Awake ()
    {
        GameManagerScript = FindObjectOfType<GameManager>();
        ScriptableObjectManagerScript = GameManagerScript.GetComponent<ScriptableObjectManager>();
        PhysicMaterialCollectionScript = ScriptableObjectManagerScript.PhysicMaterialCollectionScript;
        ImpactEffectCollectionScript = ScriptableObjectManagerScript.ImpactEffectCollectionScript;

        MainRigidbody = GetComponent<Rigidbody>();
        MainCollider = GetComponent<Collider>();
        RagdollRigidbodies = GetComponentsInChildren<Rigidbody>();
        RagdollColliders = GetComponentsInChildren<Collider>();
        BloodEruption = GetComponentInChildren<ParticleSystem>();

        for (int i = 0; i < RagdollColliders.Length; i++)
        {
            if (RagdollColliders [i] == MainCollider)
            {
                RagdollColliders [i] = null;
            }

            if (RagdollColliders [i] != null)
            {
                RagdollColliders [i].enabled = true;
            }
        }

        for (int i = 0; i < RagdollRigidbodies.Length; i++)
        {
            if (RagdollRigidbodies [i] == MainRigidbody)
            {
                RagdollRigidbodies [i] = null;
            }

            if (RagdollRigidbodies[i] != null)
            {
                RagdollRigidbodies [i].isKinematic = true;
                RagdollRigidbodies [i].useGravity = false;
                RagdollRigidbodies [i].mass = RagdollPartMass;
                RagdollRigidbodies [i].velocity = new Vector3(0f, 0f, 0f);
                RagdollRigidbodies [i].angularVelocity = new Vector3(0f, 0f, 0f);
            }
        }

        if (MainCollider != null)
        {
            MainCollider.enabled = true;
        }
        if (MainRigidbody != null)
        {
            MainRigidbody.isKinematic = true;
            MainRigidbody.useGravity = true;
            MainRigidbody.velocity = new Vector3(0f, 0f, 0f);
            MainRigidbody.angularVelocity = new Vector3(0f, 0f, 0f);
        }
    }

    void Start()
    {
        Spawner = FindObjectOfType<WaveSpawner>();
        AI = GetComponent<EnemyMovement>();
        Dead = false;

        AI.Agent.stoppingDistance = AttackReach - 0.2f;
        AI.Agent.speed = Speed;
        AI.Agent.angularSpeed = AngularSpeed;
        AI.Agent.acceleration = Acceleration;

        CurrentSpeed = Speed;
        CurrentAngularSpeed = AngularSpeed;
        CurrentAcceleration = Acceleration;
        CurrentHealth = Health;
        CurrentAttackCooldown = 1f / AttackSpeed;

        if (Armor <= 0f)
        {
            Armor = 1f;
        }

        if (TestEnemy)
        {
            Graphics.GetComponent<MeshRenderer>().material.CopyPropertiesFromMaterial(AliveMaterial);
        }

        Debug.Log(this.name + " loaded;");
    }

    void Update()
    {
        if (GameManagerScript.GameStatus != GameManager.GameState.InGame)
        {
            return;
        }

        if (Dead)
        {
            return;
        }

        if (CurrentAttackCooldown > 0f)
        {
            CurrentAttackCooldown -= Time.deltaTime;
        }

        AI.Agent.speed = CurrentSpeed;
        AI.Agent.angularSpeed = CurrentAngularSpeed;
        AI.Agent.acceleration = CurrentAcceleration;
    }

    public void Hurt(float Damage, float Penetration, float DamageMultiplier, bool Headshot, bool Knockback)
    {
        if (Dead)
        {
            return;
        }

        float ArmorResistance = Mathf.Max(Armor / Penetration, 1f);

        CurrentHealth -= (Damage * DamageMultiplier / ArmorResistance);

        if (PrintDamageFactors)
        {
            Debug.Log("FinalDamage:" + (Damage * DamageMultiplier / ArmorResistance));
            Debug.Log("ArmorResistance:" + ArmorResistance);
        }

        if (CurrentHealth <= 0f)
        {
            CurrentHealth = 0f;
            Death(Headshot);
        }
        else if (Knockback)
        {
            AI.Flinch();
        }
    }

    public void Death(bool Headshot)
    {
        Dead = true;
        Spawner.ZombieDeathCount++;
        Debug.Log(this.name + " died;");

        AI.Agent.enabled = false;

        if (TestEnemy)
        {
            Vector3 DeathForce = MainRigidbody.velocity * 10f;
            MainRigidbody.isKinematic = false;
            MainRigidbody.AddForce(DeathForce, ForceMode.Impulse);
            Graphics.GetComponent<MeshRenderer>().material.CopyPropertiesFromMaterial(DeadMaterial);
        }
        else
        {
            MainCollider.enabled = false;
            MainRigidbody.isKinematic = true;
            MainRigidbody.useGravity = false;

            foreach (Rigidbody RagdollPart in RagdollRigidbodies)
            {
                if (RagdollPart != null)
                {
                    RagdollPart.isKinematic = false;
                    RagdollPart.useGravity = true;

                    if (Headshot && RagdollPart.transform.name.Contains("Head"))
                    {
                        if (BloodEruption != null)
                        {
                            BloodEruption.Play();
                        }

                        GameObject BloodExplosion = Instantiate(ImpactEffectCollectionScript.ImpactEffectArray[ImpactEffectCollection.BloodExplosionIndex], RagdollPart.transform.position, Random.rotation);

                        Destroy(RagdollPart.gameObject);
                        Destroy(BloodExplosion, 5f);
                    }
                }
            }
        }

        StopAllCoroutines();
        Destroy(gameObject, 60f);
    }

    public void AttackStart(float TargetHeight)
    {
        if (!Dead && !Stunned && CurrentAttackCooldown <= 0f)
        {
            if (TargetHeight >= transform.position.y && TargetHeight <= transform.position.y + MainCollider.bounds.size.y)
            {
                CurrentAttackCooldown = 1f / AttackSpeed;
                CurrentSpeed = SpeedWhileAttacking;
                CurrentAngularSpeed = AngularSpeedWhileAttacking;
                CurrentAcceleration = AccelerationWhileAttacking;

                StartCoroutine(Attack(TargetHeight));
            }
        }
    }

    IEnumerator Attack(float TargetHeight)
    {
        yield return new WaitForSeconds(AttackDelay);

        Vector3 AttackPosition = new Vector3(transform.position.x, TargetHeight, transform.position.z);

        RaycastHit [] HitObjects = Physics.RaycastAll(AttackPosition, transform.forward, AttackReach, AttackingMask);

        for (int i = HitObjects.Length - 1; i >= 0; i--)
        {
            bool SelfHitFirst = false;
            bool ShortestNumber = true;

            if (HitObjects[i].collider == MainCollider)
            {
                SelfHitFirst = true;
            }
            else 
            {
                foreach (Collider RagdollCollider in RagdollColliders)
                {
                    if (HitObjects [i].collider == RagdollCollider)
                    {
                        SelfHitFirst = true;
                    }
                }
            }

            if (!SelfHitFirst)
            {
                for (int j = i - 1; j > 0; j--)
                {
                    bool SelfHitSecond = false;

                    if (HitObjects [j].collider == MainCollider)
                    {
                        SelfHitSecond = true;
                    }
                    else 
                    {
                        foreach (Collider RagdollCollider in RagdollColliders)
                        {
                            if (HitObjects [j].collider == RagdollCollider)
                            {
                                SelfHitSecond = true;
                            }
                        }
                    }

                    if (!SelfHitSecond)
                    {
                        if (i > 0 && HitObjects [i].distance > HitObjects [j].distance)
                        {
                            ShortestNumber = false;
                            break;
                        }
                    }
                }
            }

            if (i <= 0 || (ShortestNumber && HitObjects [i].distance < HitObjects [0].distance))
            {
                AttackHit(HitObjects [i]);
                break;
            }
        }

        CurrentSpeed = Speed;
        CurrentAngularSpeed = AngularSpeed;
        CurrentAcceleration = Acceleration;
    }

    void AttackHit (RaycastHit HitObject)
    {
        if (Dead || Stunned)
        {
            return;
        }

        int HitEffectIndex;
        Debug.Log(HitObject.transform.name + " was hit by a zombie;");

        PlayerStats Player = HitObject.transform.GetComponentInParent<PlayerStats>();
        Destructible DestructibleObject = HitObject.transform.GetComponentInParent<Destructible>();
        PhysicMaterial HitObjectMaterial = HitObject.transform.GetComponentInParent<Collider>().sharedMaterial;
        EnemyStats Ally = HitObject.transform.GetComponentInParent<EnemyStats>();
        GameObject Impact;

        if (Player != null)
        {
            if (!Player.Dead)
            {
                Player.Hurt(AttackDamage);
            }
        }
        else if (DestructibleObject != null)
        {
            if (!DestructibleObject.Dead)
            {
                DestructibleObject.Hurt(AttackDamage, AttackPenetration);
            }
        }

        if (Ally == null)
        {
            HitEffectIndex = PhysicMaterialCollectionScript.SetImpact(HitObjectMaterial);

            Impact = Instantiate(ImpactEffectCollectionScript.ImpactEffectArray [HitEffectIndex], HitObject.point, Quaternion.LookRotation(HitObject.normal));
            Destroy(Impact, 2f);
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.localPosition + new Vector3(0f, 0f, AttackReach));
    }
}
