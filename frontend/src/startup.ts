// Everything defined in this class runs at the startup of the application, after the store is initialized
// Initialization code goes here
import Axios, { AxiosError } from 'axios';
import _ from 'lodash';
import { AccessInfo } from 'MyModels';
import { Store } from 'redux';
import { refreshToken } from './features/auth/reducer';
import { RootState } from './store';

export default function configure(store: Store) {
   [new AxiosService()].forEach((x) => x.configure(store));
}

interface IServiceConfigurer {
   configure(store: Store): void;
}

/**
 * Synchronize the Axios default headers with the Redux store and issue a refresh token action
 * if the credentials expire
 */
class AxiosService implements IServiceConfigurer {
   private access: AccessInfo | null = null;
   private isRefreshingAccess = false;
   private requestsAwaitingAccess = new Array<{ resolve: () => void; reject: () => void }>();

   public configure(store: Store): void {
      (store as any).subscribe(() => {
         const state: RootState = store.getState();

         const token = state.auth.token;

         // check if we actually
         if (_.isEqual(state.auth.token, this.access)) {
            return;
         }

         this.access = token;
         this.updateToken();
      });

      Axios.interceptors.response.use(
         (succeeded) => succeeded,
         (error: AxiosError) => {
            const { config, response } = error;
            if (!response || !config.url) {
               return Promise.reject(error);
            }

            if (config.url.endsWith('auth/refreshtoken')) {
               this.requestsAwaitingAccess.forEach((x) => x.reject());
               this.requestsAwaitingAccess = [];
               this.isRefreshingAccess = false;
               this.access = null;
               this.updateToken();

               return Promise.reject(error);
            }

            const { status } = response;
            if (status === 401 && this.access !== null) {
               if (!this.isRefreshingAccess) {
                  this.isRefreshingAccess = true;
                  store.dispatch(refreshToken(this.access) as any);
               }

               const retryRequest = new Promise((resolve, reject) => {
                  this.requestsAwaitingAccess.push({
                     resolve: () => {
                        // when we update the defaults, this request is not affected
                        config.headers.Authorization = Axios.defaults.headers.common.Authorization;
                        resolve(Axios(config));
                     },
                     reject: () => reject(error),
                  });
               });
               return retryRequest;
            }

            return Promise.reject(error);
         },
      );
   }

   protected updateToken() {
      if (!this.access) {
         // sign out
         delete Axios.defaults.headers.common;
         return;
      }

      Axios.defaults.headers.common = {
         Authorization: `Bearer ${this.access.accessToken}`,
      };

      this.requestsAwaitingAccess.forEach((x) => x.resolve());
      this.requestsAwaitingAccess = [];
      this.isRefreshingAccess = false;
   }
}
