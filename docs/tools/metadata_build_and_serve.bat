set PORT=5941

pushd ..\

start "" http://localhost:%PORT%

docfx metadata docfx.json
docfx build docfx.json --serve --port %PORT%