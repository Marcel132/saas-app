export interface RequestState {
  state: 'idle' | 'loading' | 'success' | 'error',
  message: string;
}
