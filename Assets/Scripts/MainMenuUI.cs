using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuUI : MonoBehaviour
{
    [SerializeField] private Button playButton;
    [SerializeField] private Button quitButton;
    [SerializeField] private FlowController flowController;
    // Start is called before the first frame update
    private void Awake()
    {
        playButton.onClick.AddListener( () => {
            flowController.SignInClicked();
        });
        quitButton.onClick.AddListener( () => {
            Application.Quit();
        });
    }

    // Update is called once per frame

}
