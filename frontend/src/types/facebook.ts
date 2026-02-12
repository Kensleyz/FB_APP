export interface FacebookPageDto {
  id: string;
  facebookPageId: string;
  pageName: string;
  pageCategory: string;
  profilePictureUrl?: string;
  followerCount: number;
  isActive: boolean;
  connectedAt: string;
  lastSyncedAt?: string;
}

export interface ConnectFacebookRequest {
  accessToken: string;
  selectedPageIds: string[];
}

export interface InsightsDto {
  pageId: string;
  pageName: string;
  followerCount: number;
  followersGrowth: number;
  postsCount: number;
  engagementRate: number;
  reachTotal: number;
  impressionsTotal: number;
  period: string;
}

export interface FacebookCallbackParams {
  code: string;
  state?: string;
}
