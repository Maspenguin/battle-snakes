using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileSpawner : MonoBehaviour {
    public Projectile projectile;
    private float attackDelay = 1;
    private float attackDuration = 0;
    private float waveDelay = 0;
    private int attackType;

    private float screenHeight = 7.5f;
    private float screenWidth = 16;

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        if(attackDelay <= 0)
        {
            attackType = (Random.Range(0, 4));
            attackDuration = 5;
            attackDelay = 10;
        }
        if(0 < attackDuration)
        {
            if (waveDelay <= 0)
            {
                if (attackType == 0)//leftScreen
                {

                    //x: 0 -> ScreenWidth/2
                    //y: 0 || ScreenHeight
                    for (int i = 0; i < 4; i++)
                    {
                        float xValue = (Random.Range(0f, screenWidth/2));
                        int o = i % 2; //When o = 0 it shoots from bellow, when 1 it shoots from above
                        MakeNewProjectile("NeutStandardProjectile", new Vector2(xValue, 10 * o), 3, 180 * o, 1, Projectile.Type.Standard);
                    }
                }
                else if (attackType == 1)//rightScreen
                {
                    //x: screenWidth/2 -> screenWidth
                    //y: 0 || screenHeight
                    for (int i = 0; i < 4; i++)
                    {
                        float xValue = (Random.Range(screenWidth/2, screenWidth));
                        int o = i % 2; //When o = 0 it shoots from bellow, when 1 it shoots from above
                        MakeNewProjectile("NeutStandardProjectile", new Vector2(xValue, 10 * o), 3, 180 * o, 1, Projectile.Type.Standard);
                    }
                }
                if (attackType == 2)//bottomScreen
                {
                    //x: 0 || screenWidth
                    //y: 0 -> screenHeight/2
                    for (int i = 0; i < 4; i++)
                    {
                        float yValue = (Random.Range(0f, screenHeight/2));
                        int o = i % 2; //When o = 0 it shoots from bellow, when 1 it shoots from above
                        MakeNewProjectile("NeutStandardProjectile", new Vector2(16 * o, yValue), 3, -90 + 180 * o, 1, Projectile.Type.Standard);
                    }
                }
                else if (attackType == 3)//topScreen
                {
                    //x: 0 || screenWidth
                    //y: screenHeight/2 -> screenHeight
                    for (int i = 0; i < 4; i++)
                    {
                        float yValue = (Random.Range(screenHeight/2, screenHeight));
                        int o = i % 2; //When o = 0 it shoots from bellow, when 1 it shoots from above
                        MakeNewProjectile("NeutStandardProjectile", new Vector2(16*o, yValue), 3, -90 + 180 * o, 1, Projectile.Type.Standard);
                    }
                }

                waveDelay = 0.5f;
            }
            waveDelay -= Time.deltaTime;
            
        }
        attackDuration -= Time.deltaTime;
        attackDelay -= Time.deltaTime;
		
	}

    void MakeNewProjectile(string name, Vector2 position, float speed, float rotation, int damage, Projectile.Type type)
    {
        Projectile newProjectile = Instantiate(projectile);
        newProjectile.transform.parent = transform;
        newProjectile.gameObject.name = name;
        newProjectile.transform.position = position;
        newProjectile.speed = speed;
        newProjectile.direction = Vector2.up.Rotate(rotation);
        newProjectile.teamNumber = 0;
        newProjectile.damage = damage;
        newProjectile.type = type;
    }
}
