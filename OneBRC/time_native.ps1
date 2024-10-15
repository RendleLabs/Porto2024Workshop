# dotnet publish -c Release -o ./native ./src/OneBRC/

$sw = [System.Diagnostics.Stopwatch]::StartNew()

./native/OneBRC.exe ./billion.txt

$sw.Stop()
$sw.Elapsed
