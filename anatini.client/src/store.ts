import { reactive } from 'vue'

type Store = {
  isLoggedIn?: boolean;
  expiresUtc?: string;
}

export const store = reactive<Store>({});
