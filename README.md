# Unity Source Gen

Easy source generation for Unity with simple API, geared towards Unity usage.

Everything is created from `SourceGen`. Just give it a name, then configure it with method chaining syntax.
Nothing is _required_ except values that wouldn't compile without them, such as fields & properties requiring
a type up front. The rest follows C# defaults: members are private, types internal, methods return void.

## Example

```csharp
using NPTP.UnitySourceGen.Editor;
using NPTP.UnitySourceGen.Editor.Generatable;
using NPTP.UnitySourceGen.Editor.Syntax;

GeneratableTypeDefinition player = SourceGen.NewClass("PlayerInput").Public().Static()
    .InNamespace("MyGame.Generated")
    .WithDirectives("System", "UnityEngine")
    .WithProperty(SourceGen.NewProperty("Current", "PlayerInput").Public().Static().GetPrivateSet())
    .WithEvent(SourceGen.NewEvent("OnJumped").Public().Static().OfType("Action"))
    .WithMethod(SourceGen.NewMethod("Initialize")
        .WithAttribute("RuntimeInitializeOnLoadMethod", "RuntimeInitializeLoadType.BeforeSceneLoad")
        .Private().Static()
        .Body("Current = new PlayerInput();"))
    .WithMethod(SourceGen.NewMethod("GetDevice")
        .Public().Static()
        .Returning("InputDevice")
        .Generic(GeneratableTypeParameter.Of("TDevice", "InputDevice"))
        .Taking(GeneratableParameter.Of<int>("playerID", defaultValue: "0"))
        .Expression("Current.Devices[playerID]"));

SourceGen.WriteToPath("Assets/MyGame.Generated/PlayerInput.cs", player);
```

Which writes:

```csharp
using System;
using UnityEngine;

namespace MyGame.Generated
{
    public static class PlayerInput
    {
        public static PlayerInput Current { get; private set; }

        public static event Action OnJumped;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Initialize()
        {
            Current = new PlayerInput();
        }

        public static InputDevice GetDevice<TDevice>(int playerID = 0) where TDevice : InputDevice => Current.Devices[playerID];
    }
}
```

## Naming types

A type that already exists is named with a generic overload; a type being generated in the same run is
named with a string, since there is no `T` for it yet.

```csharp
SourceGen.NewField<int>("count")                      // int count;
SourceGen.NewField("actions", "GameplayActions")      // GameplayActions actions;   (being generated)
TypeRef.Generic("List", "GameplayActions")            // List<GameplayActions>
```

Names taken from assets or config are sanitized into valid identifiers automatically, so
`"Keyboard&Mouse"` becomes `KeyboardMouse` and `"class"` becomes `@class`. Type names are sanitized too,
per identifier, so the punctuation that shapes generics, arrays and nullables survives: `"List<My-Type>"`
becomes `List<MyType>` and `"int?"` is left alone.

## Files with several types

```csharp
GeneratableFile file = SourceGen.NewFile()
    .WithHeaderComment("// Auto-generated. Do not edit.")
    .Containing(controlSchemeEnum, controlSchemeExtensions);

SourceGen.WriteToPath("Assets/MyGame.Generated/ControlScheme.cs", file);
```

Contained types may sit in different namespaces; using directives are collected from all of them and
hoisted to the top of the file.

## Writing

`SourceGen.WriteToPath` returns whether the file was written, was already up to date, or failed. A file
whose contents have not changed is left alone, so regenerating does not force Unity to reimport and
reload the domain.

## Modifying an existing script

When only part of a file is generated, edit it in place instead:

```csharp
SourceGen.GetScriptToModify<MyClass>()
    .WithDirective("UnityEngine")
    .WithCodeChunkInRegion("Generated", chunk, replaceExistingCodeInRegion: true)
    .ExecuteModification(refreshAssets: true);
```
