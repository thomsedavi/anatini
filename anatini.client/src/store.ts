import { reactive } from 'vue'
import type { SpaceEdit } from './types';

type Store = {
  isAuthenticated: boolean | null;
  isTrusted: boolean | null;
  spaces: SpaceEdit[] | null;
}

export const store = reactive<Store>({ isAuthenticated: null, isTrusted: null, spaces: null });
