using UnityEngine;

public class Coins : MonoBehaviour
{

    [SerializeField]private GameObject floatingTextPrefab;

    void ShowText(string text)
    {
        if(floatingTextPrefab)
        {
            // Get the FloatingText Prefab
            GameObject prefab = Instantiate(floatingTextPrefab, transform.position, Quaternion.identity);
            // Set the text to the passed in text var
            prefab.GetComponentInChildren<TextMesh>().text = text;
            // Set the colour of the text component to yellow
            prefab.GetComponentInChildren<TextMesh>().color = Color.yellow;
            // Destroy the prefab after 1 second
            Destroy(prefab, 1);
        }

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Layer 3 is Player Layer
        if (collision.gameObject.layer == 3)
        {
            // Destroy the coin object
            Destroy(gameObject);
            // Show text with "+1"
            ShowText("+1");
        }
    }
}
