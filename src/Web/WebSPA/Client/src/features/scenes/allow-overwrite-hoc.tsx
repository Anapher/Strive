import { RenderSceneProps, Scene } from './types';

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
