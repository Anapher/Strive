$arg = $args[0]

$env:GITREF = ((git log -1 --pretty=format:"%D") | Out-String).Trim()
$env:GITCOMMIT = ((git rev-parse --short HEAD) | Out-String).Trim()
$env:GITTIMESTAMP = ((git log -1 --pretty=format:"%ai") | Out-String).Trim()

Write-Host "GITREF=$env:GITREF"
Write-Host "GITCOMMIT=$env:GITCOMMIT"
Write-Host "GITTIMESTAMP=$env:GITTIMESTAMP"

$command = "docker-compose -f .\docker-compose.yml -f .\docker-compose.override.yml -f .\docker-compose.dev.yml -f .\docker-compose.traefik.yml $arg"

Write-Host $command
Invoke-Expression $command