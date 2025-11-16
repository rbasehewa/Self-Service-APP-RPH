// src/app/gemini.service.ts
import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { map } from 'rxjs/operators'; // Needed for the map operator

// Define the shape of the raw response object we get from the backend proxy.
export type GeminiResponse = {
  candidates: {
    content: {
      parts: {
        text: string;
      }[];
    };
  }[];
};

// The URL for our Express server (The proxy)
const API_URL = 'http://localhost:3000'; 

@Injectable({
  providedIn: 'root',
})
export class Gemini {
  // Using the private field syntax for HttpClient - Tidy!
  readonly #http = inject(HttpClient);

  /**
   * Sends a prompt to our secure backend proxy and returns the model's text.
   * @param prompt The user's text input.
   * @returns An observable of just the generated text string.
   */
  generateContent(prompt: string) {
    return this.#http.post<GeminiResponse>(`http://localhost:3000/generate`, { prompt }).pipe(
      // Map the complex response object down to just the text!
      map((response) => response.candidates[0]?.content.parts[0].text || 'No response')
    );
  }
}