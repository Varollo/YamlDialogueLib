namespace SimpleYamlDialogue.Tests
{
    internal static class TestHelpers
    {
        public static void Validate(Func<bool> predicate) => Validate(predicate, string.Empty);
        public static void Validate(Func<bool> predicate, string message)
        {
            if (predicate.Invoke()) Assert.Pass();
            else if (string.IsNullOrEmpty(message)) Assert.Fail();
            else Assert.Fail(message);
        }
    }
}