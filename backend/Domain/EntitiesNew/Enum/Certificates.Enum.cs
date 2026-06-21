using System.Runtime.Serialization;

namespace backend.Domain.EntitiesNew.Enum;

public enum CertificatesType
{
  [EnumMember(Value = "None")]
  None,
  [EnumMember(Value = "CompTIA PenTest+")]
  ComptiaPentestPlus,

  [EnumMember(Value = "eLearnSecurity Certified Professional Penetration Tester (eCPPT)")]
  Ecppt,

  [EnumMember(Value = "Certified Ethical Hacker (CEH)")]
  Ceh,

  [EnumMember(Value = "Burp Suite Certified Practitioner (BSCP)")]
  Bscp,

  [EnumMember(Value = "Offensive Security Certified Professional (OSCP)")]
  Oscp,

  [EnumMember(Value = "GIAC Penetration Tester (GPEN)")]
  Gpen,

  [EnumMember(Value = "GIAC Web Application Penetration Tester (GWAPT)")]
  Gwapt,

  [EnumMember(Value = "Offensive Security Experienced Penetration Tester (OSEP)")]
  Osep,

  [EnumMember(Value = "Offensive Security Web Expert (OSWE)")]
  Oswe,

  [EnumMember(Value = "Certified Red Team Operator (CRTO)")]
  Crto,

  [EnumMember(Value = "Offensive Security Exploitation Expert (OSEE)")]
  Osee
}