import { store } from "@/store";
import type { IsAuthenticated } from "@/types";

export async function apiFetch<Type>(input: RequestInfo | URL, onfulfilled: (value: Type) => void, onfinally: () => void, init?: RequestInit): Promise<void> {
  if (store.expiresUtc == undefined) {
    return;
  }

  const secondsRemaining = Math.abs(new Date(store.expiresUtc).valueOf() - Date.now().valueOf()) / 1000;

  if (secondsRemaining < 600) {
    await fetch("api/authentication/refresh-token", { method: "POST" }).then((response: Response) => {
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

  fetch(`api/${input}`, init).then((response: Response) => {
    if (response.ok) {
      response.json()
        .then(onfulfilled)
        .catch(() => { console.log('Unknown Error'); });
    } else {
      console.log("Unknown Error");
    }
  })
  .finally(onfinally);
}
