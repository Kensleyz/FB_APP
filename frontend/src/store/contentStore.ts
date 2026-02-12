import { create } from 'zustand';
import type { PostVariation, BusinessType, Tone, PostType, Language } from '../types/content';

interface ContentState {
  businessName: string;
  businessDescription: string;
  businessType: BusinessType;
  tone: Tone;
  postType: PostType;
  language: Language;
  variations: PostVariation[];
  selectedVariation: PostVariation | null;
  generating: boolean;
  setFormField: (field: string, value: string) => void;
  setVariations: (variations: PostVariation[]) => void;
  setSelectedVariation: (variation: PostVariation | null) => void;
  updateVariation: (id: string, content: string) => void;
  setGenerating: (generating: boolean) => void;
  resetForm: () => void;
}

const initialFormState = {
  businessName: '',
  businessDescription: '',
  businessType: 'General' as BusinessType,
  tone: 'Friendly' as Tone,
  postType: 'Promotional' as PostType,
  language: 'English' as Language,
};

export const useContentStore = create<ContentState>()((set) => ({
  ...initialFormState,
  variations: [],
  selectedVariation: null,
  generating: false,

  setFormField: (field, value) =>
    set({ [field]: value } as Partial<ContentState>),

  setVariations: (variations) =>
    set({ variations, selectedVariation: null }),

  setSelectedVariation: (variation) =>
    set({ selectedVariation: variation }),

  updateVariation: (id, content) =>
    set((state) => ({
      variations: state.variations.map((v) =>
        v.id === id ? { ...v, content } : v
      ),
    })),

  setGenerating: (generating) => set({ generating }),

  resetForm: () =>
    set({ ...initialFormState, variations: [], selectedVariation: null }),
}));
