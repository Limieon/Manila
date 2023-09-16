# Manila
Manila is a Build System written in C# using JavaScript as it's Build Scripts

## History
Originally, Manila was intended to be written in TypeScript and used for C# Applications. This is also the origin of the name Manila, as my goal was to implement the Roslyn Compiler Platform into my Build System.

This goal has been changed to support C++ and other programming langues (as long anyone writes a plugin for it).

As this project was getting more advanced, I tried to bundle the TypeScript Sources into an executeable. This was not really possible because I dynamically loaded modules into my project and I cannot mutate an EXE after it has ben built. Also I had compability errors between CommonJS and ModuleJS.

So I decided to switch to C# after I found out about [ClearScript](https://github.com/microsoft/ClearScript), a JavaScript Engine for C#. 
ClearScript also allows support for other Scripting Languages, but that is something for the future.

The old TypeScript Code can currently be viewed under the old/TS branch

## Features
- **Simplicity**: When it is more work to write Build Scripts then just to compile your code using your Compiler's CLI, a Build System gets pretty useless.  
Manila is designed to be as simple as possible but also handle complex build logic

- **JavaScript**: Build Scripts are written in JavaScript and can do anything they want.  
In the future I also try my best to implement NPM into the BuildSystem so scripts can also use NPM Packages

- **Dependency Management**: Dependencies can be managed by just defining your repositories where the dependencies are stored and tell the Build System to use the dependecy in your build

## Goal
The goal of the build system is to create a simple but flexible API so build scripts can be as simple as just compiling your code, generating source code, writing to websockets, pushing your code to a CI/CD server such as Jenkins, and then running tests on this server.

It should also be possible to get the results of the tests and then do other stuff with them, for example, sending the test results into a channel in Discord.

## Road Map
I started to rewrite this project just a few days ago, so not many features are currently implemented

1. Build a solid Codebase so I can add the more advanced features on top of it
2. Implement basic API features such as printing to console, creating / deleting / moving files
3. Add a Plugin API so the first simple Plugin can be developed
4. Develop a C++ plugin that (at the beginning) uses clang to compile simple C++ projects
