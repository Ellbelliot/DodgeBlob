using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
	public static GameManager instance;

	public int respawnTimer;
	public int winScore;
	public GameObject cam;

	public static Dictionary<int, PlayerManager> players = new Dictionary<int, PlayerManager>();
	public static Dictionary<int, string> usernames = new Dictionary<int, string>();
	public static Dictionary<int, Dodgeball> dodgeballs = new Dictionary<int, Dodgeball>();

	public GameObject localPlayerPrefab;
	public GameObject playerPrefab;
	public GameObject killCamPrefab;
	public GameObject winCamPrefab;
	public Material[] skins;

	private void Awake()
	{
		if (instance == null)
		{
			instance = this;
			DontDestroyOnLoad(gameObject);
		}
		else if (instance != this)
		{
			Debug.Log("Instance already exists, destroying this....");
			Destroy(this);
		}
	}

	public void SpawnPlayer(int _id, int _skin, string _username, Vector3 _position, Quaternion _rotation)
	{
		GameObject _player;
		if (_id == Client.instance.myId)
		{
			_player = Instantiate(localPlayerPrefab, _position, _rotation);
		}
		else
		{
			_player = Instantiate(playerPrefab, _position, _rotation);
		}

		_player.GetComponent<PlayerManager>().id = _id;
		_player.GetComponent<PlayerManager>().skin.material = skins[_skin];
		_player.GetComponent<PlayerManager>().username = _username;
		players.Add(_id, _player.GetComponent<PlayerManager>());
		usernames.Add(_id, _player.GetComponent<PlayerManager>().username);
		if (players.TryGetValue(Client.instance.myId, out PlayerManager _myPlayer))
		{
			_myPlayer.RefreshScores();
		}
	}

	public void KillPlayer(int _id, int _instigatorId)
	{
		if (_instigatorId > 0)
		{
			players[_instigatorId].kills++;
		}
		players[Client.instance.myId].RefreshScores();

		if (_id == Client.instance.myId)
		{
			cam = Instantiate(killCamPrefab, Camera.main.transform.position, Camera.main.transform.rotation);
			cam.transform.eulerAngles = new Vector3(0, cam.transform.eulerAngles.y, cam.transform.eulerAngles.z);
			cam.transform.parent = null;
			string instigatorName = _instigatorId == -1 ? "the void" : players[_instigatorId].username;
			StartCoroutine(cam.GetComponent<KillCam>().DisplayKill(instigatorName, respawnTimer));
		}
		players[_id].GetComponent<PlayerGrab>().DropDodgeballs();
		players[_id].gameObject.transform.position = new Vector3(0, 0, 0);
		players[_id].gameObject.SetActive(false);
	}

	public void GameEnd(int _id)
	{
		cam = Instantiate(winCamPrefab, Camera.main.transform.position, Camera.main.transform.rotation);
		cam.transform.eulerAngles = new Vector3(0, cam.transform.eulerAngles.y, cam.transform.eulerAngles.z);
		cam.transform.parent = null;
		string winner = _id == -1 ? "the void" : players[_id].username;
		cam.GetComponent<WinCam>().DisplayWinner(winner);

		for (int i = 1; i <= players.Count; i++)
		{
			players[_id].GetComponent<PlayerGrab>().DropDodgeballs();
			players[i].gameObject.SetActive(false);
			players[i].kills = 0;
		}
	}

	public void RespawnPlayer(int _id)
	{
		players[_id].gameObject.transform.position = new Vector3(0, 0, 0);
		players[_id].gameObject.SetActive(true);

		if (_id == Client.instance.myId)
		{
			Destroy(cam);
		}
		players[Client.instance.myId].RefreshScores();
	}
}
