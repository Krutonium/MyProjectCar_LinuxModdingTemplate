# MyProjectCar

A project template for creating mods for My Summer/Winter Car using MSCLoader framework.

## Description

MyProjectCar is a structured template designed to streamline the development of mods for My Summer and Winter Car. It provides a
pre-configured project structure with essential files and boilerplate code to help you get started with mod development
quickly.

## Features

- Pre-configured MSCLoader mod template
- Ready-to-use entry point with essential mod metadata
- Organized project structure
- .gitignore configured for C# projects
- Supports both Windows and Linux platforms (but Intended for Linux)

## Requirements

- .NET Framework or compatible runtime
- MSCLoader framework
- My Summer/Winter Car game
- C# development environment (Visual Studio, Rider, or VS Code)

## Installation

`dotnet build -p:Configuration=Release` or `dotnet build -p:Configuration=Debug`  
Or if you just want to run it, `dotnet run`

## Usage

1. Open the project in your preferred C# IDE
2. Implement your mod logic in the `Mod_OnLoad()` method or create additional methods as needed
3. Build the project to generate the mod DLL
4. Copy the compiled DLL to your MSCLoader mods folder
5. Launch My Summer/Winter Car and enable your mod
