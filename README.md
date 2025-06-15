<div align="center">
 <img width=128px height=128px src="./Resources/Package/Common/icon.png" alt="Project logo">

# YAML Dialogue Lib

 A .Net interpreter library for .yaml files to use in your games.

[![GitHub Release](https://img.shields.io/github/v/release/Varollo/YamlDialogueLib)](https://github.com/Varollo/YamlDialogueLib/releases)
[![NuGet Version](https://img.shields.io/nuget/v/YamlDialogueLib)](https://www.nuget.org/packages/YamlDialogueLib/)
[![License](https://img.shields.io/badge/license-MIT-lime.svg)](/LICENSE)

| [🧐 About ](#-about-) | [🏁 Getting Started ](#-getting-started-) | [🎈 Usage ](#-usage-) |
|-|-|-|
</div>

## 🧐 About <a name = "about"></a>

This is *.Net Standard* library for personal use in my ~~(and yours, if you wish)~~ games. It interprets a `.yaml` file in this custom [schema](./schema.json) into C# with a Enumerator class.

The intention behind using YAML for scripting the dialogue was to keep it simple. YAML is a very "human readable" markup language, therefore with some basic instructions, or following the schema, anyone could use this library to make dialogue for their games, regardless of programming experience.

## 🏁 Getting Started <a name = "getting_started"></a>

You can either get the compiled `.dll` of the library through the official [GitHub releases page](https://github.com/Varollo/YamlDialogueLib/releases), or install it through [nuget](#).

Keep in mind, if you choose to download the dll, you'll also need to manually install [YamlDotNet](https://github.com/aaubry/YamlDotNet).

### Prerequisites

The library is based on **.Net Standard 2.0** and uses the **YamlDotNet** to load the `.yaml` files.

### Installing

Get the package on **nuget**:

```sh
PM> Install-Package YamlDialogueLib
```

Or, if you wish, download the binaries from the [releases page](https://github.com/Varollo/YamlDialogueLib/releases), and get [YamlDotNet](https://github.com/aaubry/YamlDotNet).

## 🎈 Usage <a name="usage"></a>

To make things simple, I'll create a simple Console Application and built onto it to demonstrate how you'd use this library in practice, so just assume `// ...` means the previous code I went over.

### 1. Parsing a YAML string

First off, don't forget to add the "using" statemente at the top.

```cs
using YAMLDialogueLib;
```

Next, I'll embed a string with a test dialogue. Ideally you'd load it from a server or file.

```cs
// ...

const string DIALOGUE_STR = @"
- actor: Varollo
  line: Hello World!

- actor: World
  line: Hello! :)
"
```

To turn that string into a `YamlDialogue` object, we need to call the **parser** and ask it to parse it for us.

```cs
// ...

var dialogue = YamlDialogueParser.Parse(DIALOGUE_STR);
```

Now, we can use it by accessing any of it's propperties, such as `Actor` and `Line`, and move through it with a coroutine or by calling the `MoveNext()` method from the *IEnumerator* interface.

To keep it simple, I'll use a *do while* loop to print both steps of the dialogue.

```cs
// ...

do
{
    // Access current line of dialogue
    YamlDialogueStep step = dialogue.Current;

    // Print it to the screen, formatted nicelly
    Console.WriteLine(step.Actor + " says: " + step.Line);

} while (dialogue.MoveNext());
```

You should see this in your console, if everything went right:

```
Varollo says: Hello World!
World says: Hello! :)
```

### 2. Using actions to trigger events

A big wall of text wouldn't make a dialogue scene interesting, that's why we have `actions`.

Think of **actions** as event keywords, ofcourse you can use them as you see fit, but the intent was to bring life to a, otherwise, boring conversation.

How you'd implement them, however, is up to the framework or engine you use. We are doing this in a simple console application, so let's just display them on screen.

First, let's alter our string to include some actions in the dialogue.

```cs
const string DIALOGUE_STR = @"
- actor: Varollo
  line: Hello World!

- actor: World  
  line: Hello! :)
  actions:
  - smiles
  - does a backflip
"
```

Then, change our output to accomodate the actions. Let's use a string builder this time to make things easier, shall we?

```cs
// Remember to include this at the top, for the String Builder!
using System.Text;

// ...


// Create an instance of the String Builder outside the loop
StringBuilder stepBuilder = new StringBuilder();

do
{
    // Access current line of dialogue
    YamlDialogueStep step = dialogue.Current;

    stepBuilder.Append(step.Actor + " says: ");
    stepBuilder.Append(step.Line);

    // Make sure we skip adding actions if the step has none
    if (step.Actions != null)
    {
        // step.Actions is an array, so let's use a for statement to loop through it
        for (int i = 0; i < step.Actions.Length; i++)
        {
            stepBuilder.Append(" *" + step.Actions[i]);
        }
    }

    // Use an empty AppendLine to skip to the next line
    stepBuilder.AppendLine();

} while (dialogue.MoveNext());

// Note that not only I changed the output to use the builder, but moved it out of the loop
Console.WriteLine(stepBuilder.ToString());
```

If everything went right, this is what your output should look like:

```
Varollo says: Hello World!
World says: Hello! :) *smiles *does a backflip
```

### 3. Making choices with options

A good dialogue offers the player the ability to answer different things and respond with different answers.

You can use **options** to create branch paths in the conversation or just warp around it alongside a **label**. Let's have a look how you'd do that.

As aways, let's first alter the string to add options to our little dialogue. We'll also need a label to indicate where to warp when picking an option. I'll just add one to the start and have a repeat option at the end, to keep it simple.

```cs
const string DIALOGUE_STR = @"
- label: START

- actor: Varollo
  line: Hello World!

- actor: World  
  line: Hello! :)
  actions:
  - smiles
  - does a backflip

- actor: Varollo
  line: repeat?
# confirm_option: 0 # if you want to specify a default confirmation, use this
  cancel_option: 1 # index starts at 0, index 1 is the second option
  options:
  - text: sure (1)
    target: START
  - text: nope (2)
"
```

You'll notice I didn't add a target to the second option, in that case, the dialogue will just progress normaly, without warping to a label.

Now, we'll use `Console.ReadLine` to read the answer (1 or 2) and chose the option accordinly. If we get an invalid answer, we'll use the "cancel" option which, in our case, is option 2.

Add the following code bellow your handling of actions.

```cs
// ...

// Let's also check first if we have options before attempting to add them
if (step.HasOptions)
{
    // Like actions, step.Options is an array
    for (int i = 0;i < step.Options.Length; i++)
    {
        stepBuilder.Append("\n    > " +  step.Options[i].Text);
    }

    // Since we need an answer right away,
    // let's print what we have so far and clear the builder
    Console.WriteLine(stepBuilder.ToString());
    stepBuilder.Clear();

    // Then, get the answer with ReadLine
    string answer = Console.ReadLine();

    // We'll use (1) and (2) for the choice
    if (answer == "1")
    {
        // Move to option will move to the target of chosen option
        // In this case, target is label "START"
        dialogue.MoveToOption(0);
    }
    else if (answer == "2")
    {
        // Now, in this case, no target was provided,
        // so it'll just progress the dialogue foward
        dialogue.MoveToOption(1);
    }
    else
    {
        // Let's say if you give an invalid answer, use our cancel option
        dialogue.MoveToCancelOption();
    }
}
```

Finally, if nothing went terribly wrong, you should see this output

```
Varollo says: Hello World!
World says: Hello! :) *smiles *does a backflip
Varollo says: repeat?
    > sure (1)
    > nope (2)
```

Try giving different answers and see what happens.