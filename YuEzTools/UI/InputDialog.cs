using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using static UnityEngine.UI.Button;

namespace YuEzTools.UI;

public class InputDialog
{
    public GameObject DialogObject { get; private set; }
    public TextMeshPro TitleText { get; private set; }
    public TextMeshPro InfoText { get; private set; }
    public PassiveButton SubmitButton { get; private set; }
    public PassiveButton CloseButton { get; private set; }
    
    private List<InputField> inputFields = new List<InputField>();
    
    public class InputField
    {
        public GameObject FieldObject { get; set; }
        public TextMeshPro LabelText { get; set; }
        public TextMeshPro InputText { get; set; }
        public SpriteRenderer Background { get; set; }
        public TextBoxTMP TextBox { get; set; }
        public int CharacterLimit { get; set; } = 0;
    }
    
    public static InputDialog Create(string title, string info, Transform parent = null)
    {
        if (parent == null) parent = AccountManager.Instance.transform;
        
        var template = parent.Find("PremissionRequestWindow");
        if (template == null) return null;
        
        var dialog = new InputDialog();
        dialog.DialogObject = GameObject.Instantiate(template.gameObject, parent);
        dialog.DialogObject.name = "InputDialog";
        dialog.DialogObject.SetActive(false);
        
        dialog.SetupBasicElements(title, info);
        dialog.SetupCloseButton();
        dialog.HideUnusedElements();
        
        return dialog;
    }
    
    private void SetupBasicElements(string title, string info)
    {
        TitleText = DialogObject.transform.Find("TitleText_TMP").GetComponent<TextMeshPro>();
        TitleText.text = title;
        var titleTranslator = DialogObject.transform.Find("TitleText_TMP").GetComponent<TextTranslatorTMP>();
        if (titleTranslator != null) GameObject.Destroy(titleTranslator);
        
        InfoText = DialogObject.transform.Find("InfoText_TMP").GetComponent<TextMeshPro>();
        InfoText.text = info;
        var infoTranslator = DialogObject.transform.Find("InfoText_TMP").GetComponent<TextTranslatorTMP>();
        if (infoTranslator != null) GameObject.Destroy(infoTranslator);
        
        SubmitButton = DialogObject.transform.Find("SubmitButton").GetComponent<PassiveButton>();
        SubmitButton.OnClick = new ButtonClickedEvent();
        SubmitButton.gameObject.SetActive(true);
    }
    
    private void SetupCloseButton()
    {
        CloseButton = GameObject.Instantiate(SubmitButton, SubmitButton.transform.parent);
        CloseButton.gameObject.name = "CloseButton";
        CloseButton.transform.Find("Text_TMP").GetComponent<TextMeshPro>().text = "";
        var closeTranslator = CloseButton.transform.Find("Text_TMP").GetComponent<TextTranslatorTMP>();
        if (closeTranslator != null) GameObject.Destroy(closeTranslator);
        CloseButton.transform.GetChild(0).GetComponent<SpriteRenderer>().sprite = LoadSprite("YuEzTools.Resources.Close.png", 100f);
        CloseButton.transform.localPosition = new Vector3(-3.7f, 2.4f, 0);
        CloseButton.transform.localScale = new Vector3(0.3f, 1.2f, 0);
        CloseButton.OnClick = new ButtonClickedEvent();
        CloseButton.OnClick.AddListener((Action)(() => Close()));
    }
    
    private void HideUnusedElements()
    {
        var guardianEmailTitle = DialogObject.transform.Find("GuardianEmailTitle_TMP");
        if (guardianEmailTitle != null) guardianEmailTitle.gameObject.SetActive(false);
        
        var guardianEmailConfirm = DialogObject.transform.Find("GuardianEmailConfirm");
        if (guardianEmailConfirm != null) guardianEmailConfirm.gameObject.SetActive(false);
        
        var guardianEmailConfirmTitle = DialogObject.transform.Find("GuardianEmailConfirmTitle_TMP");
        if (guardianEmailConfirmTitle != null) guardianEmailConfirmTitle.gameObject.SetActive(false);
        
        var guardianEmail = DialogObject.transform.Find("GuardianEmail");
        if (guardianEmail != null) guardianEmail.gameObject.SetActive(false);
        
        var child9 = DialogObject.transform.GetChild(9);
        if (child9 != null) child9.gameObject.SetActive(false);
    }
    
