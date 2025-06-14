using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using YamlDotNet.Serialization;

namespace YAMLDialogueLib
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

    /// <summary>
    /// Represents a dialogue file written in YAML using this library's scheema.
    /// <para>Use <see cref="YamlDialogueParser.Parse(string)"/> to parse a YAML string into an instance of YamlDialogue.</para>
    /// </summary>
    public class YamlDialogue : IEnumerator<YamlDialogueStep>
    {       
        private readonly Dictionary<string, int> _labelIndexer;
        private readonly YamlDialogueStep[] _steps;

        private int _currentStep;

        internal YamlDialogue(YamlDialogueStep[] steps)
        {
            _steps = steps;

            _labelIndexer = new Dictionary<string, int>();
            for (int i = 0; i < steps.Length; i++)
            {
                if (steps[i].Label == null)
                    continue;

                if (_labelIndexer.ContainsKey(steps[i].Label))
                    throw new WarningException("YAML DIALOGUE WARNING: Repeated labels found on dialogue. Only the first one will be considered.");

                _labelIndexer[steps[i].Label] = i;
            }
        }

        /// <summary>
        /// Matches an <paramref name="index"/> to a <see cref="YamlDialogueStep"/>.
        /// </summary>
        /// <param name="index">Index of <see cref="YamlDialogueStep"/>.</param>
        /// <returns><see cref="YamlDialogueStep"/> at <paramref name="index"/></returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public YamlDialogueStep this[int index]
        {
            get
            {
                if (index < 0 || index >= _steps.Length)
                    throw new ArgumentOutOfRangeException(nameof(index),
                        $"YAML DIALOGUE EXCEPTION: Invalid index of '{index}' on '{nameof(YamlDialogue)}'. Dialogue contains only '{_steps.Length}' steps.");

                else return _steps[index];
            }
        }

        /// <inheritdoc/>
        public YamlDialogueStep Current => _steps[_currentStep];
        object IEnumerator.Current => Current;

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <returns><c>true</c> when next index is valid, <c>false</c> when out of bounds</returns>
        public bool MoveNext()
        {
            if (_currentStep + 1 < _steps.Length)
            {
                _currentStep++;
                return true;
            }
            return false;
        }

        /// <summary>
        /// Moves the enumerator to specified <i>label</i>.
        /// </summary>
        /// <param name="label"><i>Label</i>, as defined in the <b>YAML file</b>, to move the enumerator into.</param>
        /// <returns><c>true</c> if <i>label</i> exists, <c>false</c> if not.</returns>
        public bool MoveToLabel(string label)
        {
            if (!_labelIndexer.TryGetValue(label, out int index))
                return false;

            _currentStep = index;
            return true;
        }

        /// <summary>
        /// Moves the enumerator to the "cancel" <see cref="YamlDialogueOption"/>, on the current <see cref="YamlDialogueStep"/>.
        /// </summary>
        /// <returns><inheritdoc/></returns>
        public bool MoveToCancelOption()
        {
            return MoveToOption(Current.CancelOption);
        }

        /// <summary>
        /// Moves the enumerator to the default <see cref="YamlDialogueOption"/>, on the current <see cref="YamlDialogueStep"/>.
        /// </summary>
        /// <returns><inheritdoc/></returns>
        public bool MoveToDefaultOption()
        {
            return MoveToOption(Current.DefaultOption);
        }

        /// <summary>
        /// Moves the enumerator to an option on the current <i>step</i>.
        /// </summary>
        /// <param name="optionIndex">Index of <see cref="YamlDialogueOption"/>, on current <see cref="YamlDialogueStep"/>.</param>
        /// <returns><inheritdoc/></returns>
        /// <exception cref="InvalidOperationException"></exception>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public bool MoveToOption(int optionIndex)
        {
            if (Current.Options == null)
                throw new InvalidOperationException($"YAML DIALOGUE EXCEPTION: Cannot move to option because current step has no options. (L + ratio + no options)");

            if (optionIndex < 0 || optionIndex >= Current.Options.Length)
                throw new ArgumentOutOfRangeException(nameof(optionIndex), $"YAML DIALOGUE EXCEPTION: Invalid option index of {optionIndex}, current step only has {Current.Options.Length} options.");

            if (Current.Options[optionIndex].TargetLabel == null)
                return MoveNext();
            else
                return MoveToLabel(Current.Options[optionIndex].TargetLabel);
        }

        /// <inheritdoc/>
        public void Reset()
        {
            _currentStep = 0;
        }

        void IDisposable.Dispose() { }
    }
}
