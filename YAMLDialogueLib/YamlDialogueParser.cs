using YamlDotNet.Serialization;

namespace YamlDialogueLib
{
    public static class YamlDialogueParser
    {
        private static IDeserializer _deserializer;

        /// <summary>
        /// Parses a <paramref name="dialogueStr"/> <c>string</c> into a <see cref="YamlDialogue"/> instance.
        /// </summary>
        /// <param name="dialogueStr"><c>string</c> containing dialogue, in YAML.</param>
        /// <returns> Parsed <see cref="YamlDialogue"/> instance.</returns>
        public static YamlDialogue Parse(string dialogueStr)
        {
            if (_deserializer == null)
                _deserializer = InitializeDeserializer();

            var wrapper = _deserializer.Deserialize<YamlDialogueStep[]>(dialogueStr);
            return new YamlDialogue(wrapper);
        }

        private static IDeserializer InitializeDeserializer()
        {
            return new DeserializerBuilder()
                .WithCaseInsensitivePropertyMatching()
                .Build();
        }
    }
}
