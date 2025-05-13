using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using Button = UnityEngine.UIElements.Button;

namespace DialogueGenerator
{
    public class DialogueGeneratorSetupWindow : EditorWindow
    {
        [SerializeField]
        private VisualTreeAsset m_VisualTreeAsset = default;

        [MenuItem("Window/Dialogue Generator/Setup")]
        public static void ShowSetupWindow()
        {
            var wnd = GetWindow<DialogueGeneratorSetupWindow>();
            wnd.titleContent = new GUIContent("Dialogue Generator Setup");
        }

        public void CreateGUI()
        {
            var root = rootVisualElement;

            var apiKeyField = new TextField
            {
                label = "Your API Key",
                tooltip = "API Key of the OpenAI compatible LLM of your choice"
            };
            if(EditorPrefs.HasKey("DialogueGeneratorAPIKey")) apiKeyField.SetValueWithoutNotify(EditorPrefs.GetString("DialogueGeneratorAPIKey"));
            
            var openAIEndpointField = new TextField
            {
                label = "OpenAI Endpoint",
                tooltip = "OpenAI compatible API endpoint url, (DeepSeek, ChatGPT, etc.)"
            };
            if(EditorPrefs.HasKey("DialogueGeneratorEndpointUrl")) openAIEndpointField.SetValueWithoutNotify(EditorPrefs.GetString("DialogueGeneratorEndpointUrl"));
        
            var openAIModelField = new TextField
            {
                label = "OpenAI Model",
                tooltip = "Model of the OpenAI compatible LLM of your choice (ex. deepseek-chat, gpt-4o, etc.)"
            };
            if(EditorPrefs.HasKey("DialogueGeneratorModel")) openAIModelField.SetValueWithoutNotify(EditorPrefs.GetString("DialogueGeneratorModel"));
            
            var saveButton = new Button
            {
                text = "Save"
            };
            saveButton.clicked += () => SaveApiKey(apiKeyField.text, openAIEndpointField.text, openAIModelField.text);

            root.Add(apiKeyField);
            root.Add(openAIEndpointField);
            root.Add(openAIModelField);
            root.Add(saveButton);
        }

        private static void SaveApiKey(string apiKey, string endpointUrl, string model)
        {
            EditorPrefs.SetString("DialogueGeneratorAPIKey", apiKey);
            Debug.Log("[Dialogue Generator] Saved API Key: " + EditorPrefs.GetString("DialogueGeneratorAPIKey"));
            EditorPrefs.SetString("DialogueGeneratorEndpointUrl", endpointUrl);
            Debug.Log("[Dialogue Generator] Saved Endpoint URL: " + EditorPrefs.GetString("DialogueGeneratorEndpointUrl"));
            EditorPrefs.SetString("DialogueGeneratorModel", model);
            Debug.Log("[Dialogue Generator] Saved Model: " + EditorPrefs.GetString("DialogueGeneratorModel"));
        }
    }
}