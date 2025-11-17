// src/app/models/user.model.ts
export interface User {
  id: number;
  name: string;
  immuneStatus: 'Immune' | 'Non-Immune' | 'Unknown';
  phoneNumber: string;
}

export const SAMPLE_USERS: User[] = [
  { id: 1, name: 'Alice',   immuneStatus: 'Immune',      phoneNumber: '0400 111 111' },
  { id: 2, name: 'Bob',     immuneStatus: 'Non-Immune',  phoneNumber: '0400 222 222' },
  { id: 3, name: 'Charlie', immuneStatus: 'Non-Immune',  phoneNumber: '0400 333 333' },
  { id: 4, name: 'Diana',   immuneStatus: 'Immune',      phoneNumber: '0400 444 444' },
];
