using UnityEngine;

namespace DialogueGenerator
{
    [CreateAssetMenu(fileName = "New Scene", menuName = "Dialogue Generator/Scene")]
    public class DialogueGeneratorScene : ScriptableObject
    {
        [Header("Scene / Situation Description")] 
        [Tooltip("Overall description of the current situation / scene")]
        public string Description;

        public string ToPrompt()
        {
            return Description;
        }
    }
}