    public InputField AddInputField(string label, Vector2 position, Vector2 size, int characterLimit = 0)
    {
        var emailTemplate = DialogObject.transform.Find("GuardianEmail");
        if (emailTemplate == null) return null;
        
        var fieldObj = GameObject.Instantiate(emailTemplate.gameObject, DialogObject.transform);
        fieldObj.name = $"InputField_{inputFields.Count}";
        fieldObj.SetActive(true);
        fieldObj.transform.localPosition = new Vector3(position.x, position.y, 0);
        
        var emailBehaviour = fieldObj.GetComponent<EmailTextBehaviour>();
        if (emailBehaviour != null) GameObject.Destroy(emailBehaviour);
        
        var bg = fieldObj.transform.GetChild(0).GetComponent<SpriteRenderer>();
        bg.size = size;
        
        var collider = fieldObj.GetComponent<BoxCollider2D>();
        if (collider != null) collider.size = size;
        
        var textBox = fieldObj.GetComponent<TextBoxTMP>();
        textBox.characterLimit = -1;
        
        var inputText = fieldObj.transform.GetChild(1).GetComponent<TextMeshPro>();
        inputText.transform.localPosition = new Vector3(0f, size.y / 2 - 0.2f, 0);
        inputText.enableWordWrapping = true;
        inputText.rectTransform.sizeDelta = size;

            textBox.OnChange = new ButtonClickedEvent();
            textBox.OnChange.AddListener((Action)(() =>
            {
                inputText.transform.localPosition = new Vector3(0f, size.y / 2 - 0.2f * inputText.textInfo.lineCount, 0);
            }));
        
        var labelTemplate = DialogObject.transform.Find("GuardianEmailTitle_TMP");
        if (labelTemplate != null)
        {
            var labelInstance = GameObject.Instantiate(labelTemplate.gameObject, DialogObject.transform);
            labelInstance.name = $"Label_{inputFields.Count}";
            labelInstance.SetActive(true);
            labelInstance.transform.localPosition = new Vector3(-2.3f, position.y + size.y / 2 + 0.15f, 0);
            var labelText = labelInstance.GetComponent<TextMeshPro>();
            labelText.text = label;
            labelText.fontSize = 2.5f;
            var labelTranslator = labelInstance.GetComponent<TextTranslatorTMP>();
            if (labelTranslator != null) GameObject.Destroy(labelTranslator);
            
            var field = new InputField
            {
                FieldObject = fieldObj,
                LabelText = labelText,
                InputText = inputText,
                Background = bg,
                TextBox = textBox,
                CharacterLimit = characterLimit
            };
            
            inputFields.Add(field);
            return field;
        }
        
        return null;
    }
    
    public void SetSubmitAction(Action<List<string>> onSubmit)
    {
        SubmitButton.OnClick.RemoveAllListeners();
        SubmitButton.OnClick.AddListener((Action)(() =>
        {
            var values = new List<string>();
            bool hasEmpty = false;
            
            foreach (var field in inputFields)
            {
                var text = field.InputText.text;
                if (string.IsNullOrWhiteSpace(text))
                {
                    field.Background.color = Color.red;
                    hasEmpty = true;
                }
                else
                {
                    field.Background.color = Color.white;
                    values.Add(text);
                }
            }
            
            if (!hasEmpty)
            {
                onSubmit(values);
            }
        }));
    }
    
    public void SetCloseAction(Action onClose)
    {
        CloseButton.OnClick.RemoveAllListeners();
        CloseButton.OnClick.AddListener((Action)(() =>
        {
            onClose?.Invoke();
            Close();
        }));
    }
    
    public void Show()
    {
        DialogObject.SetActive(true);
    }
    
    public void Close()
    {
        GameObject.Destroy(DialogObject);
    }
}
