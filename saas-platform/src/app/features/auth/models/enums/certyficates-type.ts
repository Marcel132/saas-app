export enum CertificatesType {
  None = 'None',
  ComptiaPentestPlus = 'CompTIA PenTest+',
  Ecppt = 'eLearnSecurity Certified Professional Penetration Tester (eCPPT)',
  Ceh = 'Certified Ethical Hacker (CEH)',
  Bscp = 'Burp Suite Certified Practitioner (BSCP)',
  Oscp = 'Offensive Security Certified Professional (OSCP)',
  Gpen = 'GIAC Penetration Tester (GPEN)',
  Gwapt = 'GIAC Web Application Penetration Tester (GWAPT)',
  Osep = 'Offensive Security Experienced Penetration Tester (OSEP)',
  Oswe = 'Offensive Security Web Expert (OSWE)',
  Crto = 'Certified Red Team Operator (CRTO)',
  Osee = 'Offensive Security Exploitation Expert (OSEE)'
}

export const certificatesTypeValues: CertificatesType[] = Object.values(CertificatesType);
