# A simple Mod for Hollow Knight

## Environment Dependencies

1. We are based on MacOS system.

2. We need *Mono* (`brew install mono`). Then we are able to use the *xbuild* command.

3. Install Hollow Knight game.

## Usage 

To compile this project, use either 

```shell 
xbuild ProjectFile.csproj 
```

or 

```shell 
dotnet build ProjectFileForDotnet.csproj
```

After that, you will get `MyFirstMod.dll` in `bin/Debug/net472/`. Copy this file to `/$Your Path$/Hollow Knight/hollow_knight.app/Contents/Resources/Data/Managed/Mods/MyFirstMod/`. You may need to create a folder named `MyFirstMod`.

Then run the game and check if the newly added mod works fine.

## Notes

We **assume in the following** that operating system is **MacOS**.

- Some essential dll files are put under `~/Library/Application Support/Steam/steamapps/common/Hollow Knight/hollow_knight.app/Contents/Resources/Data/Managed/`.

- The logging file for All mods is on `~/Library/Application Support/unity.Team Cherry.Hollow Knight/ModLog.txt`.

### Essential dll reference

```xml
<!-- 核心游戏 DLL -->
<Reference Include="Assembly-CSharp.dll" />
<Reference Include="UnityEngine.dll" />

<!-- Unity 核心模块 -->
<Reference Include="UnityEngine.CoreModule.dll" />

<!-- 输入系统 -->
<Reference Include="UnityEngine.InputModule.dll" />
<Reference Include="UnityEngine.InputLegacyModule.dll" />

<!-- UI 系统 (如果使用 UI 功能) -->
<Reference Include="UnityEngine.UI.dll" />
<Reference Include="UnityEngine.UIModule.dll" />
<Reference Include="UnityEngine.TextRenderingModule.dll" />

<!-- 基础类型 -->
<Reference Include="netstandard.dll" />

```

### Project Configuration

```xml
 <Target Name="Build">
    <Csc Sources="@(Compile)" References="@(Reference)" OutputAssembly="$(OutputPath)$(AssemblyTitle).dll" TargetType="Library" />
  </Target>
```