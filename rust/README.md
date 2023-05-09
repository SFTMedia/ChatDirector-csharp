# ChatDirector CSharp
![Discord](https://img.shields.io/discord/113990411063656454)
![.NET Framework Version](https://img.shields.io/badge/.NET%20Framework-4.8-blue)
[![Support us on Patreon](https://img.shields.io/badge/support-patreon-F96854.svg)](https://www.patreon.com/sftmedia)

## Compiling
- Get the dlls https://umod.org/games/rust/download/develop
- Extract the zip
- Put RustDedicated_Data it in this folder

## Changes in Core
Due to rust loading things weird ChatDirector-Core is copied here with some modifications to work in rust
- ChatDirector takes the raw file as input
- JSON rather than YAML
- Different .NET versions
    - no public in interfaces
