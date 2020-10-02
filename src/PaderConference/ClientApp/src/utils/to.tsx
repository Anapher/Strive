import { LocationDescriptor } from 'history';
import { Link } from 'react-router-dom';

/**
 * transform a component to a router link. Usage example: `<Link {...to("/")} />`
 * @param location the location url that should be navigated to
 */
const to = (location: LocationDescriptor) => ({ to: location, component: Link as any });

export default to;
