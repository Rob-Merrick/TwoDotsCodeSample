using UnityEngine;
using UnityEngine.UI;

public class PlayClickSound : MonoBehaviour
{
    private void Start()
    {
        GetComponent<Button>().onClick.AddListener(() => AudioManager.Instance.PlaySound("UI_Click"));
    }
}
