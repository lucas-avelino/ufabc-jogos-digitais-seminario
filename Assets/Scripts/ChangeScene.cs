using UnityEngine;

public class ChangeScene : MonoBehaviour
{
    [SerializeField] private string _sceneName;
    public void ChangeToScene()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(_sceneName);
    }
}
