import { ApplicationConfig, provideZoneChangeDetection, importProvidersFrom } from '@angular/core';
import { provideRouter } from '@angular/router';
import { provideHttpClient, withInterceptorsFromDi } from '@angular/common/http';
import { provideClientHydration, withEventReplay } from '@angular/platform-browser';

// NgRx
import { provideStore } from '@ngrx/store';
import { provideEffects } from '@ngrx/effects';
import { provideStoreDevtools } from '@ngrx/store-devtools';

// PrimeNG
import { MessageService, ConfirmationService } from 'primeng/api';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';

import { routes } from './app.routes';
import { reducers, metaReducers } from './store/app.state';
import { AuthEffects } from './store/auth/auth.effects';
import { EscolasEffects } from './features/escolas/store/escolas.effects';
import { environment } from '../environments/environment';

export const appConfig: ApplicationConfig = {
  providers: [
    provideZoneChangeDetection({ eventCoalescing: true }),
    provideRouter(routes),
    provideClientHydration(withEventReplay()),
    provideHttpClient(withInterceptorsFromDi()),
    
    // NgRx Store
    provideStore(reducers, { metaReducers }),
    provideEffects([AuthEffects, EscolasEffects]),
    provideStoreDevtools({
      maxAge: 25,
      logOnly: environment.production,
      autoPause: true,
      trace: false,
      traceLimit: 75
    }),
    
    // PrimeNG Services
    MessageService,
    ConfirmationService,
    
    // Angular Animations
    importProvidersFrom(BrowserAnimationsModule)
  ]
};
