using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class StartManager : MonoBehaviour
{
    public string nextSceneName;
    public void StartGame()
    {
        GameManager.Instance.SwitchScene(nextSceneName);
    }

    public void ExitGame()
    {
        Application.Quit();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif

    }
}
