using UnityEngine;

namespace DialogueGenerator
{
    [CreateAssetMenu(fileName = "New Character", menuName = "Dialogue Generator/Character")]
    public class DialogueGeneratorCharacter : ScriptableObject
    {
        [Header("Character Description")]
        [Tooltip("Name of this character")]public string Name;
        [Tooltip("e.g., merchant, king, bandit")]public string Role;
        [Tooltip("e.g., greedy, suspicious, formal")]public string PersonalityTraits;
        [Tooltip("e.g., casual, old-fashioned")]public string SpeakingStyle;
        [Tooltip("e.g., friend of X, wife of X")]public string Relationships;

        public string ToPrompt()
        {
            return $"Name: {Name}, Role: {Role}, Personality Traits: {PersonalityTraits}, Speaking Style: {SpeakingStyle}, Relationships: {Relationships}";
        }
    }
}