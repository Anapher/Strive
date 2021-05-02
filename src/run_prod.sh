export GITCOMMIT=$(git rev-parse --short HEAD)
export GITREF="$(git log -1 --pretty=format:"%D")"
export GITTIMESTAMP="$(git log -1 --pretty=format:"%ai")"

export ANNOUNCED_IP=127.0.0.1
export SITE_HOST=yourdomain.com
export FRONTEND_DNS_OR_IP=www.yourdomain.com
export STRIVE_TOKEN_SECRET=fill_in_random_token
export STRIVE_API_KEY=fill_in_random_token

export MEDIASOUP_MIN_PORT=40000 # Minimun RTC port for ICE, DTLS, RTP, etc.
export MEDIASOUP_MAX_PORT=49999 # Maximum RTC port for ICE, DTLS, RTP, etc.

docker-compose -f docker-compose.yml -f docker-compose.override.yml -f docker-compose.traefik.yml -f docker-compose.prod.yml up --build
