set PORT=59402

pushd ..\

start "" http://localhost:%PORT%

docfx metadata docfx.json
docfx build docfx.json --serve --port %PORT%