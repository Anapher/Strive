import Logger from './utils/logger';
import * as mediasoup from 'mediasoup';
import { WorkerSettings, Worker } from 'mediasoup/lib/types';

const logger = new Logger();

export default class MediaSoupWorkers {
   private workers: Worker[] = [];
   private nextWorkerId = 0;

   async run(numWorkers: number, settings: WorkerSettings): Promise<void> {
      logger.info('running %d mediasoup Workers...', numWorkers);

      for (let i = 0; i < numWorkers; ++i) {
         const worker = await mediasoup.createWorker(settings);

         worker.on('died', () => {
            logger.error('mediasoup Worker died, exiting in 2 seconds... [pid:%d]', worker.pid);

            setTimeout(() => process.exit(1), 2000);
         });

         this.workers.push(worker);

         // Log worker resource usage every X seconds.
         //   setInterval(async () => {
         //      const usage = await worker.getResourceUsage();

         //      logger.info('mediasoup Worker resource usage [pid:%d]: %o', worker.pid, usage);
         //   }, 120000);
      }

      logger.info('Mediasoup workers started');
   }

   getNextWorker(): Worker {
      if (this.workers.length === 0) throw new Error('Please execute run() first');

      const worker = this.workers[this.nextWorkerId];
      if (++this.nextWorkerId === this.workers.length) this.nextWorkerId = 0;

      return worker;
   }

   close(): void {
      for (const worker of this.workers) {
         worker.close();
      }

      this.workers = [];
   }
}
