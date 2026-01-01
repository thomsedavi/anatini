import { store } from "@/store";
import type { IsAuthenticated, StatusActions } from "@/types";

export async function apiFetch(
  input: RequestInfo | URL,
  statusActions: StatusActions,
  onfinally?: () => void,
  init?: RequestInit,
): Promise<void> {
  await fetch(`/api/${input}`, init).then((response: Response) => {
    if (statusActions[response.status]) {
      statusActions[response.status](response);
    }
  })
  .finally(onfinally);
}

export async function apiFetchAuthenticated(
  input: RequestInfo | URL,
  statusActions: StatusActions,
  onfinally?: () => void,
  init?: RequestInit,
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

  return await apiFetch(input, statusActions, onfinally, init);
}