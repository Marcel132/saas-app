import { inject, Injectable } from "@angular/core";
import { UserApi } from "../../../core/services/user-api";
import { switchMap, tap } from "rxjs";

@Injectable({
  providedIn: 'root'
})
export class MainStore{
  private readonly userApi = inject(UserApi)

  loadSummary(){
    this.userApi.getCurrentUserSummary().pipe(
      tap(() => console.log("działa")),
    ).subscribe()
  }

}
