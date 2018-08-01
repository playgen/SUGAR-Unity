pushd ..\

docfx metadata docfx.json
docfx build docfx.json
docfx pdf docfx.json
pause