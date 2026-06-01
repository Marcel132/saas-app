import { SpecializationType } from "../../auth/models/specialization-type.enum";
import { RoleType } from "../../auth/models/role-type.enum";


export interface CurrentUserDto {
  id: number,
  email: string,
  role: RoleType,
  firstName: string,
  lastName: string,
  nickname: string,
  description: string,
  specialization: SpecializationType[],
  companyName: string | null,
  companyNip: string | null,
  createdAt: Date,
  isActive: boolean
  permissions: string[]
  roles: string[]
}
