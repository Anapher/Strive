# Node TypeScript Starter

## Getting started

1. Clone this repository and open it

```bash
$ git clone https://github.com/HorusGoul/node-ts-starter your-next-project
$ cd your-next-project
```

2. Install dependencies

```bash
$ yarn
```

3. Launch the dev mode

```bash
$ yarn dev
```

4. You can start coding! The entry point is located in `src/index.ts`.

## What's preconfigured?

The intent of this starter is to be really slim so it's not a nightmare to remove or change stuff, that's why there are just a few things preconfigured:

- Babel
- TypeScript
- ESLint
- Prettier
- Husky pre-commit hook that runs ESLint and type checks the code base
- A few npm scripts

## Scripts

- `yarn dev`. Runs the project in dev mode, which means that it won't check types and will restart with every change you make.
- `yarn build`. Compiles the project to the `./dist` folder.
- `yarn typecheck`. Checks the typings of the project. Gets executed before trying to create a new commit but you can also run it manually.
- `yarn start`. Runs the compiled program. Remember to execute `yarn build` before attempting to launch the program.
- `yarn lint`. Runs ESLint. You can append `--fix` in order to fix autofixable issues.

## What to do next

Adapt the configuration to your needs and start coding!
