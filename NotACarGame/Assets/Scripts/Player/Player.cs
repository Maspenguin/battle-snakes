using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class Player : MonoBehaviour
{
    private AudioSource audio;
    public AudioClip damageClip;
    public Player enemy;
    public float speed = 5;
    public int teamNumber = 1;
    public float endLag = 0;
    public Vector2 facingDirection;
    public int healthPoints = 10;
    private GameObject projectiles;
    public Projectile projectile;

    public ArrayList abilities = new ArrayList();

    // Use this for initialization
    void Start()
    {
        abilities.Add(new Ability(Ability.AbilityType.ParallelGun));
        abilities.Add(new Ability(Ability.AbilityType.QuickGun));
        abilities.Add(new Ability(Ability.AbilityType.XGun));
        abilities.Add(new Ability(Ability.AbilityType.DoubleShotGun));
        audio = GetComponent<AudioSource>();
        projectiles = GameObject.Find("Projectiles");
    }

    // Update is called once per frame
    void Update()
    {
        Movement();
        Attack();
    }

    void Movement()
    {
        //rotate
        facingDirection = enemy.transform.position - transform.position;
        facingDirection = facingDirection.normalized;
        //move
        float vertical = Input.GetAxisRaw("P" + teamNumber + "Vertical");
        float horizontal = Input.GetAxisRaw("P" + teamNumber + "Horizontal");
        Vector2 rawVelocity = new Vector2(horizontal, vertical);
        rawVelocity = Vector2.ClampMagnitude(rawVelocity, 1);

        transform.Translate(speed * rawVelocity * Time.deltaTime, Space.Self);

        //Walls v
        float radius = GetComponent<CircleCollider2D>().radius;
        if(transform.position.x > 16 - radius)
        {
            transform.position = new Vector2 (16 - radius, transform.position.y);
        }
        else if (transform.position.x < 0 + radius)
        {
            transform.position = new Vector2(0 + radius, transform.position.y);
        }
        if(transform.position.y > 10 - radius)
        {
            transform.position = new Vector2(transform.position.x, 10 - radius);
        }
        else if (transform.position.y < 0 + radius)
        {
            transform.position = new Vector2(transform.position.x, 0 + radius);
        }
        //^
    }
    void Attack()
    {
        foreach (Ability ability in abilities)
        {
            int num = abilities.IndexOf(ability) + 1;

            ability.NextTick();
            switch (ability.abilityType)
            {
                case Ability.AbilityType.QuickGun:
                    ability.QuickGun("P" + teamNumber + "Fire" + num, this);
                    break;
                case Ability.AbilityType.ParallelGun:
                    ability.ParallelGun("P" + teamNumber + "Fire" + num, this);
                    break;
                case Ability.AbilityType.XGun:
                    ability.XGun("P" + teamNumber + "Fire" + num, this);
                    break;
                case Ability.AbilityType.DoubleShotGun:
                    ability.DoubleShotGun("P" + teamNumber + "Fire" + num, this);
                    break;
            }
        }
        endLag -= Time.deltaTime;
    }

    void OnTriggerEnter2D(Collider2D trigger)
    {
        if (trigger.gameObject.CompareTag("Projectile"))
        {
            Projectile projectile = trigger.gameObject.GetComponent<Projectile>();
            if (projectile.teamNumber != teamNumber)
            {
                healthPoints = healthPoints - projectile.damage;
                audio.PlayOneShot(damageClip);
                Destroy(projectile.gameObject);
            }
        }
    }

    public void MakeNewProjectile(string name, Vector2 offset, float speed, float rotation, int damage, Projectile.Type type)
    {
        Projectile newProjectile = Instantiate(projectile);
        newProjectile.transform.parent = projectiles.transform;
        newProjectile.gameObject.name = teamNumber + name;
        newProjectile.transform.position = transform.position;
        newProjectile.transform.Translate(offset);
        newProjectile.speed = speed;
        newProjectile.direction = facingDirection.Rotate(rotation);
        newProjectile.teamNumber = teamNumber;
        newProjectile.damage = damage;
        newProjectile.type = type;
    }
    
}

public class Ability
{
    public enum AbilityType
    {
        QuickGun,
        ParallelGun,
        XGun,
        DoubleShotGun
    };
    public int maxChargesAvailable;
    public int chargesAvailable;
    public float maxChargeReplenishCooldown;
    public float chargeReplenishCooldown;
    public float abilityEndLag;
    private int numberOfWaves = 0;
    private float waveDelay = 0;
    public AbilityType abilityType;

