export type HealthStatus = 'ok' | 'warn' | 'error';
export type ComponentHealth = { status: HealthStatus };

export function mergeHealth(components: (ComponentHealth | undefined)[]): HealthStatus {
   if (components.find((x) => x?.status === 'error')) return 'error';
   if (components.find((x) => x?.status === 'warn')) return 'warn';
   return 'ok';
}
