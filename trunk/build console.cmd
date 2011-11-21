del /q PassphraseGenerator.zip
zip.exe -9j PassphraseGenerator.zip Generator\bin\Release\*.* 
zip.exe -u PassphraseGenerator.zip LICENSE.txt NOTICE.txt
zip.exe PassphraseGenerator.zip -d PassphraseGenerator.vshost.exe* 
pause