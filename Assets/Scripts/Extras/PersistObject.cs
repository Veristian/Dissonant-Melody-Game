using UnityEngine;

public class PersistObject : MonoBehaviour
{
    void Awake()
    {
        // Check if an instance of this object already exists
        GameObject[] objs = FindObjectsOfType(typeof(GameObject)) as GameObject[];
        foreach (GameObject obj in objs)
        {
            if (obj == this.gameObject) continue;
            if (obj.GetComponent<PersistObject>() != null)
            {
                Destroy(this.gameObject);
                return;
            }
        }
        DontDestroyOnLoad(this.gameObject);
    }
}