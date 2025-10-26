export interface MonthlyBucket {
  id: number;
  bucketId: number;
  bucketName: string;
  year: number;
  month: number;
  limit: number;
  total: number;
  enabled: boolean;
}

export interface MonthlySpending {
  id: number;
  monthlyBucketId: number;
  date: string; // DateOnly as string YYYY-MM-DD
  description: string;
  amount: number;
  owner: string;
  tagNames: string[];
  enabled: boolean;
}

export interface GenerateMonthlyDataRequest {
  year: number;
  month: number;
}

export interface MonthlySummary {
  year: number;
  month: number;
  buckets: MonthlyBucket[];
  spendings: MonthlySpending[];
}
