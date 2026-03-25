import { store } from "@/store";
import type { StatusActions } from "@/types";

export async function apiFetch(
  input: RequestInfo | URL,
  statusActions: StatusActions,
  init?: RequestInit,
  onfinally?: () => void,
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
  init?: RequestInit,
  onfinally?: () => void,
): Promise<void> {
  if (store.isAuthenticated ?? false === false) {
    statusActions?.[401]?.();
    return;
  }

  return await apiFetch(input, statusActions, init, onfinally);
}