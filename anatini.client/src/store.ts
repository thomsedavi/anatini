import { reactive } from 'vue'

type Store = {
  isLoggedIn: boolean | null;
  expiresUtc: string | null;
}

export const store = reactive<Store>({ isLoggedIn: null, expiresUtc: null });
