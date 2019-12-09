using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class Player : MonoBehaviour
{
    //Cyborg Phantom Battle Snakes
    private AudioSource audio;
    public AudioClip damageClip;
    public Player enemy;

    public float baseMoveSpeed = 5;
    public float speed;
    public Vector2 rawVelocity;//Velocity from input, does not take into account speed
    public int teamNumber = 1;
    public float endLag = 1;
    public float maxEndLag;//For use by HUD
    public Vector2 facingDirection;
    public int healthPoints = 10;
    private GameObject projectiles; //Used to organise the projectiles spawned by the player
    private Projectile projectilePrefab; //Prefab to be used by the projectile?
    private Trail trailPrefab;
   
    public ArrayList abilities = new ArrayList();

    public string characterName;
    private Sprite playerFlash;
    private Sprite playerSprite;
    private Sprite trailSprite;

    private void Awake()
    {
        abilities.Add(new Ability(Ability.AbilityType.ParallelGun));
        abilities.Add(new Ability(Ability.AbilityType.QuickGun));
        abilities.Add(new Ability(Ability.AbilityType.XGun));
        abilities.Add(new Ability(Ability.AbilityType.DoubleShotGun));
        abilities.Add(new Ability(Ability.AbilityType.Shrink));
    }

    void Start()
    {
        print("this1");
        projectilePrefab = Resources.Load("Prefabs/Projectile", typeof(Projectile)) as Projectile;
        trailPrefab = Resources.Load("Prefabs/Trail", typeof(Trail)) as Trail;
        speed = baseMoveSpeed;

        audio = GetComponent<AudioSource>();
        projectiles = GameObject.Find("Projectiles");

        playerFlash = Resources.Load<Sprite>("Sprites/PlayerFlash") as Sprite;

        playerSprite = Resources.Load<Sprite>("Sprites/"+ characterName) as Sprite;
        GetComponent<SpriteRenderer>().sprite = playerSprite;

        trailSprite = Resources.Load<Sprite>("Sprites/"+ characterName +"Trail") as Sprite;
        GameObject player1Full = GameObject.Find("Player1Full");
        GameObject player2Full = GameObject.Find("Player2Full");
        Transform prevTransform = transform;
        for (int i = 0; i < 20; i++)
        {
            Trail newTrail = Instantiate(trailPrefab);
            newTrail.parent = prevTransform;
            prevTransform = newTrail.transform;

            if (teamNumber == 1)
            {
                newTrail.transform.parent = player1Full.transform;
            }
            else
            {
                newTrail.transform.parent = player2Full.transform;
            }
            newTrail.gameObject.name = teamNumber + "Trail" + i;
            newTrail.teamNumber = teamNumber;
            newTrail.GetComponent<SpriteRenderer>().sprite = trailSprite;
            newTrail.GetComponent<SpriteRenderer>().color = new Color(0.5f, 0.5f, 0.5f);
            newTrail.transform.position = new Vector3(transform.position.x+i*0.35f, transform.position.y, 0.1f);
            //newTrail.player = transform; //Used for alternate movement
            //newTrail.trailNum = i+1;//Used for alternate movement
            //Setlocation!
        }
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
        rawVelocity = new Vector2(horizontal, vertical);
        rawVelocity = Vector2.ClampMagnitude(rawVelocity, 1);

        transform.Translate(speed * rawVelocity * Time.deltaTime, Space.Self);

        //Walls v
        float radius = GetComponent<CircleCollider2D>().radius;
        if (transform.position.x > 16 - radius)
        {
            transform.position = new Vector2(16 - radius, transform.position.y);
        }
        else if (transform.position.x < 0 + radius)
        {
            transform.position = new Vector2(0 + radius, transform.position.y);
        }
        if (transform.position.y > 7.5f - radius)
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
                case Ability.AbilityType.Shrink:
                    ability.Shrink("P" + teamNumber + "Fire" + num, this);
                    break;
                //todo: add more ability calls
            }
        }
        endLag -= Time.deltaTime;
    }

    void OnTriggerEnter2D(Collider2D trigger)
    {
        if (trigger.gameObject.CompareTag("Projectile"))
        {
            Projectile projectile = trigger.GetComponent<Projectile>();
            if (projectile.teamNumber != teamNumber)
            {
                healthPoints = healthPoints - projectile.damage;
                audio.PlayOneShot(damageClip);

                PlayerFlash();

                //speedboost:
                //speed = baseMoveSpeed * 1.5f;
                Destroy(projectile.gameObject);
            }
        }
        if (trigger.gameObject.CompareTag("Trail"))
        {
            Trail trail = trigger.GetComponent<Trail>();
            if (trail.teamNumber != teamNumber)
            {
                healthPoints = healthPoints - trail.damage;
                audio.PlayOneShot(damageClip);

                PlayerFlash();
            }
        }
    }

    public void PlayerFlash()
    {
        CancelInvoke("PlayerFlashOff");
        GetComponent<SpriteRenderer>().sprite = playerFlash;
        Invoke("PlayerFlashOff", 0.1f);
    }

    public void PlayerFlashOff()
    {
        GetComponent<SpriteRenderer>().sprite = playerSprite;
    }

    public Projectile MakeNewProjectile(string name, Vector2 offset, float speed, float rotation, int damage, Projectile.Type type)
    {
        Projectile newProjectile = Instantiate(projectilePrefab);
        newProjectile.transform.parent = projectiles.transform;
        newProjectile.gameObject.name = teamNumber + name;
        newProjectile.transform.position = transform.position;
        newProjectile.transform.Translate(offset);
        newProjectile.speed = speed;
        newProjectile.direction = facingDirection.Rotate(rotation);
        newProjectile.teamNumber = teamNumber;
        newProjectile.damage = damage;
        newProjectile.type = type;
        return newProjectile;
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
        Shrink,
        //new:
        CircleGun,
        Dash,//Dash with a small ammount of invincibility
        Sprint,
        ProjectileDestroy
        //todo: add more ability types
    };
    public Sprite abilityIcon;
    public int maxChargesAvailable;
    public int chargesAvailable;
    public float maxChargeReplenishCooldown;
    public float chargeReplenishCooldown;
    public float abilityEndLag;
    private int numberOfWaves = 0;
    private float waveDelay = 0;
    private float abilityDuration = 0; //Used for abilities such as shrink
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
            case AbilityType.Shrink:
                maxChargesAvailable = 5;
                maxChargeReplenishCooldown = 3;
                abilityIcon = Resources.Load<Sprite>("Sprites/shrinkAbilityIcon") as Sprite;
                break;
                //todo: add more ability constructors
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

            if (player.endLag <= 0 && 0 < chargesAvailable)
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

    public void Shrink(string button, Player player)
    {
        if (Input.GetButton(button) == true)
        {
            if(0 < chargesAvailable && abilityDuration <= 0)
            {
                abilityDuration = 1;
                chargesAvailable--;
                MonoBehaviour.print("blah2");
            }
        }
        if (abilityDuration > 0)
        {
            player.transform.localScale = new Vector3(0.5f, 0.5f, 0);
            abilityDuration -= Time.deltaTime;
        }
        else
        {
            player.transform.localScale = new Vector3(1f, 1f, 0);      
        }
    }
}