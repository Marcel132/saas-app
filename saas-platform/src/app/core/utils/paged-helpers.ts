import { signal } from "@angular/core";
import { PagedRequestModel } from "../models/paged-request-model";

const INITIAL_STATE_PAGED_REQUEST: PagedRequestModel = {
  page: 1,
  pageSize: 2,
  search: null
}

export function createRequestPagedState() {
  const _request = signal<PagedRequestModel>(INITIAL_STATE_PAGED_REQUEST)

  function set(pageModel: PagedRequestModel) {
    _request.set(pageModel);
    return
  }

  function update(updater: (current: PagedRequestModel) => PagedRequestModel){
    _request.update(updater);
    return
  }

  function clear() {
    _request.set(INITIAL_STATE_PAGED_REQUEST);
    return
  }

  return {
    state: _request.asReadonly(),
    set,
    update,
    clear
  }
}
