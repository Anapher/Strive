
$arg = $args[0]

$command = 'docker-compose -f .\docker-compose.yml -f .\docker-compose.override.yml -f .\docker-compose.traefik.yml ' + $arg

Write-Host $command
iex $command