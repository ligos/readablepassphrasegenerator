rem Rebuild
dotnet clean -c Release

rem Build each package.
dotnet pack ReadablePassphrase.Words -c Release --include-symbols /p:RefNugets=True -o ../../releases
dotnet pack ReadablePassphrase.Core -c Release --include-symbols /p:RefNugets=True -o ../../releases
dotnet pack ReadablePassphrase.DefaultDictionary -c Release --include-symbols /p:RefNugets=True -o ../../releases
dotnet pack ReadablePassphrase -c Release --include-symbols /p:RefNugets=True -o ../../releases

pause