export interface Spending {
  id: number;
  bucketId: number;
  date: string; // DateOnly as string YYYY-MM-DD
  description: string;
  amount: number;
  owner: string;
  tagNames: string[];
  enabled: boolean;
  createdAt?: Date;
  updatedAt?: Date;
}

export interface CreateSpendingCommand {
  bucketId: number;
  date: string; // DateOnly as string YYYY-MM-DD
  description: string;
  amount: number;
  owner: string;
  tagNames: string[];
}

export interface UpdateSpendingRequest {
  date: string; // DateOnly as string YYYY-MM-DD
  description: string;
  amount: number;
  owner: string;
  tagNames: string[];
}
