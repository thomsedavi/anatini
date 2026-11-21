import { reactive } from 'vue'
import type { Account } from '@/common/types';

export const store = reactive<{ account: Account }>({ account: { isAuthenticated: null, isTrusted: null, spaces: null, userId: null, userHandle: null } });
