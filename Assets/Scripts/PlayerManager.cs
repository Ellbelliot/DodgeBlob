using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerManager : MonoBehaviour
{
    public int id;
    public string username;
    public int kills = 0;

    public GameObject head;
    public GameObject grab;
    public SkinnedMeshRenderer skin;
    public TMP_Text usernameText = null;
    public TMP_Text killCountText = null;
    public Animator anim;
    private string previousScoreboard;

	private void Update()
	{
        if (usernameText != null)
        {
            usernameText.text = username;
        }
    }

    public void RefreshScores()
    {
        string _text = "";
        for (int j = 1; j <= GameManager.players.Count; j++)
        {
            string _nextText = "";
            int _mostKills = 0;

            for (int i = 1; i <= GameManager.players.Count; i++)
            {
                if (i != id && !GameManager.players[i].gameObject.activeSelf)
                {
                    _nextText = GameManager.usernames[i] + ": " + previousScoreboard.Split(GameManager.usernames[i].ToCharArray())[1].Split(' ')[1] + "\n";
                    _mostKills = int.Parse(previousScoreboard.Split(GameManager.usernames[i].ToCharArray())[1].Split(' ')[1].ToString());
                }
                else if (i != id && _mostKills <= GameManager.players[i].kills && !_text.Contains(GameManager.usernames[i] + ":"))
                {
                    _nextText = GameManager.usernames[i] + ": " + GameManager.players[i].kills + "\n";
                    _mostKills = GameManager.players[i].kills;
                }
                else if (_mostKills <= GameManager.players[i].kills && !_text.Contains("You:"))
                {
                    _nextText = "You: " + kills + "\n";
                    _mostKills = kills;
                }
            }

            _text = _text + _nextText;
        }
        killCountText.text = _text;
        previousScoreboard = _text;
    }
}
