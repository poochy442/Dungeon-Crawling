using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
	public Slider slider;
	public Text text;

	PlayerManager _instance;

	void Start()
	{
		_instance = PlayerManager.instance;
	}
	
    void Update()
    {
        float percent = (_instance.playerStats.currentHealth / _instance.playerStats.maxHealth.GetValue()) * 100.0f;
		slider.value = percent;
		text.text = $"{(percent).ToString("F2")}%";
    }
}
