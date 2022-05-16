using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
	public GameObject newGameButton, continueButton;
	
	InputManager _inputManger;

    // Start is called before the first frame update
    void Start()
    {
        _inputManger = InputManager.instance;

		Button ngButton = newGameButton.GetComponent<Button>();
		ngButton.onClick.AddListener(OnNewGame);

		Button cButton = continueButton.GetComponent<Button>();
		cButton.onClick.AddListener(OnNewGame);

		continueButton.SetActive(PlayerPrefs.HasKey("XP"));
    }

    public void OnNewGame()
	{
		Debug.Log("OnNewGame");
		
		PlayerPrefs.SetFloat("Armor", 0);
		PlayerPrefs.SetFloat("Damage", 4);
		PlayerPrefs.SetFloat("MaxHealth", 100);
		PlayerPrefs.SetFloat("XP", 0);

		PlayerPrefs.Save();
		GameManager.instance.EnterTown();
	}

	public void OnContinue()
	{
		GameManager.instance.EnterTown();
	}
}
