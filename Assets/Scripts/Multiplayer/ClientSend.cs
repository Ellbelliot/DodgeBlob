using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClientSend : MonoBehaviour
{
	private static void SendTCPData(Packet _packet)
	{
		_packet.WriteLength();
		Client.instance.tcp.SendData(_packet);
	}

	private static void SendUDPData(Packet _packet)
	{
		_packet.WriteLength();
		Client.instance.udp.SendData(_packet);
	}

	#region Packets
	public static void WelcomeReceived()
	{
		using (Packet _packet = new Packet((int)ClientPackets.welcomeReceived))
		{
			_packet.Write(Client.instance.myId);
			_packet.Write(UIManager.instance.usernameField.text);
			_packet.Write(UIManager.instance.skinDropdown.value);

			SendTCPData(_packet);
		}
	}

	public static void PlayerMovement(float[] _floatInputs, bool[] _boolInputs)
	{
		using (Packet _packet = new Packet((int)ClientPackets.playerMovement))
		{
			_packet.Write(_floatInputs.Length);
			foreach (float _float in _floatInputs)
			{
				_packet.Write(_float);
			}
			_packet.Write(_boolInputs.Length);
			foreach (bool _bool in _boolInputs)
			{
				_packet.Write(_bool);
			}
			_packet.Write(GameManager.players[Client.instance.myId].head.transform.rotation);

			SendUDPData(_packet);
		}
	}

	public static void PlayerGrab(bool[] _inputs, int _slotNum)
	{
		using (Packet _packet = new Packet((int)ClientPackets.playerGrab))
		{
			_packet.Write(_inputs.Length);
			foreach (bool _input in _inputs)
			{
				_packet.Write(_input);
			}
			_packet.Write(_slotNum);

			SendUDPData(_packet);
		}
	}

	public static void PlayerAnimation(bool[] _params, float _moveMultiplier)
	{
		using (Packet _packet = new Packet((int)ClientPackets.playerAnimation))
		{
			_packet.Write(_params.Length);
			foreach (bool _param in _params)
			{
				_packet.Write(_param);
			}
			_packet.Write(_moveMultiplier);

			SendUDPData(_packet);
		}
	}
	#endregion
}
