# ASP.Net Core & React Starter

A basic implementation of ASP.Net Core as backend with a React Single Page Application as frontend. The login process is completely implemented (including front end). This project can be used a starter.
My experience has shown that it's sometimes really nice to have real world example about how an application can be structured and implemented. Lot's of tutorials are written fairly abstract without actual examples that respect all the challanges you will face. This project should by no means be an allrounder, but I think it covers a big part of what most applications need. From my experience, this is the ideal project structure with the best libraries, but I'm always open to criticism and discussion.

## Which problems does it solve?

#### General

- Build Task: Use [Fake](https://fake.build/) to build the project and execute tests which can be executed locally aswell as by your CI
- Deployment: Dockerfile to create a Docker image for your application that can be executed in every environment
- Best Practices and popular patterns to create a maintable and modern structure
- State of art implementations of all used frameworks/libraries, using the most recent updates
- Domain Driven Design

#### Frontend (React)

- TypeScript for a strongly typed code base
- [typesafe-actions](https://github.com/piotrwitek/typesafe-actions) to strongly type the Redux store, reducers and actions
- Allow absolute imports using `src/store/....` (the `src` prefix, see discussion [here](https://github.com/facebook/create-react-app/issues/5118))
- [Material-UI](https://material-ui.com/) as design framework
- [Axios](https://github.com/axios/axios) for HTTP requests
- [TsLint](https://palantir.github.io/tslint/) and [Prettier](https://prettier.io/) are preconfigured
- [Formik](https://github.com/jaredpalmer/formik) together with [redux-promise-listener](https://github.com/erikras/redux-promise-listener) for easy forms (with custom typings and a React Hook)

#### Backend (ASP.Net Core)

- C# 8 with nullables
- Swagger API
- [Serilog](https://github.com/serilog/serilog) for structured logs
- [Autofac](https://autofac.org/) for fast dependency injection
- [AutoMapper](https://automapper.org/) for object mapping (e. g. entities to dtos)
- [Ef Core](https://docs.microsoft.com/en-us/ef/core/) as ORM

#### Authentication & Identity

- [ASP.Net Core Identity](https://docs.microsoft.com/en-us/aspnet/core/security/authentication/identity?view=aspnetcore-2.2&tabs=visual-studio) with BCrypt password hasing
- Authentication is implemented using JWT and refresh tokens
- The front end automatically refreshes the JWT if a refresh token is available (using Axios interceptors)

#### Realtime communication

- [SignalR](https://docs.microsoft.com/en-us/aspnet/core/signalr/introduction?view=aspnetcore-2.2)
- in front end, it is directly connected to the redux store, hooks are included that subscribe/unsubscribe to events if the component is loaded

#### Errors

- Provide a clean and defined way to transfer errors from the backend business logic to the React ui
- Every error has a code, a message and a type
- The ui has easy ways to process and show the error

#### Tests

- Unit Tests are implemented using [XUnit](https://xunit.net/)
- Mocks are created using [Moq](https://github.com/moq/moq4)
- Integration tests that can be debugged

## Initialize

Build with [Fake](https://fake.build/)

```
fake build
```

In root lies a script (/rename-solution.ps1) that will rename `AwesomeAspCore` to the name of your application. Just run it and it will prompt you asking for the new name.

## Sources

- https://fullstackmark.com/post/19/jwt-authentication-flow-with-refresh-tokens-in-aspnet-core-web-api
- https://github.com/piotrwitek/react-redux-typescript-guide
- https://github.com/zkavtaskin/Domain-Driven-Design-Example
