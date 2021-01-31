using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class KillCam : MonoBehaviour
{
    public TMP_Text mainKillText;
    public TMP_Text killTimer;

    public IEnumerator DisplayKill(string _name, int _timer)
    {
        mainKillText.text = $"You died! (Killed by {_name}).";
        for (int i = _timer; i > 0; i--)
        {
            killTimer.text = i.ToString();
            yield return new WaitForSeconds(1f);
        }
        killTimer.text = "Respawning!";
    }
}
