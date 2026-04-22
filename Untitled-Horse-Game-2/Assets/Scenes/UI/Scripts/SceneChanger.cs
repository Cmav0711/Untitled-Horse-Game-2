using UnityEngine;
using UnityEngine.SceneManagement;


public class SceneChanger : MonoBehaviour
{
    public void LoadWalk()
    {
        SceneManager.LoadScene(2);
    }

    public void LoadCar()
    {
        SceneManager.LoadScene(3);
    }
    public void LoadVN()
    {
        SceneManager.LoadScene(4);
    }
}