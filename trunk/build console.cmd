rem Rebuild
dotnet clean -c Release
dotnet build -c Release

rem Build ZIP file for .NET 4.5.2
del /q PassphraseGenerator.net452.zip
copy /y ..\LICENSE.txt PassphraseGenerator\bin\Release\net452\LICENSE.txt
copy /y ..\NOTICE.txt PassphraseGenerator\bin\Release\net452\NOTICE.txt
copy /y README.txt PassphraseGenerator\bin\Release\net452\README.txt
zip.exe -9j PassphraseGenerator.net452.zip PassphraseGenerator\bin\Release\net452\*.* 


rem Publish for .NET 6.0
dotnet publish PassphraseGenerator -c Release -f net60

rem Build ZIP file for .NET 6.0
del /q PassphraseGenerator.net60.zip
copy /y ..\LICENSE.txt PassphraseGenerator\bin\Release\net60\publish\LICENSE.txt
copy /y ..\NOTICE.txt PassphraseGenerator\bin\Release\net60\publish\NOTICE.txt
copy /y README.txt PassphraseGenerator\bin\Release\net60\publish\README.txt
zip.exe -9j PassphraseGenerator.net60.zip PassphraseGenerator\bin\Release\net60\publish\*.* 

rem Publish for .NET 8.0
dotnet publish PassphraseGenerator -c Release -f net80

rem Build ZIP file for .NET 8.0
del /q PassphraseGenerator.net80.zip
copy /y ..\LICENSE.txt PassphraseGenerator\bin\Release\net80\publish\LICENSE.txt
copy /y ..\NOTICE.txt PassphraseGenerator\bin\Release\net80\publish\NOTICE.txt
copy /y README.txt PassphraseGenerator\bin\Release\net80\publish\README.txt
zip.exe -9j PassphraseGenerator.net80.zip PassphraseGenerator\bin\Release\net80\publish\*.* 


pause