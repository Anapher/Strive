https://github.com/DoTheEvo/Traefik-v2-examples

## In Strive/src folder
```bash
touch acme.json && chmod 600 acme.json

```

## Firewall
```bash
sudo apt install ufw
sudo ufw allow ssh comment 'allow SSH connections'
sudo ufw default deny incoming
sudo ufw default allow outgoing comment 'allow all outgoing traffic'
sudo ufw allow 80/tcp comment 'Strive HTTP'
sudo ufw allow 443/tcp comment 'Strive HTTPS'
sudo ufw allow 40000:49999/tcp comment 'Strive WebRTC'
sudo ufw allow 40000:49999/udp comment 'Strive WebRTC'
sudo ufw enable
```

`sudo ufw status verbose`