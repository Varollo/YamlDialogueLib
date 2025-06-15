using YamlDotNet.Serialization;

namespace YamlDialogueLib
{
    public class YamlDialogueStep
    {
        /// <summary> Actions can be used to identify events or keywords and are fully optional. </summary>
        public string[] Actions { get; internal set; }
        /// <summary> The Actor represents the speaker on the conversation. </summary>
        public string Actor { get; internal set; }
        /// <summary> A Line is the text spoken on the conversation. </summary>
        public string Line { get; internal set; }
        /// <summary> Label is used to warp arround the dialogue. </summary>
        public string Label { get; internal set; }
        /// <summary> Options are possible choices to take during the conversation. </summary>
        public YamlDialogueOption[] Options { get; internal set; }
        /// <summary> Index of the "confirm" option. Used together with <see cref="Options"/> and meant to represent an "ok" or "yes" choice. </summary>
        [YamlMember(Alias = "default_option", ApplyNamingConventions = false)]
        public int ConfirmOption { get; internal set; }
        /// <summary> Index of the "cancel" option. Used together with <see cref="Options"/> and meant to represent an "cancel" or "no" choice.
        [YamlMember(Alias = "cancel_option", ApplyNamingConventions = false)]
        public int CancelOption { get; internal set; }
        /// <summary> Wether or not the step has different options. </summary>
        public bool HasOptions => Options != null && Options.Length > 0;
    }
}
