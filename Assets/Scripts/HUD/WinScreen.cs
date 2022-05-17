using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WinScreen : MonoBehaviour
{
    public Button resetButton;

    // Start is called before the first frame update
    void Start()
    {
		resetButton.onClick.AddListener(OnReset);
    }

    public void OnReset()
	{
		PlayerPrefs.DeleteAll();
		PlayerPrefs.Save();
		Time.timeScale = 1;
		GameManager.instance.MainMenu();
	}
}
