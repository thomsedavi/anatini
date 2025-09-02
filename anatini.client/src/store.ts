import { reactive } from 'vue'

type Store = {
  isLoggedIn?: boolean;
}

export const store = reactive<Store>({});
