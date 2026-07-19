import { store } from "@/store";
import type { Request, SearchParameter } from "@/types";

export async function apiFetch({ input, statusActions, init, searchParameters, onfinally }: Request): Promise<void> {
  await fetch(`/api/${input}${getParameters(searchParameters)}`, init).then((response: Response) => {
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
  const responses: Response[] = await Promise.all(requests.map(request => fetch(`/api/${request.input}${getParameters(request.searchParameters)}`, request.init)));

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
