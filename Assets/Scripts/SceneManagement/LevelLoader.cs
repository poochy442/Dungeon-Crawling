using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LevelLoader : MonoBehaviour
{
	public static LevelLoader instance { get; private set; }
	public GameObject loadingScreen;
	public Slider slider;
	public Text text;
	Color _color = new Color(0, 115, 171, 1);

	void Awake()
	{
		instance = this;
	}

    public void LoadLevel (int sceneIndex)
	{
		StartCoroutine(LoadAsync(sceneIndex));
	}

	IEnumerator LoadAsync (int sceneIndex)
	{
		AsyncOperation operation = SceneManager.LoadSceneAsync(sceneIndex);

		loadingScreen.SetActive(true);

		while(!operation.isDone)
		{
			float progress = Mathf.Clamp01(operation.progress / 0.9f); // Correction for Unity Loading / Activation

			if(progress <= 0.8) text.color = _color;
			else text.color = Color.white;

			text.text = progress * 100f + " %";
			slider.value = progress;

			yield return null;
		}

		yield return null;
	}
}
