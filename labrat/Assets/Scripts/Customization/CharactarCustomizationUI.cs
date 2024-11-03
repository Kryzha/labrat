using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharactarCustomizationUI : MonoBehaviour
{
    [SerializeField] private Button rightButton;
    [SerializeField] private Button leftButton;
    [SerializeField] private Button hatButton;
    [SerializeField] private Button loadButton;
    [SerializeField] private Button saveButton;
    [SerializeField] private PlayerCharacterCustomized playerCharacterCustomized;

    void Awake()
    {
        rightButton.onClick.AddListener(() => {
            Debug.Log("Pose changed");
            playerCharacterCustomized.ChangePose();
        });
        leftButton.onClick.AddListener(() => {
            Debug.Log("Pose changed");
            playerCharacterCustomized.ChangePose();
        });
        hatButton.onClick.AddListener(() => {
            Debug.Log("Hat button clicked");
            playerCharacterCustomized.ChangeHat();
        });
        loadButton.onClick.AddListener(() => {
            Debug.Log("Load");
            playerCharacterCustomized.Load();
        });
        saveButton.onClick.AddListener(() => {
            Debug.Log("Save");
            playerCharacterCustomized.Save();
        });
    }


}
