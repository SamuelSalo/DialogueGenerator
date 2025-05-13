using System;
using System.ClientModel;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Ink;
using OpenAI;
using OpenAI.Chat;
using UnityEditor;
using UnityEngine;

namespace DialogueGenerator
{
    public static class DialogueGenerator
    {
        private static OpenAIClient OpenAIClient;
        private static ChatClient ChatClient;
        
        private static void InitOpenAIClient()
        {
            var options = new OpenAIClientOptions
            {
                Endpoint = new Uri(EditorPrefs.GetString("DialogueGeneratorEndpointUrl"))
            };
            
            var credential = new ApiKeyCredential(EditorPrefs.GetString("DialogueGeneratorAPIKey"));
            
            OpenAIClient = new OpenAIClient(credential, options);
            ChatClient = OpenAIClient.GetChatClient(EditorPrefs.GetString("DialogueGeneratorModel"));
        }
        public static async Task GenerateDialogueAsync(DialogueGenerationInfo dialogueGenerationInfo)
        {
            Debug.Log("[DialogueGenerator] Generating dialogue...");
            if(OpenAIClient == null) InitOpenAIClient();
            
            var charactersPrompt = new StringBuilder();

            foreach (var character in dialogueGenerationInfo.Characters)
            {
                charactersPrompt.Append(character.ToPrompt());
                charactersPrompt.Append("\n");
            }
            
            var prompt = $@"
Generate character dialogue using the Ink scripting language.

Rules:
1. Output valid Ink script only. IMPORTANT Do not wrap the output in code blocks (triple quote) or markdown.
2. Each line of dialogue must be a spoken sentence, immediately followed on the same line by a tag in the format: #Actor=CharacterName
3. Do not include the character’s name inside the spoken dialogue text.
4. Follow the personality and tone guidelines for each character.
5. Stay focused on the provided topic or situation.
6. Do not include any formatting, explanation, or commentary — only raw Ink script lines.
7. Start immediately with the first line of dialogue without an entry knot.
8. If the input description asks for choices, generate choices and knots in ink format.

Choice example:
* Choice text 1
       Response line #Actor=CharacterName
   * Choice text 2
       Response line #Actor=CharacterName

* Choice that leads to knot
    Response line #Actor=CharacterName
    -> exampleknot
== exampleknot ==
Dialogue continues

Characters:
{charactersPrompt}

Scene: 
{dialogueGenerationInfo.Scene}

Dialogue Description:
{dialogueGenerationInfo.Description}

Style:
Dialogue length: {dialogueGenerationInfo.Length}
Tone: {dialogueGenerationInfo.Tone}
Allow choices and branching: {dialogueGenerationInfo.AllowChoicesAndBranching}
                           ";
            var resultTask = ChatClient.CompleteChatAsync(prompt);
            var result = await resultTask;
            Debug.Log(result.Value.Content[0].Text);

            var inkCompiler = new Compiler(result.Value.Content[0].Text);
            var json = inkCompiler.Compile().ToJson();
            
            var path = Path.Combine(Application.dataPath, $"Resources/DialogueGenerator/Output/{dialogueGenerationInfo.Name}.json");
            var directory = Path.GetDirectoryName(path);
            
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }
            
            await File.WriteAllTextAsync(path, json);
            AssetDatabase.Refresh();
            Debug.Log("[DialogueGenerator] Dialogue generation complete. Output file: " + path);
        }
    }

    public struct DialogueGenerationInfo
    {
        public string Name;
        public string Description;
        public string Length;
        public bool AllowChoicesAndBranching;
        public string Tone;
        public DialogueGeneratorCharacter[] Characters;
        public DialogueGeneratorScene Scene;

        public DialogueGenerationInfo(string name, string description, string length, bool allowChoicesAndBranching, string tone, DialogueGeneratorCharacter[] characters, DialogueGeneratorScene scene)
        {
            this.Name = name;
            this.Description = description;
            this.Length = length;
            this.AllowChoicesAndBranching = allowChoicesAndBranching;
            this.Tone = tone;
            this.Characters = characters;
            this.Scene = scene;
        }
    }
}
