using SimpleYamlDialogue.Tests;
using YAMLDialogueLib;

namespace YamlDialogueLib.Tests
{
    public class YamlDialogueSerializationTests
    {
        [Test]
        public void TestTargetLabel()
        {
            const string YAML = @"
- label: foo
- line: bar";

            var result = YamlDialogueParser.Parse(YAML);
            result.MoveNext();
            result.MoveToLabel("foo");
            TestHelpers.Validate(() => result.Current == result[0]);
        }

        [Test]
        public void TestPickOptionNoTarget()
        {
            const string YAML = @"
- options:
    - text: Option
- label: Label";

            var result = YamlDialogueParser.Parse(YAML);

            result.MoveToOption(0);
            TestHelpers.Validate(() => result.Current == result[1]);
        }

        [Test]
        public void TestPickOptionWithTarget()
        {
            const string YAML = @"
- label: Label
- options:
    - target: Label";

            var result = YamlDialogueParser.Parse(YAML);

            result.MoveNext();
            result.MoveToOption(0);
            TestHelpers.Validate(() => result.Current == result[0]);
        }

        [Test]
        public void TestPickDefaultOption()
        {
            const string YAML = @"
- label: Label

- default_option: 1
  options:
    - text: Option 0
    - target: Label

- line: Wrong one";

            var result = YamlDialogueParser.Parse(YAML);

            result.MoveNext();
            result.MoveToDefaultOption();
            TestHelpers.Validate(() => result.Current == result[0]);
        }

        [Test]
        public void TestPickCancelOption()
        {
            const string YAML = @"
- label: Label

- cancel_option: 1
  options:
    - text: Option 0
    - target: Label

- line: Wrong one";

            var result = YamlDialogueParser.Parse(YAML);

            result.MoveNext();
            result.MoveToCancelOption();
            TestHelpers.Validate(() => result.Current == result[0]);
        }

        [Test]
        public void TestStepIndexIsOrderInYamlListStartingFromZero()
        {
            const string YAML = @"
- line: Line 1
- line: Line 2";

            var result = YamlDialogueParser.Parse(YAML);

            TestHelpers.Validate(() => result[1].Line == "Line 2");
        }

        [Test]
        public void TestResetMustReturnToFirstStep()
        {
            const string YAML = @"
- line: Line 1
- line: Line 2";

            var result = YamlDialogueParser.Parse(YAML);
            
            result.MoveNext();
            TestHelpers.Validate(() => result.Current.Line == "Line 2");

            result.Reset();
            TestHelpers.Validate(() => result.Current.Line == "Line 1");
        }

        [Test]
        public void TestDeserializeLinesOnly()
        {
            const string YAML = @"
- line: Line 1
- line: Line 2";

            var result = YamlDialogueParser.Parse(YAML);

            for (int i = 0; i < 2; i++)
            {
                TestHelpers.Validate(() => result.Current.Line != null);
                TestHelpers.Validate(() => result.Current.Actor == null);
                TestHelpers.Validate(() => result.Current.Actions == null);
            }
        }

        [Test]
        public void TestDeserializeComplete()
        {
            const string YAML = @"
- actor: Actor
  line: Line
  label: Label1
  actions:
  - action_1
  - action_2
  options:
  - text: Option1
    target: Target
  - text: Option2
    target: Target
  default_option: 1
  cancel_option: 1

- actor: Actor
  line: Line
  label: Label2
  actions:
  - action_1
  - action_2
  options:
  - text: Option1
    target: Target
  - text: Option2
    target: Target
  default_option: 1
  cancel_option: 1
";

            var result = YamlDialogueParser.Parse(YAML);

            for (int i = 0; i < 2; i++)
            {
                TestHelpers.Validate(() => result.Current.Actor != null, $"On step #{i}");
                TestHelpers.Validate(() => result.Current.Line != null, $"On step #{i}");
                TestHelpers.Validate(() => result.Current.Actions != null, $"On step #{i}");
                TestHelpers.Validate(() => result.Current.Label != null, $"On step #{i}");

                TestHelpers.Validate(() => result.Current.DefaultOption != 0, $"On step #{i}");
                TestHelpers.Validate(() => result.Current.CancelOption != 0, $"On step #{i}");

                for (int j = 0; j < 2; j++)
                {
                    TestHelpers.Validate(() => result.Current.Options[j].Text != null, $"On step #{i}, option #{j}");
                    TestHelpers.Validate(() => result.Current.Options[j].TargetLabel != null, $"On step #{i}, option #{j}");
                }
            }
        }
    }

}