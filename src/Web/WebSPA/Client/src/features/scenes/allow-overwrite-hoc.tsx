import { RenderSceneProps, Scene } from './types';

/**
 * If an overwrite value exists, return the overwrite value, else return the initialized component
 * @param Component the component that renders the scene
 * @returns return an instance of the component or the scene that overwrites the component
 */
const allowOverwrite = <S extends Scene>(Component: React.ComponentType<RenderSceneProps<S>>) => {
   return function AllowOverwriteComponent(props: RenderSceneProps<S>) {
      const overwrite = props.next();
      if (overwrite) {
         return <>{overwrite}</>;
      }

      return <Component {...props} />;
   };
};

export default allowOverwrite;
