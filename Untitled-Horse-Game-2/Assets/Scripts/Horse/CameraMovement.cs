using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    [SerializeField] private float mouseSensitivity = 2f;
    public GameObject horse;
    private Vector3 offset;
    private float rotationY;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        offset = transform.position - horse.transform.position;    
    }

    // Update is called once per frame
    private void Update()
    {
        //HandleRotation();
    }

    private void LateUpdate()
    {
        transform.position = horse.transform.position + offset;
    }
}
