# Compiling
- Get the dlls https://umod.org/games/rust/download/develop
- Extract the zip
- Put RustDedicated_Data it in this folder

# Changes in Core
Due to rust loading things weird ChatDirector-Core is copied here with some modifications to work in rust
- ChatDirector takes the raw file as input
- JSON rather than YAML
- Different .NET versions
    - no public in interfaces