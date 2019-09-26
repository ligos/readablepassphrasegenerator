rem Rebuild
dotnet clean -c Release
dotnet build -c Release

rem Clean the build folder
rmdir /s /q "ReadablePassphrase.build"
mkdir "ReadablePassphrase.build"

rem Ensure dictionary is up to date.
cd MergePartsOfSpeech\bin\Release\net471
MergePartsOfSpeech.exe
copy /y dictionary.xml "../../../../KeePassReadablePassphrase/dictionary.xml"
cd ../../../..

rem Copy required files to build folder.
copy /y "KeePassReadablePassphrase\*.*"  "ReadablePassphrase.build"
copy /y "ReadablePassphrase.Core\bin\Release\net40\ReadablePassphrase.Core.dll" "ReadablePassphrase.build"
copy /y "ReadablePassphrase.Core\bin\Release\net40\ReadablePassphrase.Core.pdb" "ReadablePassphrase.build"
copy /y "ReadablePassphrase.Core\bin\Release\net40\ReadablePassphrase.Words.dll" "ReadablePassphrase.build"
copy /y "ReadablePassphrase.Core\bin\Release\net40\ReadablePassphrase.Words.pdb" "ReadablePassphrase.build"

copy /y LICENSE.txt ReadablePassphrase.build
copy /y NOTICE.txt ReadablePassphrase.build

rem Replace the new Microsoft.NET.Sdk based project file with the old one, because KeePass only supports the old one.
del /q ReadablePassphrase.build\KeePassReadablePassphrase.csproj
move "ReadablePassphrase.build\KeePassReadablePassphrase.csproj.old" "ReadablePassphrase.build\KeePassReadablePassphrase.csproj"

rem Build the plgx file!
"%ProgramFiles(x86)%\KeePass Password Safe 2\KeePass.exe"  --plgx-create "%CD%\ReadablePassphrase.build" --plgx-prereq-net:4.0 --plgx-prereq-kp:2.36 

move /y "ReadablePassphrase.build.plgx" "ReadablePassphrase.plgx"
pause