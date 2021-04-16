FROM node:14
WORKDIR /app

COPY package.json .
COPY yarn.lock .
RUN yarn install

COPY . .

RUN yarn build

ARG MEDIASOUP_MIN_PORT
ARG MEDIASOUP_MAX_PORT
ENV MEDIASOUP_LISTEN_IP=0.0.0.0

EXPOSE 3000
EXPOSE 9000
EXPOSE 2000-2020

CMD [ "node", "build/index.js" ]