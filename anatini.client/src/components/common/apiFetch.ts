import { store } from "@/store";
import type { IsAuthenticated } from "@/types";

export async function apiFetch<Type>(
  input: RequestInfo | URL,
  onfulfilled: (value: Type) => void,
  onfinally?: () => void,
  init?: RequestInit,
  statusActions?: {[id: number]: () => void},
  handleResponse?: (response: Response) => void
): Promise<void> {
  await fetch(`/api/${input}`, init).then((response: Response) => {
    if (response.ok) {
      handleResponse?.(response);

      response.json()
        .then(onfulfilled)
        .catch(() => { console.log('Unknown Error'); });
    }

    if (statusActions?.[response.status]) {
      statusActions[response.status]();
    }
  })
  .finally(onfinally);
}

export async function apiFetchAuthenticated<Type>(
  input: RequestInfo | URL,
  onfulfilled: (value: Type) => void,
  onfinally?: () => void,
  init?: RequestInit,
  statusActions?: {[id: number]: () => void},
  handleResponse?: (response: Response) => void
): Promise<void> {
  if (store.expiresUtc === null) {
    statusActions?.[401]?.();
    return;
  }

  const secondsRemaining = Math.abs(new Date(store.expiresUtc).valueOf() - Date.now().valueOf()) / 1000;

  if (secondsRemaining < 600) {
    await fetch("/api/authentication/refresh-token", { method: "POST" }).then((response: Response) => {
      if (response.ok) {
        response.json()
          .then((value: IsAuthenticated) => {
            store.isLoggedIn = value.isAuthenticated;
            store.expiresUtc = value.expiresUtc;
          })
          .catch(() => {
            store.isLoggedIn = false;
          });
      } else {
        store.isLoggedIn = false;
      }
    });
  }

  return await apiFetch(input, onfulfilled, onfinally, init, statusActions, handleResponse);
}