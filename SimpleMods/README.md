# A simple Mod for Hollow Knight

We may abbreviate **Hollow Knight** as **HK** in the following discussion.

## Environment Dependencies

1. We are based on MacOS system.

2. You use *.NET*, which means you should download DotNet (from https://dotnet.microsoft.com/zh-cn/download), then you can use *dotnet build* command. 

3. Install Hollow Knight game.

4. We need **Modding API** (https://github.com/hk-modding/api), which provides some APIs to build further mods, preparing us with a suitable developement environment. There are a few ways to achieve this.
   -  I have tried this method but it seems unable to work (resulting in failure to open the *HK* app): download the released package, unzip it, and use the content to substitute files in `hollow_knight.app/Contents/Resources/Data/Managed/`.
   -  Download **Scarab** (a mod manager, from https://github.com/fifty-six/Scarab), which should implicitly overwrite some files under the `Managed/` folder.
   -  You can refer to https://prashantmohta.github.io/ModdingDocs/getting-started.html for further information.
  


## Usage 

To compile this project, use 

```shell 
dotnet build ProjectFileForDotnet.csproj
```

After that, you will get `MyFirstMod.dll` in `bin/Debug/net472/`. Copy this file to `/$Your Path$/Hollow Knight/hollow_knight.app/Contents/Resources/Data/Managed/Mods/MyFirstMod/`. You may need to create a folder named `MyFirstMod`.

Then run the game and check if the newly added mod works fine.

## Notes

1. Some essential dll files are put under 
   ```~/Library/Application Support/Steam/steamapps/common/Hollow Knight/hollow_knight.app/Contents/Resources/Data/Managed/```.

2. The logging file for All mods is on ```~/Library/Application Support/unity.Team Cherry.Hollow Knight/ModLog.txt```.

3. To enable directly demonstrating logging info in the game, you need to modify ```~/Library/Application Support/unity.Team Cherry.Hollow Knight/ModdingApi.GlobalSettings.json```. To be specific, change the value of following item from `false` to `true`.

```json 
  "ShowDebugLogInGame": true,
```

4. *(to be further investigated)* Simply quitting the HK app and removing some Mod files in `Managed/Mods` may not disable the already loaded mods. If that happens, try close and reopen Steam app, then HK app. 

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
  <PropertyGroup>
    <TargetFramework>net472</TargetFramework>
    <OutputType>Library</OutputType>
    <LangVersion>latest</LangVersion>
    <AssemblyName>YourModName</AssemblyName> <!-- This is the name of the output DLL -->
    <RootNamespace>MyMods</RootNamespace> <!-- if there is already a namespace specified in .cs file, 
                                                   this line doesn't work -->
    <OutputPath>bin\$(Configuration)\</OutputPath>
    <EnableDefaultCompileItems>false</EnableDefaultCompileItems>
  </PropertyGroup>
```