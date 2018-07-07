cd MergePartsOfSpeech\bin\Release\net471
MergePartsOfSpeech.exe
copy /y dictionary.xml "../../../dictionary.xml"
copy /y dictionary.xml "../../../../KeePassReadablePassphrase/dictionary.xml"
copy /y dictionary.xml "../../../../Test/dictionary.xml"
copy /y dictionary.xml "../../../../PassphraseGenerator/dictionary.xml"
copy /y dictionary.xml "../../../../ReadablePassphrase.DefaultDictionary/dictionary.xml"
cd ../../../..
del "PassphraseGenerator\dictionary.xml.gz"
gzip.exe -9 PassphraseGenerator/dictionary.xml
del "ReadablePassphrase.DefaultDictionary\dictionary.xml.gz"
gzip.exe -9 ReadablePassphrase.DefaultDictionary/dictionary.xml

