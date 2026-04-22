using UnityEngine;

public class PRISON : MonoBehaviour
{
    private SceneChanger _sceneChanger;

    private void Awake()
    {
        _sceneChanger = FindFirstObjectByType<SceneChanger>();
    }

    private void OnTriggerEnter(Collider other)
    {
        _sceneChanger.LoadVN();
    }
}
