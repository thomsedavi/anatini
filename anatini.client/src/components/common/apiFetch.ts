import { store } from "@/store";
import type { Request, SearchParameter } from "@/types";

let csrfBootstrapPromise: Promise<void> | null = null;
let csrfTokenAuthState: boolean | null = null;

export async function apiFetch({ input, statusActions, init, searchParameters, onfinally }: Request): Promise<void> {
  const requestInit = await createRequestInitWithCsrf(init);

  await fetch(`/api/${input}${getParameters(searchParameters)}`, requestInit).then((response: Response) => {
    const statusAction = statusActions[response.status];
    
    if (statusAction !== undefined) {
      statusAction(response);
    } else {
      console.log('unhandled response', response.status);
    }
  })
  .finally(onfinally);
}

export async function apiFetchAll(requests: Request[]): Promise<void> {
  const responses: Response[] = await Promise.all(requests.map(async request => {
    const requestInit = await createRequestInitWithCsrf(request.init);
    return fetch(`/api/${request.input}${getParameters(request.searchParameters)}`, requestInit);
  }));

  if (responses.every(response => response.ok)) {
    responses.forEach((response: Response, index: number) => {
      const statusAction = requests[index].statusActions[response.status];


      if (statusAction !== undefined) {
        statusAction(response);
      } else {
        console.log('unhandled response', response.status);
      }
    });
  } else {
    console.log('TODO handle');
  }
}

export async function apiFetchAuthenticated({ input, statusActions, init, searchParameters, onfinally }: Request): Promise<void> {
  if (store.isAuthenticated === null || !store.isAuthenticated) {
    statusActions?.[401]?.();
    return;
  }

  return await apiFetch({ input, statusActions, init, searchParameters, onfinally });
}

function getParameters(searchParameters?: SearchParameter[]): string {
  let parameters = '';

  if (searchParameters !== undefined && searchParameters.length > 0) {
    parameters += '?';
    parameters += searchParameters.map(searchParameter => `${searchParameter.key}=${searchParameter.value}`).join('&');
  }
  
  return parameters;
}

async function createRequestInitWithCsrf(init?: RequestInit): Promise<RequestInit | undefined> {
  if (!isUnsafeMethod(init?.method)) {
    return init;
  }

  await ensureCsrfToken();

  const csrfToken = getCookieValue('XSRF-TOKEN');

  if (csrfToken === null) {
    return init;
  }

  const headers = new Headers(init?.headers);
  headers.set('X-CSRF-TOKEN', csrfToken);

  return {
    ...init,
    headers,
  };
}

function isUnsafeMethod(method?: string): boolean {
  const normalizedMethod = method?.toUpperCase() ?? 'GET';
  return normalizedMethod === 'POST' || normalizedMethod === 'PUT' || normalizedMethod === 'PATCH' || normalizedMethod === 'DELETE';
}

async function ensureCsrfToken(): Promise<void> {
  const isAuthenticated = store.isAuthenticated === true;

  if (getCookieValue('XSRF-TOKEN') !== null && csrfTokenAuthState === isAuthenticated) {
    return;
  }

  csrfBootstrapPromise ??= fetch('/api/authentication/csrf-token', { method: 'GET', cache: 'no-store' }).then(() => undefined).finally(() => {
    csrfBootstrapPromise = null;
  });

  await csrfBootstrapPromise;
  csrfTokenAuthState = isAuthenticated;
}

function getCookieValue(name: string): string | null {
  const escapedName = name.replace(/[-[\]{}()*+?.,\\^$|#\s]/g, '\\$&');
  const match = document.cookie.match(new RegExp(`(?:^|; )${escapedName}=([^;]*)`));

  if (match === null) {
    return null;
  }

  return decodeURIComponent(match[1]);
}
