using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class NextScene : Environment
{
    public string nextSceneName; // The name of the scene to load
    protected override void PlayerEnterTrigger()
    {
        GameManager.Instance.SwitchScene(nextSceneName);
    }
}
