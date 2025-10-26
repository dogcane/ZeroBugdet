export interface Bucket {
  id: number;
  name: string;
  description: string;
  defaultLimit: number;
  enabled: boolean;
  createdAt?: Date;
  updatedAt?: Date;
}

export interface CreateBucketCommand {
  name: string;
  description: string;
  defaultLimit: number;
}

export interface UpdateBucketRequest {
  name: string;
  description: string;
  defaultLimit: number;
}
