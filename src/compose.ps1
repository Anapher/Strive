$arg = $args[0]

$env:GITINFO = ((git log -1 --pretty=format:"%d at %ai") | Out-String).Trim()
$env:GITCOMMIT = ((git rev-parse --short HEAD) | Out-String).Trim()

Write-Host "GITINFO=$env:GITINFO"
Write-Host "GITCOMMIT=$env:GITCOMMIT"

$command = "docker-compose -f .\docker-compose.yml -f .\docker-compose.override.yml -f .\docker-compose.dev.yml -f .\docker-compose.traefik.yml $arg"

Write-Host $command
Invoke-Expression $command