import { reactive } from 'vue'
import type { ChannelEdit } from './types';

type Store = {
  isAuthenticated: boolean | null;
  isTrusted: boolean | null;
  channels: ChannelEdit[] | null;
}

export const store = reactive<Store>({ isAuthenticated: null, isTrusted: null, channels: null });
