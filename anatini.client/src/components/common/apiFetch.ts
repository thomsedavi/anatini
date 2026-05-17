import { store } from "@/store";
import type { SearchParameter, StatusActions } from "@/types";

export async function apiFetch(
  input: RequestInfo | URL,
  statusActions: StatusActions,
  init?: RequestInit,
  searchParameters?: SearchParameter[],
  onfinally?: () => void,
): Promise<void> {
  let parameters = '';

  if (searchParameters !== undefined && searchParameters.length > 0) {
    parameters += '?';
    parameters += searchParameters.map(a => `${a.key}=${a.value}`).join('&');
  }

  await fetch(`/api/${input}${parameters}`, init).then((response: Response) => {
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
  searchParameters?: SearchParameter[],
  onfinally?: () => void,
): Promise<void> {
  if (store.isAuthenticated === null || !store.isAuthenticated) {
    statusActions?.[401]?.();
    return;
  }

  return await apiFetch(input, statusActions, init, searchParameters, onfinally);
}