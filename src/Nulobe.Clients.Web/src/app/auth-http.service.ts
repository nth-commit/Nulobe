import { Injectable, FactoryProvider } from "@angular/core";
import { ConnectionBackend, RequestOptions, Request, RequestOptionsArgs, Response, Http, Headers, XHRBackend } from "@angular/http";
import { Observable } from "rxjs/Rx";

import { NULOBE_ENV_SETTINGS } from './app.settings';
import { AuthService } from './features/auth';

export function authHttpFactory(xhrBackend: XHRBackend, requestOptions: RequestOptions, authService: AuthService): Http {
  return new AuthHttp(xhrBackend, requestOptions, authService);
}

export const authHttpProvider = <FactoryProvider>{
    provide: Http,
    useFactory: authHttpFactory,
    deps: [XHRBackend, RequestOptions, AuthService]
};

@Injectable()
export class AuthHttp extends Http {
    
    constructor(
        private backend: ConnectionBackend,
        private defaultOptions: RequestOptions,
        private authService: AuthService
    ) {
        super(backend, defaultOptions);
    }

    request(url: string | Request, options?: RequestOptionsArgs): Observable<Response> {
        let urlString = <string>url;
        if (typeof(url) === 'object') {
            urlString = url.url
        }
        return super.request(url, this.getRequestOptionArgs(urlString, options));
    }

    get(url: string, options?: RequestOptionsArgs): Observable<Response> {
        return super.get(url, this.getRequestOptionArgs(url, options));
    }

    post(url: string, body: string, options?: RequestOptionsArgs): Observable<Response> {
        return super.post(url, body, this.getRequestOptionArgs(url, options));
    }

    put(url: string, body: string, options?: RequestOptionsArgs): Observable<Response> {
        return super.put(url, body, this.getRequestOptionArgs(url, options));
    }

    delete(url: string, options?: RequestOptionsArgs): Observable<Response> {
        return super.delete(url, this.getRequestOptionArgs(url, options));
    }

    private getRequestOptionArgs(url: string, options?: RequestOptionsArgs) : RequestOptionsArgs {
        if (!options) {
            options = {};
        }

        if (!options.headers) {
            options.headers = new Headers();
        }

        let { apiBaseUrl } = NULOBE_ENV_SETTINGS;
        if (url.startsWith(apiBaseUrl)) {
            let apiPath = url.substring(apiBaseUrl.length);
            let authorityName = this.getAuthorityName(apiPath);

            let bearerToken = this.authService.getBearerToken(authorityName);
            if (bearerToken) {
                options.headers.append('Authorization', `Bearer ${bearerToken}`);
            }
        }

        return options;
    }

    private getAuthorityName(apiPath: string): string {
        let pathSegments = apiPath.split('/');
        switch (pathSegments[1].toLowerCase()) {
            case 'quizlet': return 'quizlet';
            default: return null;
        }
    }
}