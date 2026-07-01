import { RoleType } from "../enums/role-type.enum";
import { CurrentUserBase } from "./current-user-base";

export interface CurrentCompany extends CurrentUserBase{
  role: RoleType.company;
  
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
