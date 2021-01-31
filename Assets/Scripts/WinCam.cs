using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class WinCam : MonoBehaviour
{
    public TMP_Text mainText;

    public void DisplayWinner(string _name)
    {
        mainText.text = $"{_name} wins!!!";
    }
}
