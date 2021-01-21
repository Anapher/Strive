FROM node:14
WORKDIR /app

COPY package.json .
COPY yarn.lock .
RUN yarn install

COPY . .

RUN yarn build

EXPOSE 8080
CMD [ "node", "build/index.js" ]