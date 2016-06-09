@echo off
set o="%cd%\Application\"
rd /S /Q %o%
md %o%
md %o%\Debug
md %o%\Release
xcopy /S /D /Y /exclude:uncopy.txt "%cd%\AccessManager\bin" %o%
xcopy /S /D /Y /exclude:uncopy.txt "%cd%\DnsBlackList\bin" %o%
xcopy /S /D /Y /exclude:uncopy.txt "%cd%\LocalXmlAPI\bin" %o%
xcopy /S /D /Y /exclude:uncopy.txt "%cd%\MailServerLauncher\bin" %o%
xcopy /S /D /Y /exclude:uncopy.txt "%cd%\MailServerManager\bin" %o%
xcopy /S /D /Y /exclude:uncopy.txt "%cd%\MailServerService\bin" %o%
xcopy /S /D /Y /exclude:uncopy.txt "%cd%\VirusScan\bin" %o%
xcopy /S /D /Y /exclude:uncopy.txt "%cd%\dll" %o%\Debug\
xcopy /S /D /Y /exclude:uncopy.txt "%cd%\dll" %o%\Release\
pause