import { RoleTypeValues } from "../enums/role-type.const";
import { CurrentUserBase } from "./current-user-base";

export interface CurrentCompany extends CurrentUserBase{
  role: typeof RoleTypeValues.Company;

  nip: string;
  name: string;

  phone: string;
  country: string;
  city: string;
  street: string;
  postalCode: string;

  bio?: string;
  websiteUrl?: string;
}
