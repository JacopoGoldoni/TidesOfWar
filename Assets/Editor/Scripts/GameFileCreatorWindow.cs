using System.Drawing;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class GameFileCreatorWindow : EditorWindow
{
    [SerializeField]
    private VisualTreeAsset m_UXMLTree = default;

    private const string m_ButtonPrefix = "button_";
    private const string m_TextFieldPrefix = "textField_";

    private Structure_Battalion battalionStructure;

    enum GameFileTypes
    {
        Battalion,
        Company,
        ArtilleryBattery,
        Country,
        Region,
        Province
    }

    [MenuItem("Window/GameFileCreator")]
    public static void ShowWindow()
    {
        GameFileCreatorWindow wnd = GetWindow<GameFileCreatorWindow>();
        wnd.titleContent = new GUIContent("GameFileCreator");
    }

    public void CreateGUI()
    {
        VisualElement root = rootVisualElement;

        root.Add(m_UXMLTree.Instantiate());

        SetupHandlers();
    }

    private void SetupHandlers()
    {
        VisualElement root = rootVisualElement;

        var textFields = root.Query<TextField>();
        textFields.ForEach(TextFieldHandler);

        var buttons = root.Query<Button>();
        buttons.ForEach(ButtonHandler);
    }

    private void ButtonHandler(Button button)
    {
        string name = button.name.Substring(m_ButtonPrefix.Length);

        if(name == "createFile")
        {
            button.clicked += () => { 
                ArmyUtility.CreateBattalionFile(
                    battalionStructure.ID, 
                    battalionStructure.name, 
                    battalionStructure.TAG, 
                    battalionStructure.template_name
                    ); 
            };
        }
    }

    private void TextFieldHandler(TextField textField)
    {
        string name = textField.name.Substring(m_TextFieldPrefix.Length);

        if(name == "battalionName")
        {
            textField.RegisterCallback<ChangeEvent<string>>((evt) =>
            {
                textField.value = evt.newValue;
                battalionStructure.template_name = evt.newValue;
            });
        }
    }
}