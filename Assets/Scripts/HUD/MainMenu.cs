using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
	public GameObject newGameButton, continueButton;

    // Start is called before the first frame update
    void Start()
    {
		Button ngButton = newGameButton.GetComponent<Button>();
		ngButton.onClick.AddListener(OnNewGame);

		Button cButton = continueButton.GetComponent<Button>();
		cButton.onClick.AddListener(OnContinue);

		continueButton.SetActive(PlayerPrefs.HasKey("XP"));
    }

    public void OnNewGame()
	{
		PlayerPrefs.SetFloat("Armor", 0);
		PlayerPrefs.SetFloat("Damage", 6);
		PlayerPrefs.SetFloat("SpellDamage", 10);
		PlayerPrefs.SetFloat("MaxHealth", 50);
		PlayerPrefs.SetFloat("XP", 0);
		PlayerPrefs.SetInt("Level", 1);
		PlayerPrefs.Save();

		GameManager.instance.EnterGame();
	}

	public void OnContinue()
	{
		GameManager.instance.EnterGame();
	}
}
