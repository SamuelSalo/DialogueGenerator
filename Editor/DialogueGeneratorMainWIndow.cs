using System.Collections.Generic;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace DialogueGenerator
{
    public class DialogueGeneratorMainWindow : EditorWindow
    {
        [SerializeField] private List<DialogueGeneratorCharacter> characters;
        [SerializeField] private DialogueGeneratorScene scene;
        
        private SerializedObject serializedObject;
        private SerializedProperty serializedCharacters;
        private SerializedProperty serializedScene;
        
        [MenuItem("Window/Dialogue Generator/Dialogue Generator")]
        public static void ShowMainWindow()
        {
            var wnd = GetWindow<DialogueGeneratorMainWindow>();
            wnd.titleContent = new GUIContent("Dialogue Generator");
        }

        private void OnEnable()
        {
            characters = new List<DialogueGeneratorCharacter>();
            serializedObject = new SerializedObject(this);
            serializedCharacters = serializedObject.FindProperty("characters");
            serializedScene = serializedObject.FindProperty("scene");
        }

        public void CreateGUI()
        { 
            var root = rootVisualElement;
            serializedObject.Update();
            serializedObject.ApplyModifiedProperties();
            
            var dialogueNameField = new TextField
            {
                label = "Dialogue Name",
                tooltip = "The name of the output dialogue"
            };
            var dialogueDescriptionField = new TextField
            {
                label = "Dialogue Description",
                tooltip = "Write a description of the dialogue you want here. (Multiline)",
                multiline = true
            };
            

            var lengthDropdown = new DropdownField()
            {
                label = "Length",
                tooltip = "How long should the dialogue be?",
            };
            
            lengthDropdown.choices = new List<string>
            {
                "Single Line",
                "Short",
                "Medium",
                "Long",
                "Very Long"
            };

            var choicesToggle = new Toggle
            {
                label = "Allow choices?",
                tooltip = "Allow branching dialogue and choices in the dialogue?"
            };

            var toneField = new TextField
            {
                label = "Tone",
                tooltip = "Overall tone / theme of the dialogue",
            };

            var charactersElement = new PropertyField(serializedCharacters)
            {
                label = "Characters",
                tooltip = "Characters featured in the dialogue"
            };
            charactersElement.Bind(serializedObject);
            
            var sceneElement = new PropertyField(serializedScene)
            {
                label = "Scene",
                tooltip = "Scene, setting or situation of the dialogue"
            };
            sceneElement.Bind(serializedObject);
            
            root.Add(dialogueNameField);
            root.Add(dialogueDescriptionField);
            root.Add(lengthDropdown);
            root.Add(choicesToggle);
            root.Add(toneField);
            root.Add(charactersElement);
            root.Add(sceneElement);
            
            var generateButton = new Button
            {
                text = "Generate",
            };
            generateButton.clicked += () =>
            {
                var info = new DialogueGenerationInfo
                {
                    Name = dialogueNameField.text,
                    Description = dialogueDescriptionField.text,
                    Length = lengthDropdown.text,
                    Tone = toneField.text,
                    Characters = characters.ToArray(),
                    Scene = ScriptableObject.Instantiate(serializedScene.objectReferenceValue as DialogueGeneratorScene)
                };

                var valid = true;
                
                foreach (var field in typeof(DialogueGenerationInfo).GetFields())
                {
                    var value = field.GetValue(info);
                    switch (value)
                    {
                        case string s:
                        {
                            if (string.IsNullOrEmpty(s))
                            {
                                Debug.LogError("[DialogueGenerator] " + field.Name + " is empty!");
                                valid = false;
                            }

                            break;
                        }
                        case DialogueGeneratorScene generatorScene:
                        {
                            if (!generatorScene)
                            {
                                Debug.LogError("[DialogueGenerator] " + field.Name + " is empty!");
                                valid = false;
                            }

                            break;
                        }
                        case DialogueGeneratorCharacter[] { Length: 0 }:
                            Debug.LogError("[DialogueGenerator] " + field.Name + " is empty!");
                            valid = false;
                            break;
                    }
                }
                /*
                if (string.IsNullOrEmpty(info.Scene.Description))
                {
                    Debug.LogError("[DialogueGenerator] scene description is empty!");
                    valid = false;
                }

                
                foreach (var field in typeof(DialogueGeneratorCharacter).GetFields())
                {
                    if (field.GetValue(info) is string s)
                    {
                        if (string.IsNullOrEmpty(s))
                        {
                            Debug.LogError("[DialogueGenerator] " + field.Name + " is empty!");
                            valid = false;
                        }
                    }
                }
                */
                
                if(valid)
                    StartGeneration(ref info);
            };
            root.Add(generateButton);
        }

        private void StartGeneration(ref DialogueGenerationInfo info)
        {
            if (!EditorPrefs.HasKey("DialogueGeneratorAPIKey") || !EditorPrefs.HasKey("DialogueGeneratorEndpointUrl") || !EditorPrefs.HasKey("DialogueGeneratorModel"))
            {
                Debug.LogError("[DialogueGenerator] Dialogue generator has not been setup! Please setup using Window/DialogueGenerator/Setup");
                return;
            }
            
            DialogueGenerator.GenerateDialogueAsync(info);
        }
    }
}