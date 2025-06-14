using YamlDotNet.Serialization;

namespace SimpleYamlDialogue
{
    /// <summary>
    /// Represents a optional choice to take during the conversation.
    /// </summary>
    public class YamlDialogueOption
    {
        /// <summary> Text to be displayed or to represent the option. </summary>
        public string Text { get; internal set; }
        /// <summary> Label to target when choosing the option. </summary>
        [YamlMember(Alias = "target", ApplyNamingConventions = false)]
        public string TargetLabel { get; internal set; }
    }
}
