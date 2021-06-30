using UnityEngine;
using UnityEngine.SceneManagement;

public class Client : MonoBehaviour
{
    [SerializeField] private GameObject enemy;
    public GameObject parent;

    public void Leave()
    {
        SceneManager.LoadScene(1);
    }

}
