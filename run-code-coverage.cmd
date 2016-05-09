.\packages\OpenCover.4.6.519\tools\OpenCover.Console.exe -register:user "-target:.\packages\NUnit.ConsoleRunner.3.2.1\tools\nunit3-console.exe" "-targetargs: .\Tests\bin\Debug\Tests.dll"

.\packages\ReportGenerator.2.4.5.0\tools\ReportGenerator.exe "-reports:results.xml" "-targetdir:.\coverage"

pause