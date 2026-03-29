import { reactive } from 'vue'

type Store = {
  isAuthenticated: boolean | null;
}

export const store = reactive<Store>({ isAuthenticated: null });
