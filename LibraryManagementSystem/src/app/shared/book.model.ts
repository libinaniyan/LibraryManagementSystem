export interface Book {
  id: number;
  title: string;
  author: string;
  publisher: string;
  publication_date: Date;
  genre: string;
  price: number;
  barcode: string;
  rating?: number;
  review_count?: number;
  total_copies: number;
  available_copies?: number;   
  }
  
