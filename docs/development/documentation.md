---
uid: documentation
---

# Documentation

SUGAR's documentation is generated using [DocFX](https://dotnet.github.io/docfx/) using tripple slash code comments and [Swagger](https://swagger.io/) to generate the REST API.

## Building

There are various build scripts in docs/tools to build, copy and serve the docs.

### Requirements

- [DocFX](https://dotnet.github.io/docfx/)

- "docfx" as a command needs to be availabe via the command console for the scripts to work.

- PDF documentation requires [wkhtmltopdf](https://wkhtmltopdf.org/).

### PDF

The PDF was generated by following [this guide](https://dotnet.github.io/docfx/tutorial/walkthrough/walkthrough_generate_pdf.html).