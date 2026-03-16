using UnityEngine;
using UnityEngine.SceneManagement;

public class SplashController : StateMachineBehaviour
{
    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        SceneManager.LoadScene("ARMain");
    }
}
