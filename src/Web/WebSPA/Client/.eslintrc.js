module.exports = {
   root: true,
   parser: '@typescript-eslint/parser',
   plugins: ['@typescript-eslint'],
   extends: [
      'eslint:recommended',
      'plugin:react/recommended',
      'plugin:@typescript-eslint/eslint-recommended',
      'plugin:@typescript-eslint/recommended',
      'prettier',
   ],
   parserOptions: {
      ecmaVersion: 2018, // Allows for the parsing of modern ECMAScript features
      sourceType: 'module', // Allows for the use of imports
   },
   settings: {
      react: {
         version: 'detect', // Tells eslint-plugin-react to automatically detect the version of React to use
      },
   },
   rules: {
      // disable the rule for all files
      '@typescript-eslint/explicit-module-boundary-types': 'off',
      'react/prop-types': 'off',
      '@typescript-eslint/no-explicit-any': 'off',
      'react/react-in-jsx-scope': 'off',
      '@typescript-eslint/no-non-null-assertion': 'off',
   },
};
