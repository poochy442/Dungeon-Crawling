using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ExperienceBar : MonoBehaviour
{
    public Slider slider;
	public Text progressText, levelText;

	PlayerManager _instance;

	void Start()
	{
		_instance = PlayerManager.instance;
	}
	
    void Update()
    {
        float percent = Mathf.Min(_instance.playerStats.experience / _instance.playerStats.ExperienceToLevel, 1f);
		slider.value = percent;
		progressText.text = $"{(percent * 100f).ToString("F2")}%";
		levelText.text = $"Level {_instance.playerStats.level}";
    }
}
