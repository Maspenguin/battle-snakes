using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {
    public Player enemy;
    public Projectile projectile;
    public float speed = 5;
    public int teamNumber = 1;
    public float maxAttackCooldown = 0.5f;
    private float attackCooldown = 0;
    private Vector2 facingDirection;
    public int healthPoints = 1000;
    private GameObject projectiles;

    // Use this for initialization
    void Start()
    {
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
        Vector2 verticalVelocity = Vector2.up * vertical * speed;
        Vector2 horizontalVelocity = Vector2.right * horizontal * speed;
        transform.Translate(verticalVelocity * Time.deltaTime, Space.Self);
        transform.Translate(horizontalVelocity * Time.deltaTime, Space.Self);
    }
    void Attack()
    {
        if (Input.GetButton("P" + teamNumber + "Fire1") == true)
        {
            if (attackCooldown <= 0)
            {
                Projectile newProjectile = Instantiate(projectile);
                newProjectile.transform.parent = projectiles.transform;
                newProjectile.gameObject.name = teamNumber+"Projectile";
                newProjectile.transform.position = transform.position;
                newProjectile.direction = facingDirection;
                newProjectile.teamNumber = teamNumber;

                //attackCooldown = maxAttackCooldown;
            }
        }
        attackCooldown -= Time.deltaTime;
    }

    void OnTriggerEnter2D(Collider2D trigger)
    {
        if (trigger.gameObject.CompareTag("Projectile"))
        {
            Projectile projectile = trigger.gameObject.GetComponent<Projectile>();
            if (projectile.teamNumber != teamNumber)
            {
                healthPoints = healthPoints - projectile.damage;
            }
        }
    }
}