    public Ability(AbilityType abilityType)
    {
        this.abilityType = abilityType;
        switch (abilityType)
        {
            case AbilityType.QuickGun:
                maxChargesAvailable = 20;
                maxChargeReplenishCooldown = 0.5f;
                abilityEndLag = 0.1f;
                break;
            case AbilityType.ParallelGun:
                maxChargesAvailable = 6;
                maxChargeReplenishCooldown = 2f;
                abilityEndLag = 1f;
                break;
            case AbilityType.XGun:
                maxChargesAvailable = 3;
                maxChargeReplenishCooldown = 2f;
                abilityEndLag = 0.9f;
                break;
            case AbilityType.DoubleShotGun:
                maxChargesAvailable = 4;
                maxChargeReplenishCooldown = 3f;
                abilityEndLag = 1f;
                break;
        }
        chargesAvailable = maxChargesAvailable;
    }


    public void NextTick()
    {
        if (chargesAvailable < maxChargesAvailable)
        {
            chargeReplenishCooldown -= Time.deltaTime;
            if (chargeReplenishCooldown <= 0)
            {
                chargesAvailable++;
                chargeReplenishCooldown = maxChargeReplenishCooldown;
            }
        }
        
    }

    //Abilities

    public void QuickGun(string button, Player player)
    {
        if (Input.GetButton(button) == true)
        {
            if (player.endLag <= 0 && 0 < chargesAvailable)
            {
                player.MakeNewProjectile("ProjectileQuickGun", Vector2.zero, 5, 0, 1, Projectile.Type.Standard);
                player.endLag = abilityEndLag;
                chargesAvailable--;
            }
        }
    }

    public void ParallelGun(string button, Player player)
    {
        if (Input.GetButton(button) == true)
        {

            if (player.endLag <= 0 &&  0 < chargesAvailable)
            {
                numberOfWaves = 5;
                player.endLag = abilityEndLag;
                chargesAvailable--;
            }
        }
        if (numberOfWaves > 0)
        {
            if (waveDelay <= 0)
            {
                player.MakeNewProjectile("ProjectileParallelGun", player.facingDirection.Rotate(90) * 0.5f, 6, 0, 1, Projectile.Type.Standard);

                player.MakeNewProjectile("ProjectileParallelGun", player.facingDirection.Rotate(-90) * 0.5f, 6, 0, 1, Projectile.Type.Standard);

                waveDelay = 0.1f;
                numberOfWaves -= 1;
            }
            waveDelay -= Time.deltaTime;
        }
    }

    public void XGun(string button, Player player)
    {
        if (Input.GetButton(button) == true)
        {
            if (player.endLag <= 0 && 0 < chargesAvailable)
            {
                player.MakeNewProjectile("ProjectileXGun", Vector2.zero, 4, 80, 1, Projectile.Type.XGunLeft);

                player.MakeNewProjectile("ProjectileXGun", Vector2.zero, 3, 70, 1, Projectile.Type.XGunLeft);

                player.MakeNewProjectile("ProjectileXGun", Vector2.zero, 4, -80, 1, Projectile.Type.XGunRight);

                player.MakeNewProjectile("ProjectileXGun", Vector2.zero, 3, -70, 1, Projectile.Type.XGunRight);
                player.endLag = abilityEndLag;
                chargesAvailable--;
            }
        }
    }

    public void DoubleShotGun(string button, Player player)
    {
        if (Input.GetButton(button) == true)
        {
            if (player.endLag <= 0 && 0 < chargesAvailable)
            {
                numberOfWaves = 2;
                player.endLag = abilityEndLag;
                chargesAvailable--;
            }

        }
        if (numberOfWaves > 0)
        {
            if (waveDelay <= 0)
            {
                if (numberOfWaves == 2)
                {
                    player.MakeNewProjectile("ProjectileDoubleShotGun", Vector2.zero, 5, -30, 1, Projectile.Type.Standard);
                    player.MakeNewProjectile("ProjectileDoubleShotGun", Vector2.zero, 5, 0, 1, Projectile.Type.Standard);
                    player.MakeNewProjectile("ProjectileDoubleShotGun", Vector2.zero, 5, 30, 1, Projectile.Type.Standard);
                }
                else
                {
                    player.MakeNewProjectile("ProjectileDoubleShotGun", Vector2.zero, 5, -50, 1, Projectile.Type.Standard);
                    player.MakeNewProjectile("ProjectileDoubleShotGun", Vector2.zero, 5, -10, 1, Projectile.Type.Standard);
                    player.MakeNewProjectile("ProjectileDoubleShotGun", Vector2.zero, 5, 10, 1, Projectile.Type.Standard);
                    player.MakeNewProjectile("ProjectileDoubleShotGun", Vector2.zero, 5, 50, 1, Projectile.Type.Standard);
                }

                waveDelay = 0.2f;
                numberOfWaves -= 1;
            }
            waveDelay -= Time.deltaTime;
        }
    }
}