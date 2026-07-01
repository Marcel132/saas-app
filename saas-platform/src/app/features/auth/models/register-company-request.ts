export interface RegisterCompanyRequest {
  email: string,
  password: string,
  name: string,
  nip: string,
  phone: string,
  city: string,
  country: string,
  postalCode: string,
  street: string,
  bio: string | null,
  websiteUrl: string | null
}
