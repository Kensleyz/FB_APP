export interface DashboardOverview {
  connectedPages: number;
  totalPagesAllowed: number;
  postsThisMonth: number;
  postsLimit: number;
  imagesThisMonth: number;
  imagesLimit: number;
  upcomingPosts: number;
  engagementRate: number;
  recentPosts: RecentPostDto[];
}

export interface RecentPostDto {
  id: string;
  content: string;
  pageName: string;
  status: PostStatus;
  scheduledFor?: string;
  publishedAt?: string;
  likes?: number;
  comments?: number;
  shares?: number;
}

export type PostStatus = 'Scheduled' | 'Published' | 'Failed' | 'Cancelled';

export interface ScheduleDto {
  id: string;
  pageId: string;
  pageName: string;
  content: string;
  imageUrl?: string;
  hashtags: string[];
  callToAction: string;
  scheduledFor: string;
  status: PostStatus;
  facebookPostId?: string;
  errorMessage?: string;
  createdAt: string;
  publishedAt?: string;
}

export interface CreateScheduleRequest {
  pageId: string;
  content: string;
  imageUrl?: string;
  hashtags: string[];
  callToAction: string;
  scheduledFor: string;
}

export interface UpdateScheduleRequest {
  content?: string;
  imageUrl?: string;
  hashtags?: string[];
  callToAction?: string;
  scheduledFor?: string;
}

export interface CalendarResponse {
  month: string;
  posts: ScheduleDto[];
}
