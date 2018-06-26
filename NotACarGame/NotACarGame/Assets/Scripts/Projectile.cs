using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour {

    public float speed = 5;
    public Vector2 direction;
    public int teamNumber = 0;
    public int damage = 1;
    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        Vector2 velocity = speed * direction;//Vector2.up;
        //velocity = velocity.Rotate(rotation);
        transform.Translate(velocity * Time.deltaTime, Space.Self);
    }
}
