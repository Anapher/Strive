import axios, { AxiosRequestConfig, AxiosResponse } from 'axios';
import { RtpCapabilities } from 'mediasoup-client/lib/RtpParameters';
import { SuccessOrError, SuccessOrErrorSucceeded } from 'src/communication-types';
import { SfuConnectionInfo } from 'src/core-hub.types';
import {
   CreateTransportRequest,
   CreateTransportResponse,
   InitializeConnectionRequest,
   ConnectTransportRequest,
   TransportProduceRequest,
   TransportProduceResponse,
   ChangeStreamRequest,
} from './types';

export default class SfuClient {
   constructor(private connectionInfo: SfuConnectionInfo) {}

   public async getRouterCapabilities(): Promise<SuccessOrError<RtpCapabilities>> {
      const response = await axios.get('/router-capabilities', this.getConfig());
      return SfuClient.wrapResponse(response);
   }

   public async initializeConnection(request: InitializeConnectionRequest): Promise<SuccessOrError> {
      const response = await axios.post('/init-connection', request, this.getConfig());
      return SfuClient.wrapResponse(response);
   }

   public async createTransport(request: CreateTransportRequest): Promise<SuccessOrError<CreateTransportResponse>> {
      const response = await axios.post('/create-transport', request, this.getConfig());
      return SfuClient.wrapResponse(response);
   }

   public async connectTransport(request: ConnectTransportRequest): Promise<SuccessOrError> {
      const response = await axios.post('/connect-transport', request, this.getConfig());
      return SfuClient.wrapResponse(response);
   }

   public async transportProduce(request: TransportProduceRequest): Promise<SuccessOrError<TransportProduceResponse>> {
      const response = await axios.post('/transport-produce', request, this.getConfig());
      return SfuClient.wrapResponse(response);
   }

   public async changeStream(request: ChangeStreamRequest): Promise<SuccessOrError> {
      const response = await axios.post('/change-stream', request, this.getConfig());
      return SfuClient.wrapResponse(response);
   }

   private getConfig(): AxiosRequestConfig {
      return {
         baseURL: this.connectionInfo.url,
         headers: { Authorization: `Bearer ${this.connectionInfo.authToken}` },
         validateStatus: null,
      };
   }

   private static wrapResponse<T>(res: AxiosResponse): SuccessOrError<T> {
      if (res.status >= 200 && res.status <= 300) {
         const result: SuccessOrErrorSucceeded<T> = { success: true, response: res.data };
         return result as any;
      }

      return { success: false, error: res.data };
   }
}
