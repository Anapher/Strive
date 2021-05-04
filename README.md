[![ASP.NET Core](https://github.com/Anapher/Strive/actions/workflows/asp-net-core-test.yml/badge.svg)](https://github.com/Anapher/Strive/actions/workflows/asp-net-core-test.yml)
[![codecov](https://codecov.io/gh/Anapher/Strive/branch/develop/graph/badge.svg?token=G074V29MMN)](https://codecov.io/gh/Anapher/Strive)
[![Maintainability](https://api.codeclimate.com/v1/badges/8b02320c4149952fe1c5/maintainability)](https://codeclimate.com/github/Anapher/Strive/maintainability)
[![License: Apache 2.0](https://img.shields.io/badge/License-Apache%202.0-blue.svg)](./LICENSE)

<br />
<p align="center">
  <h1 align="center">Strive</h1>

  <p align="center">
    Open source video conference system
    <br />
    <br />
    <a href="https://demo.openstrive.org/">View Demo</a>
    ·
    <a href="https://github.com/Anapher/Strive/issues">Report Bug</a>
    ·
    <a href="https://github.com/Anapher/Strive/issues">Request Feature</a>
  </p>
</p>



## Table of content

- [About the project](#about-the-project)
    - [Architecture](#architecture)
- [Getting Started](#getting-started)
    - [Running on localhost with Docker Compose and Traefik](#running-on-localhost-with-docker-compose-and-traefik)
    - [Running on production server](#running-on-production-server)
    - [Development](#development)
- [Contributing](#contributing)
- [License](#license)
- [Acknowledgements](#acknowledgements)


## About The Project
Last year when I was tutor at my university, I had to use BigBlueButton which was a very frustrating experience. I wondered how hard it can be to create a video conference system and that's how Strive was started. You can find a list with issues of BBB I wanted to address [here](./advantages_over_bbb.md).

### Architecture
![Architecture](./img/architecture.png)

#### WebSPA
The frontend, written with React/TypeScript. Basically everything you see.

#### Identity Microservice
Manages the authentication process. This is basically an [OpenID Connect](https://openid.net/connect/) server that provides the frontend for the login aswell as managing the access/refresh tokens.

#### Selective Forwarding Unit (SFU)
Redirects the media traffic (audio/video) between the participants.

#### Conference Management
This is the heart of Strive, here are conferences created, chat messages delivered, rooms created etc.

<!-- GETTING STARTED -->
## Getting Started

### Running on localhost with Docker Compose and Traefik
1. Clone the repo
   ```sh
   git clone https://github.com/Anapher/Strive.git
   ```
   
2. Got to src directory
   ```sh
   cd src/
   ```
3. Execute docker compose using the script
   - On Windows, execute
   ```powershell
   ./compose.ps1 up --build
   ```
   - On Linux, execute
   ```sh
   chmod +x ./compose.sh && ./compose.sh up --build
   ```
4. Go to `https://localhost` using your favorite browser. Please note that you may have to trust the self signed certificate for localhost. If you are using Google Chrome, you can simply enable this option [chrome://flags/#allow-insecure-localhost](chrome://flags/#allow-insecure-localhost)
   
In the `.env` file you can change some parameters, but they are already preconfigured for local testing.

### Running on production server
Please refer to the [installation instructions](./installation.md).

### Development
For developing, you likely want to focus on one microservice only. First of all, you need to setup the infrastructure, namely a RabbitMQ server (with delayed message exchange plugin) and a MongoDB database, both running on default ports.
The easiest way to do so is by executing
   ```sh
   docker-compose -f docker-compose.yml -f docker-compose.override.yml up nosqldata rabbitmq
   ```
   
Then you need to start the required microservices you do NOT want to modify:
- Identity microservice (runs on `http://localhost:55105`):
   ```sh
   cd src/Services/Identity/Identity.API && dotnet run
   ```
- WebSPA (runs on `http://localhost:55103`):
   ```sh
   cd src/Web/WebSPA && dotnet run
   ```
  Do not use yarn start in the ClientApp folder, as the ASP.Net Core app injects some parameters required for the frontend to work. Fast refresh will still work in the React app.
- SFU (runs on `http://localhost:3000`)
   ```sh
   cd src/Services/SFU && yarn dev
   ```
- Conference Management (runs on `http://localhost:55104`)
   ```sh
   cd src/Services/ConferenceManagement/Strive && dotnet run
   ```
Do not change these ports as they are configured to work together in a local development environment. You can then attach a debugger to the microservice you want to change. For ASP.Net Core projects, instead of executing this command you may also open the solution with Visual Studio and run the debugger here.

<!-- CONTRIBUTING -->
## Contributing

Contributions are what make the open source community such an amazing place to be learn, inspire, and create. Any contributions you make are **greatly appreciated**.

1. Fork the Project
2. Create your Feature Branch (`git checkout -b feature/AmazingFeature`)
3. Commit your Changes (`git commit -m 'Add some AmazingFeature'`)
4. Push to the Branch (`git push origin feature/AmazingFeature`)
5. Open a Pull Request


<!-- LICENSE -->
## License

Distributed under the Apache-2.0 License. See `LICENSE` for more information.


<!-- ACKNOWLEDGEMENTS -->
## Acknowledgements
* [ASP.Net Core](https://docs.microsoft.com/en-us/aspnet/core)
* [mediasoup](https://mediasoup.org/)
* [Material-UI](https://material-ui.com/)
* [React Hook Form](https://react-hook-form.com/)
* [Duende IdentityServer](https://duendesoftware.com/)
