export type RequestStateStatus = 'idle' | 'loading' | 'success' | 'error';

export interface RequestState {
  state: 'idle' | 'loading' | 'success' | 'error',
  message: string;
}
