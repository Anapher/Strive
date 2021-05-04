export GITCOMMIT=$(git rev-parse --short HEAD)
export GITREF="$(git log -1 --pretty=format:"%D")"
export GITTIMESTAMP="$(git log -1 --pretty=format:"%ai")"

echo "GITREF=$GITREF"
echo "GITCOMMIT=$GITCOMMIT"
echo "GITTIMESTAMP=$GITTIMESTAMP"

docker-compose -f docker-compose.yml -f docker-compose.override.yml -f docker-compose.dev.yml -f docker-compose.traefik.yml "$@"