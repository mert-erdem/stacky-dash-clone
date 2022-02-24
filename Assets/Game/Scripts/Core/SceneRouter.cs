using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneRouter : MonoBehaviour
{
    private void Start()
    {
        StartCoroutine(LoadLastLevel());
    }

    private IEnumerator LoadLastLevel()
    {
        yield return new WaitForSeconds(1.5f);

        int lastLevelIndex = PlayerPrefs.GetInt("LEVEL", 1);
        SceneManager.LoadScene(lastLevelIndex);
    }
}
