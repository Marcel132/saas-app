import { RoleType } from "./enums/role-type.enum";
import { SpecializationType } from "./enums/specialization-type.enum";

export interface RegisterRequest
{
  Email: string,
  Password: string,
  Role: RoleType,
  SpecializationType: SpecializationType[],
  Description: string,
  FirstName: string,
  LastName: string,
  Nickname: string,
  PhoneNumber: string,
  City: string,
  Country: string
  PostalCode: string,
  Street: string,
  CompanyName: string | null,
  CompanyNip: string | null
}
