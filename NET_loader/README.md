# Maverick.Agent
- is project Console App (.NET framwork 4.8) for developing and testing
# Maverick.Load
- is project Class Library (.NET framework 4.8) for targeting fileless loader .NET
> inhertit Maverick.Agent
> outfile .dll
# Maverick.Template
- is project WinExe (.NET Core 9.0) for targeting file executable malware
> outfile .exe
- can fake Resource and Icon of any legit application
:::spoiler
``ResourceHacker.exe -open target.exe -save temp.rc -action extract -mask ICONGROUP,, -log NUL``
``ResourceHacker.exe -open target.exe -save loader.rc -action extract -mask VERSIONINFO,, -log NUL``
- To fix: add .ico file into .rc file (see example loader.res)
``ResourceHacker.exe -open loader.rc -save loader.res -action compile -log NUL``
:::