using System.Collections;
using System;
using System.IO;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class LevelManager : MonoBehaviour {
	#region VARIABLES
	//static variables
	static public bool GameOver = false;
	static public int score = 0;
	static public float EnemyLimitAtYAxis;
	static public List<EnemyBehavior> EnemiesInScene = new List<EnemyBehavior>();

	//UI variables
	public TMP_Text[] scoreCounters;
	public TMP_Text GameOverText;
	public GameObject GameOverCanvas;

	//Player variables
	public PlayerScript player;

	//PowerUp variables
	private PowerUpBehavior powerUp;

	//Enemy spawner variables
	public GameObject enemyPrefab;
	private int maxEnemiesToSpawn = 100;
	private List<EnemyBehavior> activeEnemies = new List<EnemyBehavior>();

	//Level manager variables
	private int levelLayout;
	[SerializeField] TextAsset LevelDataFile;
	[HideInInspector] public List<int[]> LevelData = new List<int[]>();
	private int currentLevel = 0;
	#endregion
	
	void Start() {
		//Read metadata
		ReadLayout();
		ReadLevelFileData();
		//Set Losing area 
		EnemyLimitAtYAxis = player.transform.position.y + 0.5f;
		EnemyBufferBuilder();
		//Save powerUp reference
		powerUp = FindObjectOfType<PowerUpBehavior>();
		SpawnWave();
	}

	void Update() {
		foreach (TMP_Text scoreText in scoreCounters) {
			scoreText.text = "SCORE: " + (score.ToString("00000"));
		}
		if (EnemiesInScene.Count == 0) {
			currentLevel++;
			if (currentLevel > LevelData.Count) currentLevel = 0;
			SpawnWave();
		}
		if (GameOver) {
			Time.timeScale = 0;
			GameOverCanvas.SetActive(true);
		}
	}

	void ReadLayout() {
		byte[] layoutByteArray = (File.ReadAllBytes(Application.dataPath + "/Levels/Layout.metadata"));
		if (layoutByteArray.Length > 0) {
			levelLayout = Convert.ToInt32(layoutByteArray[0]);
		} else {
			LayoutPrintError();
		}
		//User has modified the file and inserted its own numbers
		if (levelLayout == 49) levelLayout = 1;
		else if (levelLayout == 50) levelLayout = 2;
		else if (levelLayout > 2 && levelLayout < 1) LayoutPrintError();
	}

	void LayoutPrintError() {
		levelLayout = 1;
		Debug.LogWarning("WARNING: LAYOUT FILE WAS MODIFIED AND NO LONGER HOLDS ITS VALUES");
	}

	void ReadLevelFileData() {
		string[] input = LevelDataFile.text.Split("\n");

		if (input.Length > 0) {
			for (int i = 0; i < input.Length; i++) {
				string[] individualStringValues = input[i].Split(",");
				int[] levelIntValues = new int[individualStringValues.Length];
				for (int j = 0; j < individualStringValues.Length; j++) {
					if (individualStringValues[j] != "") {
						levelIntValues[j] = int.Parse(individualStringValues[j]);
					}
				}
				LevelData.Add(levelIntValues);
			}
		}
		if (LevelData.Count == 0) {
			Debug.LogWarning("NO LEVELS FOUND IN TEXT FILE, MAKE SURE TO CREATE AT LEAST ONE WITH THE LEVEL EDITOR");
		}

	}


	private void EnemyBufferBuilder() {
		for (int i = 0; i < maxEnemiesToSpawn; i++) {
			EnemyBehavior tmpEnemy = Instantiate(enemyPrefab, transform.position, Quaternion.identity).GetComponent<EnemyBehavior>();
			tmpEnemy.SetInactiveState();
			activeEnemies.Add(tmpEnemy);
		}
	}

	public void SpawnWave() {
		float levelSpeedBoost = ((float)levelLayout + (float)currentLevel) / 5;

		if (currentLevel < LevelData.Count) {
			float OffsetX = 0;
			float centerX = levelLayout == 2 ? -0.25f : 0;
			int rows = 5;

			for (int i = 0; i < LevelData[currentLevel].Length; i++) {
				//Correct Layout for level 2
				if (i % rows == 0 && levelLayout == 2) {
					if (OffsetX == 0) OffsetX = 0.5f;
					else OffsetX = 0;
				}
				Vector2 pos = new Vector2(i % rows - (rows / 2) + OffsetX + centerX, (Boundary.window_height / 2) - Mathf.Floor(i / rows) + (LevelData[currentLevel].Length / rows));
				if (LevelData[currentLevel][i] != 0) {
					foreach (EnemyBehavior enemy in activeEnemies) {
						if (!enemy.isActive) {
							EnemiesInScene.Add(enemy);
							enemy.SpawnEnemy(pos, LevelData[currentLevel][i],levelSpeedBoost);
							break;
						}
					}
				}
			}
		} else {
			GameOver = true;
			GameOverText.text = "WELL DONE!\nYOU FINISHED\nTHE GAME";
		}
	}


	public void ResetScene() {
		Time.timeScale = 1;
		currentLevel = 0;
		score = 0;

		GameOver = false;
		GameOverText.text = "GAME OVER";

		while (EnemiesInScene.Count > 0) {
			EnemiesInScene[0].SetInactiveState();
			EnemiesInScene.Remove(EnemiesInScene[0]);
		}

		powerUp.SetInactiveState();
		player.ResetPlayer();
		SpawnWave();
	}

	public void TitleScreen() {
		//reset static values and leave scene ready for another run
		ResetScene();
		SceneManager.LoadScene("MainMenu");
	}

	private void OnEnable() {
		//Reset Static counter at sceneload
		EnemiesInScene = new List<EnemyBehavior>();
	}
}
