import { Component, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { Gemini } from '../gemini';
import { SAMPLE_USERS, User } from '../models/user.model';

@Component({
  selector: 'app-generate-text',
  standalone: true,
  imports: [FormsModule, CommonModule],
  templateUrl: './generate-text.html',
  styleUrls: ['./generate-text.scss'],
})
export class GenerateText {
  readonly #genAI = inject(Gemini);

  prompt = signal('');
  generatedResponse = signal<{ text: string; error: string | null }>({
    text: '',
    error: null,
  });

  users: User[] = SAMPLE_USERS;

  /* Existing text-generation method (for HTML, emails, etc.) */
  generateResponse() {
    this.generatedResponse.set({ text: 'Loading...', error: null });

    this.#genAI.generateContent(this.prompt()).subscribe({
      next: (text) =>
        this.generatedResponse.set({
          text,
          error: null,
        }),
      error: () =>
        this.generatedResponse.set({
          text: 'Something went wrong on the server, check your console!',
          error: 'Error generating text',
        }),
    });
  }

  /* NEW: ask about the JSON dataset */
  queryUsers() {
    this.generatedResponse.set({ text: 'Loading...', error: null });

    this.#genAI.queryUsers(this.prompt(), this.users).subscribe({
      next: (res) => {
        if (res.result) {
          // pretty-print JSON
          this.generatedResponse.set({
            text: JSON.stringify(res.result, null, 2),
            error: null,
          });
        } else if (res.raw) {
          this.generatedResponse.set({ text: res.raw, error: null });
        } else {
          this.generatedResponse.set({
            text: 'No result returned.',
            error: null,
          });
        }
      },
      error: () =>
        this.generatedResponse.set({
          text: 'Something went wrong while querying users.',
          error: 'Error querying users',
        }),
    });
  }
}