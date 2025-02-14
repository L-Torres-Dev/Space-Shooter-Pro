using UnityEngine;
using UnityEngine.UI;

public class ShieldUI : MonoBehaviour
{
    [SerializeField] Image[] images;

    public void VisualizeShields(int shieldStrength)
    {
        ClearAll();

        for (int i = 0; i < shieldStrength; i++)
        {
            images[i].gameObject.SetActive(true);
        }
    }

    private void ClearAll()
    {
        for (int i = 0; i < images.Length; i++)
        {
            images[i].gameObject.SetActive(false);
        }
    }
}