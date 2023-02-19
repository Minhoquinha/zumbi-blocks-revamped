using System.Collections;
using UnityEngine;

public class Explosive : MonoBehaviour
{
    [Header ("Explosive Type")]
    public bool ImpactExplosive;
    public bool TimedExplosive;
    public bool RemoteDetonationExplosive;

    [Header ("Main Properties")]
    public float ExplosionTime;
    public float ExplosionRadius;
    public float BlastForce;
    public float BlastDamage = 5f; //Damage dealt by the explosion's shockwave, goes through walls//
    public float FragmentDamage = 1f; //Damage dealt if an object is in direct line of sight of the explosion//
    public float FragmentPenetration = 100f;
    public float FragmentBleedingChance = 0.01f; //Chance from 0f to 1f that the explosion's fragments cause players to bleed//
    private float HitDistance;

    [Header ("Main References")]
    [Space(50)]
    public GameObject ExplosionEffect;
    [HideInInspector]
    public PlayerStats ExplosiveOwner;
    private GameManager GameManagerScript;
    private ScriptableObjectManager ScriptableObjectManagerScript;
    private PhysicMaterialCollection PhysicMaterialCollectionScript;
    private ImpactEffectCollection ImpactEffectCollectionScript;

    void Awake()
    {
        GameManagerScript = FindObjectOfType<GameManager>();
        ScriptableObjectManagerScript = GameManagerScript.GetComponent<ScriptableObjectManager>();
        PhysicMaterialCollectionScript = ScriptableObjectManagerScript.PhysicMaterialCollectionScript;
        ImpactEffectCollectionScript = ScriptableObjectManagerScript.ImpactEffectCollectionScript;
    }

    void Start()
    {
        if (TimedExplosive)
        {
            StartCoroutine(Explode());
        }
    }

    public IEnumerator Explode ()
    {
        yield return new WaitForSeconds(ExplosionTime);

        GameObject Explosion = Instantiate(ExplosionEffect, transform.position, transform.rotation);
        Destroy(Explosion, 2f);

        RaycastHit [] HitArray = Physics.SphereCastAll(transform.position, ExplosionRadius, new Vector3(1, 0, 0));
        foreach (RaycastHit Hit in HitArray)
        {
            Collider HitObject = Hit.collider;

            BlastHit(HitObject);
        }

        Destroy(gameObject);
    }

    void BlastHit (Collider HitObject)
    {
        Debug.Log(HitObject.transform.name + " was hit by an explosion;");

        EnemyStats Enemy = HitObject.transform.GetComponentInParent<EnemyStats>();
        PlayerStats Player = HitObject.transform.GetComponentInParent<PlayerStats>();
        Destructible DestructibleObject = HitObject.transform.GetComponentInParent<Destructible>();
        Rigidbody ObjectRigidbody = HitObject.GetComponent<Rigidbody>();

        if (ObjectRigidbody != null)
        {
            ObjectRigidbody.AddExplosionForce(BlastForce, transform.position, ExplosionRadius);
        }

        HitDistance = Mathf.Max(Vector3.Distance(HitObject.transform.position, transform.position), 1f);
        float CurrentBlastDamage = BlastDamage / HitDistance;

        if (Enemy != null)
        {
            if (!Enemy.Dead)
            {
                bool Knockback = BlastForce >= 150f;

                Enemy.Hurt(CurrentBlastDamage, 10000f, 1f, false, Knockback);

                if (ExplosiveOwner != null)
                {
                    PlayerHUD HUD = ExplosiveOwner.GetComponent<PlayerHUD>();

                    if (HUD != null)
                    {
                        HUD.Hitmarker.CrossFadeAlpha(1f, 0f, false);
                        HUD.Hitmarker.CrossFadeAlpha(0f, HUD.HitmarkerDuration, false);
                    }
                }
            }
        }
        else if (Player != null)
        {
            if (!Player.Dead)
            {
                Player.Hurt(CurrentBlastDamage, 0f);

                if (ExplosiveOwner != null)
                {
                    PlayerHUD HUD = ExplosiveOwner.GetComponent<PlayerHUD>();

                    if (HUD != null)
                    {
                        HUD.Hitmarker.CrossFadeAlpha(1f, 0f, false);
                        HUD.Hitmarker.CrossFadeAlpha(0f, HUD.HitmarkerDuration, false);
                    }
                }
            }
        }
        else if (DestructibleObject != null)
        {
            if (!DestructibleObject.Dead)
            {
                DestructibleObject.Hurt(CurrentBlastDamage, 10000f);
            }

            if (ExplosiveOwner != null)
            {
                PlayerHUD HUD = ExplosiveOwner.GetComponent<PlayerHUD>();

                if (HUD != null)
                {
                    HUD.Hitmarker.CrossFadeAlpha(1f, 0f, false);
                    HUD.Hitmarker.CrossFadeAlpha(0f, HUD.HitmarkerDuration, false);
                }
            }
        }

        Vector3 Direction = (HitObject.transform.position - transform.position).normalized;
        RaycastHit[] HitArray = Physics.RaycastAll(transform.position, Direction, HitDistance);
        if (HitArray.Length == 1 && HitArray[0].collider == HitObject)
        {
            FragmentHit(HitArray[0]);
        }
    }

    void FragmentHit (RaycastHit HitObject)
    {
        int HitEffectIndex;
        Debug.Log(HitObject.transform.name + " was hit by an explosion fragment;");

        EnemyStats Enemy = HitObject.transform.GetComponentInParent<EnemyStats>();
        PlayerStats Player = HitObject.transform.GetComponentInParent<PlayerStats>();
        Destructible DestructibleObject = HitObject.transform.GetComponentInParent<Destructible>();
        PhysicMaterial HitObjectMaterial = HitObject.transform.GetComponentInParent<Collider>().sharedMaterial;
        GameObject Impact;

        float CurrentFragmentDamage = FragmentDamage / HitDistance;

        if (Enemy != null)
        {
            if (!Enemy.Dead)
            {
                Enemy.Hurt(CurrentFragmentDamage, FragmentPenetration, 1f, false, false);
            }
        }
        else if (Player != null)
        {
            if (!Player.Dead)
            {
                Player.Hurt(CurrentFragmentDamage, FragmentBleedingChance);
            }
        }
        else if (DestructibleObject != null)
        {
            if (!DestructibleObject.Dead)
            {
                DestructibleObject.Hurt(CurrentFragmentDamage, FragmentPenetration);
            }
        }

        HitEffectIndex = PhysicMaterialCollectionScript.SetImpact(HitObjectMaterial);

        Impact = Instantiate(ImpactEffectCollectionScript.ImpactEffectArray [HitEffectIndex], HitObject.point, Quaternion.LookRotation(HitObject.normal));
        Destroy(Impact, 2f);
    }

    void OnCollisionEnter(Collision collision)
    {
        if (ImpactExplosive)
        {
            StartCoroutine(Explode());
        }
    }
}
