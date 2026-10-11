using UnityEngine;
using UnityEngine.SceneManagement;

public class DeathZone : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // 현재 씬 재로드
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }
}