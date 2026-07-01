export const ExperienceLevel ={
  None: "None",
  Intern: "Intern",
  Beginner: "Beginner",
  Junior: "Junior",
  Mid: "Mid",
  Senior: "Senior",
} as const;

export type ExperienceLevel =
  typeof ExperienceLevel[keyof typeof ExperienceLevel];
