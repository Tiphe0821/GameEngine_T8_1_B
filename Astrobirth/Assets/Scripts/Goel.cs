using UnityEngine;
using UnityEngine.SceneManagement;

public class Goal : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            string currentScene = SceneManager.GetActiveScene().name;

            if (currentScene == "Scene_3")
            {
                SceneManager.LoadScene("Scene_1");
            }
            else if (currentScene == "Scene_1")
            {
                SceneManager.LoadScene("Scene_2");
            }
            else if (currentScene == "Scene_2")
            {
                SceneManager.LoadScene("Scene_3");
            }
        }
    }
}