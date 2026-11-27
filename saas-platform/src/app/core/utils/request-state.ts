import { signal } from "@angular/core";

const INITIAL_STATE: RequestState = { state: 'idle', message: '' };
const DEFAULT_ERROR_MESSAGE = "Wystąpił nieoczekwiany błąd";

export function createRequestState() {
  const _request = signal<RequestState>(INITIAL_STATE);

  function set(state: RequestStateStatus, message: string | null) {
    const trimmed = message?.trim() ?? ''

    if (state === 'error' && !trimmed) {
      console.error('Error detected: message not found');
      _request.set({ state, message: DEFAULT_ERROR_MESSAGE });
      return;
    }

    _request.set({ state, message: trimmed });
  }

  function clear() {
    _request.set(INITIAL_STATE);
  }

  return {
    state: _request.asReadonly(),
    set,
    clear
  }
}
export type RequestStateStatus = 'idle' | 'loading' | 'success' | 'error';

export interface RequestState {
  state: 'idle' | 'loading' | 'success' | 'error',
  message: string;
}
