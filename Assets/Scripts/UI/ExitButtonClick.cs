using UnityEngine;
using UnityEngine.UI;

public class ExitButtonClick : MonoBehaviour
{
    Button button;

    private void Awake()
    {
        button = transform.GetComponent<Button>();
        button.onClick.AddListener(ClickHandler);
    }

    private void ClickHandler()
    {
        Application.Quit();
    }
}
