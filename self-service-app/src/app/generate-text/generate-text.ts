import { Component, computed, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { Gemini } from '../gemini';
import { SAMPLE_USERS, User } from '../models/user.model';
import { Usertable } from '../usertable/usertable';
import { ExampleQuery } from '../models/example-query.model';
import { EXAMPLE_QUERIES } from '../data/example-queries.data';

@Component({
  selector: 'app-generate-text',
  standalone: true,
  imports: [FormsModule, CommonModule, Usertable],
  templateUrl: './generate-text.html',
  styleUrls: ['./generate-text.scss'],
})
export class GenerateText {
  readonly #genAI = inject(Gemini);

  // prompt + response
  prompt = signal('');
  generatedResponse = signal<{ text: string; error: string | null }>({
    text: '',
    error: null,
  });

  /**
   * Loading flag:
   *  - true while we are calling /query-users OR doing fallback /generate
   *  - used for button spinner + (optional) table skeleton
   */
  isLoading = signal(false);

  /**
   * LLM user result:
   *  - empty → show full SAMPLE_USERS
   *  - non-empty → show whatever Gemini returned
   */
  llmUsers = signal<User[]>([]);
  // This is what your template's @for is using
  readonly exampleQueries: ExampleQuery[] = EXAMPLE_QUERIES;
  // what the table should show:
  // - before first LLM query → all SAMPLE_USERS
  // - after LLM query       → whatever Gemini returned
  // This decides what the table sees
  readonly tableUsers = computed<User[]>(() => {
    const llm = this.llmUsers();
    // if LLM has results, use them
    return llm.length > 0 ? llm : SAMPLE_USERS; // otherwise SAMPLE_USERS
  });

  useExample(example: ExampleQuery) {
    this.prompt.set(example.prompt);
  }

  generateResponse() {
    this.generatedResponse.set({ text: 'Loading...', error: null });
    this.isLoading.set(true);

    this.#genAI.generateContent(this.prompt()).subscribe({
      next: (text) => {
        this.generatedResponse.set({ text, error: null });
        this.isLoading.set(false);
      },
      error: () => {
        this.generatedResponse.set({
          text: 'Something went wrong on the server, check your console!',
          error: 'Error generating text',
        });
        this.isLoading.set(false);
      },
    });
  }

  /**
   * New “smart” handler for the single button.
   *
   * Flow:
   *   1) Try /query-users with the staff dataset.
   *   2) If we get a non-empty user array → update table + show JSON in card.
   *   3) If we get no structured users (or empty array) → fall back to a
   *      general Gemini answer by calling generateResponse().
   */
  queryUsersSmart() {
    this.isLoading.set(true);
    this.generatedResponse.set({ text: 'Loading...', error: null });

    this.#genAI.queryUsers(this.prompt(), SAMPLE_USERS).subscribe({
      next: (res) => {
        // case A: structured JSON array of users came back
        if (res.result && res.result.length > 0) {
          this.llmUsers.set(res.result);

          this.generatedResponse.set({
            text: JSON.stringify(res.result, null, 2),
            error: null,
          });

          this.isLoading.set(false);
          return;
        }

        // case B: no structured users → clear table result and fall back
        this.llmUsers.set([]);

        // If there was some raw text from Gemini, show it,
        // but we still do a proper /generate call to handle “dog” etc.
        if (res.raw) {
          this.generatedResponse.set({
            text: res.raw,
            error: null,
          });
        }

        // Fallback to normal AI answer (uses the same prompt)
        this.generateResponse(); // generateResponse will turn off isLoading
      },
      error: () => {
        this.llmUsers.set([]);
        this.generatedResponse.set({
          text: 'Something went wrong while querying users.',
          error: 'Error querying users',
        });
        this.isLoading.set(false);
      },
    });
  }
}
