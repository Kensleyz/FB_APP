export type BusinessType =
  | 'SpazaShop'
  | 'HairSalon'
  | 'Church'
  | 'Restaurant'
  | 'Gym'
  | 'FuneralParlor'
  | 'TaxiAssociation'
  | 'General';

export type Tone =
  | 'Professional'
  | 'Casual'
  | 'Friendly'
  | 'Energetic'
  | 'Respectful';

export type PostType =
  | 'Promotional'
  | 'Educational'
  | 'Engagement'
  | 'Event'
  | 'ProductShowcase'
  | 'Community';

export type Language = 'English' | 'Afrikaans' | 'Mixed';

export interface GeneratePostRequest {
  businessType: BusinessType;
  tone: Tone;
  postType: PostType;
  language: Language;
  businessName: string;
  businessDescription?: string;
}

export interface PostVariation {
  id: string;
  content: string;
  hashtags: string[];
  callToAction: string;
}

export interface GeneratePostResponse {
  variations: PostVariation[];
}

export interface GenerateImageRequest {
  query: string;
  postContent?: string;
}

export interface GenerateImageResponse {
  imageUrl: string;
  attribution: string;
}
