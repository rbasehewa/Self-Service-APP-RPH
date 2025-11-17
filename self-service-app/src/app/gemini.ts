// src/app/gemini.service.ts
import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { map } from 'rxjs/operators'; // Needed for the map operator
import { User } from './models/user.model';

// Define the shape of the raw response object we get from the backend proxy.
export type GeminiResponse = {
  text: string;
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
    return this.#http
      .post<GeminiResponse>(`${API_URL}/generate`, { prompt })
      .pipe(map((response) => response.text || 'No response'));
  }

  queryUsers(prompt: string, data: User[]) {
    return this.#http.post<{ result?: User[]; raw?: string }>(`${API_URL}/query-users`, {
      prompt,
      data,
    });
  }
}
