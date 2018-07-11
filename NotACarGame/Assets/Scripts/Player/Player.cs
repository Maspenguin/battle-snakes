using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class Player : MonoBehaviour
{
    private AudioSource audio;
    public AudioClip damageClip;
    public Player enemy;

    public float baseMoveSpeed = 5;
    private float speed;
    public int teamNumber = 1;
    public float endLag = 0;
    public float maxEndLag = 0;//For use by HUD
    public Vector2 facingDirection;
    public int healthPoints = 10;
    private GameObject projectiles;
    public Projectile projectile;

    public ArrayList abilities = new ArrayList();
    private Sprite playerFlash;
    private Sprite defaultSprite;
    // Use this for initialization
    void Start()
    {
        speed = baseMoveSpeed;
        abilities.Add(new Ability(Ability.AbilityType.ParallelGun));
        abilities.Add(new Ability(Ability.AbilityType.QuickGun));
        abilities.Add(new Ability(Ability.AbilityType.XGun));
        abilities.Add(new Ability(Ability.AbilityType.DoubleShotGun));
        audio = GetComponent<AudioSource>();
        projectiles = GameObject.Find("Projectiles");

        playerFlash = Resources.Load<Sprite>("Sprites/playerFlash") as Sprite;
        defaultSprite = GetComponent<SpriteRenderer>().sprite;
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
        if(transform.position.y > 7.5f - radius)
        {
            transform.position = new Vector2(transform.position.x, 7.5f - radius);
        }
        else if (transform.position.y < 0 + radius)
        {
            transform.position = new Vector2(transform.position.x, 0 + radius);
        }
        //^
    }
    void Attack()
    {
        foreach (Ability ability in abilities)//Attempts to use each ability in order
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

                //invincibility v
                CancelInvoke("PlayerFlash");
                invincibility = true;
                flashCount = 0;
                speed = baseMoveSpeed * 1.5f;
                InvokeRepeating("PlayerFlash", 0, 0.1f);
                // ^
                Destroy(projectile.gameObject);
            }
        }
    }
    int flashCount = 0;
    bool invincibility = false;
    public void PlayerFlash()
    {
        if (GetComponent<SpriteRenderer>().sprite != playerFlash)
        {
            GetComponent<SpriteRenderer>().sprite = playerFlash;
        }
        else
        {
            GetComponent<SpriteRenderer>().sprite = defaultSprite;
            flashCount++;
        }
        if (flashCount > 5)
        {
            CancelInvoke("PlayerFlash");
            invincibility = false;
            speed = baseMoveSpeed;
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
        DoubleShotGun,
        //new:
        CircleGun,
        Blink,
        Shrink,
        Sprint,
        ProjectileDestroy
    };
    public Sprite abilityIcon;
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
                abilityIcon = Resources.Load<Sprite>("Sprites/quickGunAbilityIcon") as Sprite;
                break;
            case AbilityType.ParallelGun:
                maxChargesAvailable = 6;
                maxChargeReplenishCooldown = 2.5f;
                abilityEndLag = 1f;
                abilityIcon = Resources.Load<Sprite>("Sprites/parallelGunAbilityIcon") as Sprite;
                break;
            case AbilityType.XGun:
                maxChargesAvailable = 3;
                maxChargeReplenishCooldown = 2f;
                abilityEndLag = 0.9f;
                abilityIcon = Resources.Load<Sprite>("Sprites/xGunAbilityIcon") as Sprite;
                break;
            case AbilityType.DoubleShotGun:
                maxChargesAvailable = 4;
                maxChargeReplenishCooldown = 3f;
                abilityEndLag = 1f;
                abilityIcon = Resources.Load<Sprite>("Sprites/doubleShotGunAbilityIcon") as Sprite;
                break;
        }
        chargesAvailable = maxChargesAvailable;
        chargeReplenishCooldown = maxChargeReplenishCooldown;
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
                player.maxEndLag = abilityEndLag;
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
                player.maxEndLag = abilityEndLag;
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
                player.maxEndLag = abilityEndLag;
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
                player.maxEndLag = abilityEndLag;
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