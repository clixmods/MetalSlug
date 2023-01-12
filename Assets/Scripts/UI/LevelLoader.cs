using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class LevelLoader : MonoBehaviour
{
    [SerializeField] private Animator transition;
    [SerializeField] private float transitionTime = 1f;

    public void LaunchGame(InputAction.CallbackContext context)
    {
        switch (context.phase)
        {
            case InputActionPhase.Performed:
                LoadNextLevel();
                // 1 player
                break;
            case InputActionPhase.Canceled:
                break;
            default:
                break;
        }
    }

    private void LoadNextLevel()
    {
        StartCoroutine(LoadLevel(SceneManager.GetActiveScene().buildIndex + 1));
    }

    IEnumerator LoadLevel(int levelIndex)
    {
        transition.SetTrigger("Start");

        yield return new WaitForSeconds(transitionTime);

        SceneManager.LoadScene(levelIndex);
    }
}