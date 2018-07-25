# Documentation
SUGAR Unity's documentation is generated using [DocFX](https://dotnet.github.io/docfx/) using tripple slash code comments and DicFX .md and .toc files located in docs/.

### Requirements
- [DocFX](https://dotnet.github.io/docfx/)
- "docfx" as a command needs to be availabe via the command console for the scripts to work.
- PDF documentation requires [wkhtmltopdf](https://wkhtmltopdf.org/).

### Process

There are various build scripts in docs/tools to build, copy and serve the docs.

Tool | Function 
- | - 
all.bat | Build the docs site and pdf.
copy_to_unity.bat | Copy the built pdf into the unity project.
all_and_copy.bat | all.bat and copy_to_unity.bat
metadata_build_and_serve.bat | Build the site and serve. Use this to test the generated docs.
metadata_pdf.bat | Build the pdf.

Note: The PDF docfx config was created by following [this guide](https://dotnet.github.io/docfx/tutorial/walkthrough/walkthrough_generate_pdf.html).

## Saving Data
SUGAR Unity includes a class which handles saving and retrieving of data, similar to the way in which [Player Prefs](https://docs.unity3d.com/ScriptReference/PlayerPrefs.html) are set, retrieved and deleted. The class uses the following prefix: SUGAR_PREFS_ followed by the key provided.

### Current Usage
Currently the saving and retrieving of data is used for storing player login tokens, allowing for players login details to be saved so they can log in without entering details every time