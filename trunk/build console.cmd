rem Rebuild
dotnet clean -c Release
dotnet build -c Release

rem Build ZIP file for .NET 4.5.2
del /q PassphraseGenerator.net452.zip
zip.exe -9j PassphraseGenerator.net452.zip PassphraseGenerator\bin\Release\net452\*.* 
zip.exe -u PassphraseGenerator.net452.zip LICENSE.txt NOTICE.txt README.txt

rem Publish for .NET Core 2.1
dotnet publish PassphraseGenerator -c Release -f netcoreapp2.1 

rem Build ZIP file for .NET Core 2.1
del /q PassphraseGenerator.netcoreapp21.zip
zip.exe -9j PassphraseGenerator.netcoreapp21.zip PassphraseGenerator\bin\Release\netcoreapp2.1\publish\*.* 
zip.exe -u PassphraseGenerator.netcoreapp21.zip LICENSE.txt NOTICE.txt

pause