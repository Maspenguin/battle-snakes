using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManagement : MonoBehaviour {
    public Player playerPrefab;

	void Start ()
    {
        StartGame();
	}

	void Update ()
    {
		
	}
    
    void StartGame()
    {
        print("Start");
        Player player1 = Instantiate(playerPrefab);
        player1.characterName = "Snowman";
        player1.teamNumber = 1;
        player1.transform.position = new Vector3(1, 4, 0);
        player1.gameObject.name = "Player1";

        Player player2 = Instantiate(playerPrefab);
        player2.characterName = "Scuttlebug";
        player2.teamNumber = 2;
        player2.transform.position = new Vector3(13, 4, 0);
        player2.gameObject.name = "Player2";

        player1.enemy = player2;
        player2.enemy = player1;
    }
}
