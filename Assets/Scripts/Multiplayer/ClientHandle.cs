using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;

public class Clienthandle : MonoBehaviour
{
	public static void Welcome(Packet _packet)
	{
		string _msg = _packet.ReadString();
		int _myId = _packet.ReadInt();

		Debug.Log($"Message sent from server: {_msg}");
		Client.instance.myId = _myId;
		ClientSend.WelcomeReceived();

		Client.instance.udp.Connect(((IPEndPoint)Client.instance.tcp.socket.Client.LocalEndPoint).Port);
	}

	public static void SpawnPlayer(Packet _packet)
	{
		int _id = _packet.ReadInt();
		string _username = _packet.ReadString();
		int _skin = _packet.ReadInt();
		Vector3 _position = _packet.ReadVector3();
		Quaternion _rotation = _packet.ReadQuaternion();

		GameManager.instance.SpawnPlayer(_id, _skin, _username, _position, _rotation);
	}

	public static void PlayerDisconnected(Packet _packet)
	{
		int _id = _packet.ReadInt();

		Destroy(GameManager.players[_id].gameObject);
		GameManager.players.Remove(_id);
		GameManager.usernames.Remove(_id);
		GameManager.players[Client.instance.myId].RefreshScores();
	}

	public static void PlayerPosition(Packet _packet)
	{
		int _id = _packet.ReadInt();
		Vector3 _position = _packet.ReadVector3();

		if (GameManager.players.TryGetValue(_id, out PlayerManager _player))
		{
			_player.gameObject.transform.position = _position;
		}
	}

	public static void PlayerRotation(Packet _packet)
	{
		int _id = _packet.ReadInt();
		Quaternion _rotation = _packet.ReadQuaternion();

		if (GameManager.players.TryGetValue(_id, out PlayerManager _player))
		{
			_player.gameObject.transform.rotation = _rotation;
		}
	}

	public static void PlayerVelocity(Packet _packet)
	{
		int _id = _packet.ReadInt();
		Vector3 _velocity = _packet.ReadVector3();

		if (GameManager.players.TryGetValue(_id, out PlayerManager _player))
		{
			_player.gameObject.GetComponent<Rigidbody>().velocity = _velocity;
		}
	}

	public static void PlayerAnimation(Packet _packet)
	{
		int _id = _packet.ReadInt();

		if (GameManager.players.TryGetValue(_id, out PlayerManager _player))
		{
			_player.anim.SetBool("Peaking", _packet.ReadBool());
			_player.anim.SetBool("Jumping", _packet.ReadBool());
			_player.anim.SetBool("Falling", _packet.ReadBool());
			_player.anim.SetBool("Land", _packet.ReadBool());
			_player.anim.SetBool("Move", _packet.ReadBool());
			_player.anim.SetBool("WallJumping", _packet.ReadBool());
			_player.anim.SetFloat("MoveMultiplier", _packet.ReadFloat());
		}
	}

	public static void DodgeballPosition(Packet _packet)
	{
		int _id = _packet.ReadInt();
		Vector3 _position = _packet.ReadVector3();

		GameManager.dodgeballs[_id].transform.position = _position;
	}

	public static void DodgeballVelocity(Packet _packet)
	{
		int _id = _packet.ReadInt();
		Vector3 _velocity = _packet.ReadVector3();

		GameManager.dodgeballs[_id].GetComponent<Rigidbody>().velocity = _velocity;
	}

	public static void DodgeballState(Packet _packet)
	{
		int _id = _packet.ReadInt();
		bool _activated = _packet.ReadBool();
		bool _enabled = _packet.ReadBool();

		GameManager.dodgeballs[_id].activated = _activated;
		GameManager.dodgeballs[_id].gameObject.SetActive(_enabled);
	}

	public static void PlayerGrab(Packet _packet)
	{
		int _id = _packet.ReadInt();
		int _slot = _packet.ReadInt();
		int _dodgeballId = _packet.ReadInt();

		Dodgeball _dodgeball = GameManager.dodgeballs[_dodgeballId];

		if (_id == Client.instance.myId)
		{
			GameManager.players[_id].GetComponent<PlayerGrab>().GrabDodgeball(_dodgeball.gameObject, _slot);
		}
		else
		{
			_dodgeball.transform.SetParent(GameManager.players[_id].grab.transform);
			_dodgeball.transform.localPosition = Vector3.zero;
			_dodgeball.GetComponent<Collider>().enabled = false;
			_dodgeball.GetComponent<Rigidbody>().isKinematic = true;
			_dodgeball.activated = false;
			_dodgeball.immunity = false;
		}
	}

	public static void PlayerThrow(Packet _packet)
	{
		int _id = _packet.ReadInt();
		int _slot = _packet.ReadInt();
		int _dodgeballId = _packet.ReadInt();

		Dodgeball _dodgeball = GameManager.dodgeballs[_dodgeballId];

		if (_id == Client.instance.myId)
		{
			GameManager.players[_id].GetComponent<PlayerGrab>().Throw(_slot);
		}
		else
		{
			_dodgeball.transform.localPosition = Vector3.zero;
			_dodgeball.GetComponent<Collider>().enabled = false;
			_dodgeball.GetComponent<Rigidbody>().isKinematic = true;
			_dodgeball.transform.parent = null;
			_dodgeball.activated = false;
			_dodgeball.immunity = false;
		}
	}

	public static void PlayerDie(Packet _packet)
	{
		int _id = _packet.ReadInt();
		int _instigatorId = _packet.ReadInt();

		GameManager.instance.KillPlayer(_id, _instigatorId);
	}

	public static void PlayerRespawn(Packet _packet)
	{
		int _id = _packet.ReadInt();

		GameManager.instance.RespawnPlayer(_id);
	}

	public static void GameEnd(Packet _packet)
	{
		int _id = _packet.ReadInt();

		GameManager.instance.GameEnd(_id);
	}
}
