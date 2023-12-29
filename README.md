# Readable Passphrase Generator #

The Readable Passphrase Generator generates passphrases which are (mostly) grammatically correct but nonsensical.
These are easy to remember (for humans) but difficult to guess (for humans and computers). 

Developed in C# with a KeePass plugin, console app and public API.
Runs wherever the .NET Framework, .NET Core or Mono are available.

--- 

See [MakeMeAPassword](https://makemeapassword.ligos.net/generate/ReadablePassphrase) to generate readable passphrases online (without KeePass).
Or [Steven Zeck's Javascript port](https://saintly.zeck.net/readablepassphrase/) (runs entirely in your browser).

**[Click here for step-by-step instructions to install the KeePass plugin](https://github.com/ligos/readablepassphrasegenerator/wiki/KeePass-Plugin-Step-By-Step-Guide).**

**[Download KeePass plugin or console app](https://github.com/ligos/readablepassphrasegenerator/releases)**

**[Developers can install from NuGet](https://www.nuget.org/packages/ReadablePassphrase/)** ([and see the API](https://github.com/ligos/readablepassphrasegenerator/wiki/Public-API))


Use [Scoop](https://scoop.sh/) to install (from [scoop-extras](https://github.com/lukesampson/scoop-extras/blob/master/bucket/keepass-plugin-readable-passphrase.json)):

```
PS> scoop install keepass
PS> scoop install keepass-plugin-readable-passphrase
```

#### Chocolatey 📦 
Or you can [use Chocolatey to install](https://community.chocolatey.org/packages/keepass-plugin-readablepassphrasegen#install) it in a more automated manner:

```
choco install keepass-plugin-readablepassphrasegen
```

To [upgrade KeePass Plugin Readable Passphrase Generator](https://community.chocolatey.org/packages/keepass-plugin-readablepassphrasegen#upgrade) to the [latest release version](https://community.chocolatey.org/packages/keepass-plugin-readablepassphrasegen#versionhistory) for enjoying the newest features, run the following command from the command line or from PowerShell:

```
choco upgrade keepass-plugin-readablepassphrasegen
```

---

If you like the Readable Passphrase Generator you can donate to support development, or just say thanks.

[![Donate $5](https://www.paypalobjects.com/en_AU/i/btn/btn_donate_LG.gif)](https://www.paypal.com/cgi-bin/webscr?cmd=_s-xclick&hosted_button_id=7J8NPZ7MEN9N8)


## Getting Started ##

[Please read the Wiki Homepage for details of using or contributing to the Readable Passphrase Generator](https://github.com/ligos/readablepassphrasegenerator/wiki).

## Recent Changes ##
* Version 1.4.0
	* 18,505 words in the default dictionary (~1,000 more than 1.3.0)
	* Support .NET Framework 4.5.2, .NET 6.0, .NET 8.0.
* Version 1.3.0
	* 17,548 words in the default dictionary (~2,200 more than 1.2.0)
	* 1,455 fake words (from [ThisWordDoesNotExist.com](https://www.thisworddoesnotexist.com/)) with option to exclude fake words
	* Backend word scraper supporting [ThisWordDoesNotExist.com](https://www.thisworddoesnotexist.com/) and [Dictionary.com](https://dictionary.com). Thanks to [drventure](https://github.com/ligos/readablepassphrasegenerator/pull/9).
	* Add option to count length by words and letters.
	* Support .NET Framework 4.5.2, .NET Core 3.1, .NET 6.0.
* Version 1.2.1
    * Fix possible IndexOutOfRangeException with combination of Numeric and Custom mutators ([GitHub issue 3](https://github.com/ligos/readablepassphrasegenerator/issues/3))
    * Fix Numeric and Constant mutators not applied if Upper mutator is disabled ([GitHub issue 2](https://github.com/ligos/readablepassphrasegenerator/issues/2))
* Version 1.2.0
    * Add constant mutator to improve chances of meeting password requirements.
	* Fix issue with some custom phrase definitions ([BitBucket issue 15](https://bitbucket.org/ligos/readablepassphrasegenerator/issues/15/custom-phrase-description-unhanded-error))
	* 15,346 words in the default dictionary (~300 more than 1.0.0)
	* Add support for .NET Core 3.1.
	* Migration from BitBucket to GitHub.
	* Add support for C# 8 [nullable reference types](https://docs.microsoft.com/en-us/dotnet/csharp/nullable-references) (for developers).

	
## License

Readable Passphrase Generator is licensed under the [Apache License](https://www.apache.org/licenses/LICENSE-2.0), copyright Murray Grant.

It may be used freely under the terms of the above license. 

Summary: it may be used in any project (commercial or otherwise) as long as you attribute copyright to me somewhere and indicate its licensed under the Apache License.
