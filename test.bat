@echo off

set dir="cluster"

rem start dotnet run -p %dir%\HF.Samples.ServerNode nodeA -q order -w 100

rem start dotnet run -p %dir%\HF.Samples.ServerNode nodeB -q storage -w 100

start cmd /k "cd %dir%\HF.Samples.Console && dotnet run"

rem start cmd /k "cd %dir%\HF.Samples.APIs && dotnet run"

rem start dotnet run -p %dir%\HF.Samples.Consumer