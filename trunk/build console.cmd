rem Rebuild
dotnet clean -c Release
dotnet build -c Release

rem Build ZIP file for .NET 4.5.2
del /q PassphraseGenerator.net452.zip
copy /y ..\LICENSE.txt PassphraseGenerator\bin\Release\net452\LICENSE.txt
copy /y ..\NOTICE.txt PassphraseGenerator\bin\Release\net452\NOTICE.txt
copy /y README.txt PassphraseGenerator\bin\Release\net452\README.txt
zip.exe -9j PassphraseGenerator.net452.zip PassphraseGenerator\bin\Release\net452\*.* 


rem Publish for .NET Core 3.1
dotnet publish PassphraseGenerator -c Release -f netcoreapp3.1 

rem Build ZIP file for .NET Core 3.1
del /q PassphraseGenerator.netcoreapp31.zip
copy /y ..\LICENSE.txt PassphraseGenerator\bin\Release\netcoreapp3.1\publish\LICENSE.txt
copy /y ..\NOTICE.txt PassphraseGenerator\bin\Release\netcoreapp3.1\publish\NOTICE.txt
copy /y README.txt PassphraseGenerator\bin\Release\netcoreapp3.1\publish\README.txt
zip.exe -9j PassphraseGenerator.netcoreapp31.zip PassphraseGenerator\bin\Release\netcoreapp3.1\publish\*.* 


rem Publish for .NET 6.0
dotnet publish PassphraseGenerator -c Release -f net60

rem Build ZIP file for .NET 6.0
del /q PassphraseGenerator.net60.zip
copy /y ..\LICENSE.txt PassphraseGenerator\bin\Release\net60\publish\LICENSE.txt
copy /y ..\NOTICE.txt PassphraseGenerator\bin\Release\net60\publish\NOTICE.txt
copy /y README.txt PassphraseGenerator\bin\Release\net60\publish\README.txt
zip.exe -9j PassphraseGenerator.net60.zip PassphraseGenerator\bin\Release\net60\publish\*.* 


pause