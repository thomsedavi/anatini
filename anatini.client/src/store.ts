import { reactive } from 'vue'

type Store = {
  isSignedIn: boolean | null;
}

export const store = reactive<Store>({ isSignedIn: null });
