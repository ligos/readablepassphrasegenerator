# Readable Passphrase Generator #

The Readable Passphrase Generator generates passphrases which are (mostly) grammatically correct but nonsensical.
These are easy to remember (for humans) but difficult to guess (for humans and computers). 

Developed in C# with a KeePass plugin, console app and public API.
Runs wherever the .NET Framework, .NET Core or Mono are available.

--- 

See [MakeMeAPassword](https://makemeapassword.ligos.net/generate/ReadablePassphrase) to generate readable passphrases online (without KeePass).
Or [Steven Zeck's Javascript port](https://saintly.zeck.net/readablepassphrase/) (runs entirely in your browser).

**[Click here for step-by-step instructions to install the KeePass plugin](wiki/KeePass-Plugin-Step-By-Step-Guide).**

**[Download KeePass plugin or console app](releases)**

**[Developers can install from NuGet](https://www.nuget.org/packages/ReadablePassphrase/)** ([and see the API](wiki/Public-API))

---

If you like the Readable Passphrase Generator you can donate to support development, or just say thanks.

[![Donate $5](https://www.paypalobjects.com/en_AU/i/btn/btn_donate_LG.gif)](https://www.paypal.com/cgi-bin/webscr?cmd=_s-xclick&hosted_button_id=7J8NPZ7MEN9N8)


## Getting Started ##

[Please read the Wiki Homepage for details of using or contributing to the Readable Passphrase Generator](wiki).

## Recent Changes ##
* Version 1.1.2
    * Add support for .NET Core.
	* Add NuGet package.
* Version 1.1.1
    * Change update site to [makemeapassword.ligos.net](https://makemeapassword.ligos.net).
	* Add KeyBase site as contact info.
	* Enforce requirement for KeePass 2.36 or newer.
* Version 1.1.0
    * Update to target .NET 4.0.
	* Fix bug where [KeePass plugin crashes when using `&` as a word separator](https://bitbucket.org/ligos/readablepassphrasegenerator/issues/11/crash-when-entering-multiple-characters-in).
* Version 1.0.0
	* Fix bug where None word separator can still include some spaces.
	* Fix bug where Space and None word separators were swapped.
	* Add .NET 4.0 SKU as supported runtime (to fix Mono warning).
	* Migrated all links to BitBucket from Codeplex.
	* 15,020 words in the default dictionary (~900 more than 0.17 release).
* Version 0.17
	* Fix serious [non-random password bug](https://bitbucket.org/ligos/readablepassphrasegenerator/wiki/0.17.0-Fix-for-Non-Random-Passphrases). 
	* All users should upgrade to this version as soon as possible
	* **It is highly recommended to reset any passphrases generated in the last 4 years**
	* Versions of the plugin affected by this bug will crash KeePass 2.36+, you must upgrade to 0.17 or newer.

	
## License

Readable Passphrase Generator is licensed under the [Apache License](https://www.apache.org/licenses/LICENSE-2.0), copyright Murray Grant.

It may be used freely under the terms of the above license. 

Summary: it may be used in any project (commercial or otherwise) as long as you attribute copyright to me somewhere and indicate its licensed under the Apache License.
