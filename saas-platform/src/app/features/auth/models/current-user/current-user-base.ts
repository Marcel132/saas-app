import { RoleType } from "../enums/role-type.enum";
import { CurrentCompany } from "./current-company";
import { CurrentPentester } from "./current-pentester";

export interface CurrentUserBase {
  id: string,
  email: string,
  role: RoleType,
  isActive: boolean,
  createdAt: string,

  roles: string[],
  permissions: string[],
}

export type CurrentUser =
  | CurrentPentester
  | CurrentCompany;
