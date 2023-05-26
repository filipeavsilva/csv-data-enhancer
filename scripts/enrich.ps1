param (
    [string]$csv
)

if (!$csv) {
    Write-Host "Please pass a csv file with transactions to enrich as the csv parameter"
    Write-Host " "
    Write-Host "Usage: "
    Write-Host "       enrich.ps1 -csv <name of csv file with transactions to enrich>"
    exit 1
}

Invoke-WebRequest -Method POST -ContentType 'text/csv' -Infile $csv -Uri http://localhost:7000/enrich | Select-Object -Expand Content