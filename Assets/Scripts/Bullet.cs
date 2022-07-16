using UnityEngine;

public class Bullet : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        print("Bullet hit");
        Destroy(gameObject);
    }
}
