# Minecraft Tab Patcher
This tool fixes a bug in Minecraft 1.13 and 1.14 where you are unable to close the inventory if you use 'tab' as your inventory key.

## Why should i use this tool?
This tool is only for people who want to use the 'tab' key to open and close the inventory in Minecraft. Since Minecraft 1.13 you can no longer close the inventory with 'tab'. The game will focus the new recipe book button instead. 

The game will not check your keyboard bindings if there is a system key with higher priority. This tool will disable 'tab' as a system key in Minecraft.

**This comes with a trade off: As saied: You won't be able to use 'tab' to switch to the next ui element anymore. And i mean in every screen in the entire application!**

### Couldn't this be a mod too?
The tools to create Minecraft 1.13 or 1.14 mods haven't been released yet. I failed to update MCP *(Mod Coder Pack)* by hand. So I created my own patcher to fix the bug in any version of Minecraft. Even in other mods! You can also patch an OptiFine or a Forge version. Try it out!

## How does it work?
1. Download and extract the tool. 
2. Run *MinecraftTabPatcher.exe*.
3. Read the instructions carefully and press any key to continue.
4. Select the base Minecraft version you want to patch.
5. The tool will create a new Minecraft version. Wait.
6. Start the Minecraft launcher.
7. Create a new profile.
8. Choose the new created version and save the profile.
9. Now run your new profile in the luncher.
10. Bind your inventory to 'tab'.
11. Start a world.
12. Open and close your inventory by pressing the 'tab' key. It will work!

## Requirements

* Minecraft *(obviously)*
* [.NET Core Runtime 2.1](https://dotnet.microsoft.com/download/dotnet-core/2.1)

## Build from source
* Install [.NET Core SDK 2.1](https://dotnet.microsoft.com/download/dotnet-core/2.1)
* Run ```dotnet restore```
* Run ```dotnet publish -c Release -r win-x86 --self-contained false``` to build a 32bit app
* Run ```dotnet publish -c Release -r win-x64 --self-contained false``` to build a 64bit app
* Remove ```--self-contained false``` to create a portable app. You won't need the *.NET Core Runtime* anymore but it will generate a much larger app.

