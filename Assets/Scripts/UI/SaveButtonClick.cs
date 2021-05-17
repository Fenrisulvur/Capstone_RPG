using RPG.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

public class SaveButtonClick : MonoBehaviour
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
        wrapper.Save();
    }
}
