import { signal } from "@angular/core";
import { RequestState, RequestStateStatus } from "../models/request-state";

const INITIAL_STATE: RequestState = { state: 'idle', message: '' };
const DEFAULT_ERROR_MESSAGE = "Wystąpił nieoczekwiany błąd";

export function createRequestState(){
  const _request = signal<RequestState>(INITIAL_STATE);

  function set(state: RequestStateStatus, message: string | null){
    const trimed = message?.trim() ?? ''

    if(state === 'error' && !trimed){
      console.error('Error detected: message not found');
      _request.set({ state, message: DEFAULT_ERROR_MESSAGE });
      return;
    }

    _request.set({state, message: trimed});
  }

  function clear(){
    _request.set(INITIAL_STATE);
  }

  return {
    state: _request.asReadonly(),
    set,
    clear
  }
}
