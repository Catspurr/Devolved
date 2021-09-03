using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerNameInput : MonoBehaviour
{
    [Header("UI")] 
    [SerializeField] private TMP_InputField nameInputField;
    [SerializeField] private Button continueButton;
    
    public static string DisplayName { get; private set; }

    private const string PlayerPrefsNameKey = "PlayerName";

    private void Start() => SetUpInputField();

    private void SetUpInputField()
    {
        if (!PlayerPrefs.HasKey(PlayerPrefsNameKey)) return;

        var defaultName = PlayerPrefs.GetString(PlayerPrefsNameKey);
        nameInputField.text = defaultName;
        SetContinueButton(defaultName);
    } //tries to set the default text to the input field to the last saved name in case there is a saved name and set and updates the continue button

    public void SetContinueButton(string displayName)
    {
        continueButton.interactable = !string.IsNullOrEmpty(displayName);
    } //checks the string to see if its empty, if not then enable the continue button, updated each time the input field is updated

    public void SavePlayerName()
    {
        DisplayName = nameInputField.text;
        PlayerPrefs.SetString(PlayerPrefsNameKey, DisplayName);
    } //saves the name to player prefs when clicking continue
}
