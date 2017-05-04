# Readable Passphrase Generator #

The Readable Passphrase Generator generates passphrases which are (mostly) grammatically correct but nonsensical.
These are easy to remember (for humans) but difficult to guess (for humans and computers). 

Developed in C# with a KeePass plugin, console app and public API.
Runs wherever .NET or Mono is available.

--- 

See [MakeMeAPassword](https://makemeapassword.org/generate/ReadablePassphrase) to generate readable passphrases online (without KeePass).

**[Click here for step-by-step instructions to install the KeePass plugin](https://bitbucket.org/ligos/readablepassphrasegenerator/wiki/KeePass-Plugin-Step-By-Step-Guide).**

**[Download KeePass plugin or console app](https://bitbucket.org/ligos/readablepassphrasegenerator/downloads/)**

---

If you like the Readable Passphrase Generator you can donate to support development, or just say thanks.

[![Donate $5](https://www.paypalobjects.com/en_AU/i/btn/btn_donate_LG.gif)](https://www.paypal.com/cgi-bin/webscr?cmd=_s-xclick&hosted_button_id=7J8NPZ7MEN9N8)


## Getting Started ##

[Please read the Wiki Homepage for details of using or contributing to the Readable Passphrase Generator](https://bitbucket.org/ligos/readablepassphrasegenerator/wiki/Home).

## Recent Changes ##
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
* Version 0.16
	* Fix cases where custom phrase definitions can cause exceptions ([issue](https://readablepassphrase.codeplex.com/discussions/647967)). 
	* 14,171 words in the default dictionary (~25 more than 0.14 release).
* Version 0.15
	* Add support for an arbitrary delimiter between words ([issue](https://readablepassphrase.codeplex.com/workitem/22)). 
	* 14,147 words in the default dictionary (~500 more than 0.14 release).
* Version 0.14.1
	* Fixed a bug which can cause a crash with the upper case whole word mutator. 
* Version 0.14.0
	* Added additional uppercase "[mutators](https://bitbucket.org/ligos/readablepassphrasegenerator/wiki/Complying-with-Complexity-Rules-(Mutators))" which make whole words and sequences (or runs) of letters uppercase.
	* Added another numeric "[mutators](https://bitbucket.org/ligos/readablepassphrasegenerator/wiki/Complying-with-Complexity-Rules-(Mutators))" which adds numbers at the end of the passphrase.
	* Fixed mutators so they work correctly when no spaces in a passphrase (eg: when making a WiFi passphrase)
	* 13,580 words in the default dictionary (~400 more than previous release).

## License

Readable Passphrase Generator is licensed under the [Apache License](https://www.apache.org/licenses/LICENSE-2.0), copyright Murray Grant.

It may be used freely under the terms of the above license. 

Summary: it may be used in any project (commercial or otherwise) as long as you attribute copyright to me somewhere and indicate its licensed under the Apache License.
