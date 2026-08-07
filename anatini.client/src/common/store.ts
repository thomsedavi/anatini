import { reactive } from 'vue'
import type { SpaceEdit } from '@/common/types';

type Store = {
  isAuthenticated: boolean | null;
  userId: string | null;
  userHandle: string | null;
  isTrusted: boolean | null;
  spaces: SpaceEdit[] | null;
}

export const store = reactive<Store>({ isAuthenticated: null, isTrusted: null, spaces: null, userId: null, userHandle: null });
