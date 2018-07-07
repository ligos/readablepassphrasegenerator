cd MergePartsOfSpeech\bin\Release\net471
MergePartsOfSpeech.exe
copy /y dictionary.xml "../../../dictionary.xml"
copy /y dictionary.xml "../../../../KeePassReadablePassphrase/dictionary.xml"
copy /y dictionary.xml "../../../../Test/dictionary.xml"
copy /y dictionary.xml "../../../../Generator/dictionary.xml"
copy /y dictionary.xml "../../../../ReasablePassphrase.DefaultDictionary/dictionary.xml"
cd ../../../..
del "Generator\dictionary.xml.gz"
gzip.exe -9 Generator/dictionary.xml
del "ReasablePassphrase.DefaultDictionary\dictionary.xml.gz"
gzip.exe -9 ReasablePassphrase.DefaultDictionary/dictionary.xml

