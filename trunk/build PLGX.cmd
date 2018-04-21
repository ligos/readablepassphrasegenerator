rmdir /s /q "ReadablePassphrase.build"
mkdir "ReadablePassphrase.build"

copy /y "KeePassReadablePassphrase\*.*"  "ReadablePassphrase.build"
copy /y LICENSE.txt ReadablePassphrase.build
copy /y NOTICE.txt ReadablePassphrase.build

"%ProgramFiles(x86)%\KeePass Password Safe 2\KeePass.exe"  --plgx-create "%CD%\ReadablePassphrase.build" --plgx-prereq-net:4.0 --plgx-prereq-kp:2.36 

move /y "ReadablePassphrase.build.plgx" "ReadablePassphrase.plgx"
pause