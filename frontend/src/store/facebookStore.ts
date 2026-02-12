import { create } from 'zustand';
import type { FacebookPageDto } from '../types/facebook';

interface FacebookState {
  pages: FacebookPageDto[];
  selectedPageId: string | null;
  loading: boolean;
  setPages: (pages: FacebookPageDto[]) => void;
  addPage: (page: FacebookPageDto) => void;
  removePage: (pageId: string) => void;
  setSelectedPage: (pageId: string | null) => void;
  setLoading: (loading: boolean) => void;
}

export const useFacebookStore = create<FacebookState>()((set) => ({
  pages: [],
  selectedPageId: null,
  loading: false,

  setPages: (pages) => set({ pages }),

  addPage: (page) =>
    set((state) => ({ pages: [...state.pages, page] })),

  removePage: (pageId) =>
    set((state) => ({
      pages: state.pages.filter((p) => p.id !== pageId),
      selectedPageId: state.selectedPageId === pageId ? null : state.selectedPageId,
    })),

  setSelectedPage: (pageId) => set({ selectedPageId: pageId }),

  setLoading: (loading) => set({ loading }),
}));
