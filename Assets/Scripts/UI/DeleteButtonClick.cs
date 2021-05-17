using RPG.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

public class DeleteButtonClick : MonoBehaviour
{
    Button button;

    private void Awake()
    {
        button = transform.GetComponent<Button>();
        button.onClick.AddListener(ClickHandler);
    }

    private void ClickHandler()
    {
        SavingWrapper wrapper = FindObjectOfType<SavingWrapper>();
        wrapper.Delete();
    }
}
