import { HubConnection } from '@microsoft/signalr';

export type SignalrHandler = (...args: any[]) => void;
export type HubSubscription = { handler: SignalrHandler; name: string };

/**
 * Registers a handler that will be invoked when the hub method with the specified method name is invoked.
 * Return a subscription object that can be unregistered later.
 * @param connection the Signalr connection
 * @param name The name of the hub method to define.
 * @param handler The handler that will be raised when the hub method is invoked.
 */
export function subscribeEvent(connection: HubConnection, name: string, handler: SignalrHandler): HubSubscription {
   connection.on(name, handler);
   return { name, handler };
}

/**
 * Removes the specified handler for the specified hub method.
 * @param connection the Signalr connection
 * @param subscription the hub subscription to unsubscribe
 */
export function unsubscribeEvent(connection: HubConnection, { name, handler }: HubSubscription): void {
   connection.off(name, handler);
}

export function unsubscribeAll(connection: HubConnection, subscriptions: HubSubscription[]): void {
   for (const sub of subscriptions) {
      unsubscribeEvent(connection, sub);
   }
}
