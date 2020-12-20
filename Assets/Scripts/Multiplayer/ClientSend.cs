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
	#endregion
}
