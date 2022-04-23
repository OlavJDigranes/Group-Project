using UnityEngine;

public class Coins : MonoBehaviour
{

    [SerializeField]private GameObject floatingTextPrefab;

    private void Start()
    {
        
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Layer 3 is Player Layer
        if(collision.gameObject.layer == 3)
        {
            Destroy(gameObject);
            ShowText("+1");
        }
    }

    void ShowText(string text)
    {
        if(floatingTextPrefab)
        {
            GameObject prefab = Instantiate(floatingTextPrefab, transform.position, Quaternion.identity);
            prefab.GetComponentInChildren<TextMesh>().text = text;
            prefab.GetComponentInChildren<TextMesh>().color = Color.yellow;
            Destroy(prefab, 1);
        }

    }
}
