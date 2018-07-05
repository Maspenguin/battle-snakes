using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour {
    public enum Type
    {
        Standard,
        XGunRight,
        XGunLeft
    };
    private float worldWidth = 16;
    private float worldHeight = 10;
    public Type type = Type.Standard;
    public float speed = 0;
    public Vector2 direction;
    public int teamNumber = 0;
    public int damage = 1;
    private float lifeTime = 0;
    // Use this for initialization
    void Start ()
    {
        if(teamNumber == 1)
        {
            GetComponent<SpriteRenderer>().color = new Color(0f, 1f, 0.1924231f);
        }
        else if (teamNumber == 2)
        {
            GetComponent<SpriteRenderer>().color = new Color(1f, 0.7850062f, 0f);
        }
	}
	
	// Update is called once per frame
	void Update () {
        lifeTime += Time.deltaTime;
        SpecialCases();
        Vector2 velocity = speed * direction;
        transform.Translate(velocity * Time.deltaTime, Space.Self);
       
        DespawnCheck();

    }
    
    public void DespawnCheck()
    {
        if (transform.position.x < 0 || 16 < transform.position.x || transform.position.y < 0 || 10 < transform.position.y)
        {
            Destroy(gameObject);
        }
    }

    void SpecialCases()
    {
        //if (type == Type.XGunLeft || type == Type.XGunRight)
        if (lifeTime <= 1)
        {
            if (type == Type.XGunRight)
            {
                direction = direction.Rotate(120 * Time.deltaTime);//Degrees it spins in 1 second
            }
            else if (type == Type.XGunLeft)
            {
                direction = direction.Rotate(-120 * Time.deltaTime);
            }
        }
    }
}
