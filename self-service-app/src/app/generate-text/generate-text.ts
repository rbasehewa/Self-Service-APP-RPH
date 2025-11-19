import { Component, computed, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { Gemini } from '../gemini';
import { SAMPLE_USERS, User } from '../models/user.model';
import { Usertable } from '../usertable/usertable';
import { ExampleQuery } from '../models/example-query.model';
import { EXAMPLE_QUERIES } from '../data/example-queries.data';
import { faSearchengin } from '@fortawesome/free-brands-svg-icons';
import { FaIconComponent } from '@fortawesome/angular-fontawesome';
import { faWandMagicSparkles } from '@fortawesome/free-solid-svg-icons';

@Component({
  selector: 'app-generate-text',
  standalone: true,
  imports: [FormsModule, CommonModule, Usertable, FaIconComponent],
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

  readonly faSearchengin = faSearchengin;
  readonly faWandMagicSparkles = faWandMagicSparkles;

  /** Has the user run at least one AI table query? */
  hasLLMQuery = signal(false);

  /** Remember the last AI query text (for empty-state message) */
  lastQuery = signal('');

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

  /**
   * Table data:
   *  - Before ANY AI query → show SAMPLE_USERS
   *  - After an AI query   → show whatever llmUsers() holds (even empty)
   */
  readonly tableUsers = computed<User[]>(() => {
    return this.hasLLMQuery() ? this.llmUsers() : SAMPLE_USERS;
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
    const currentPrompt = this.prompt().trim();

    // store the query for the empty-state card
    this.lastQuery.set(currentPrompt);

    // flag that we are now in "AI mode"
    this.hasLLMQuery.set(true);

    // start loading
    this.isLoading.set(true);

    // reset previous response
    this.generatedResponse.set({ text: 'Loading...', error: null });

    this.#genAI.queryUsers(currentPrompt, SAMPLE_USERS).subscribe({
      next: (res) => {
        this.isLoading.set(false);

        if (res.result && Array.isArray(res.result)) {
          console.log('response:', res.result);
          this.llmUsers.set(res.result);

          this.generatedResponse.set({
            text: JSON.stringify(res.result, null, 2),
            error: null,
          });
        } else {
          // model didn’t give structured JSON → treat as “no matches”
          this.llmUsers.set([]);

          this.generatedResponse.set({
            text: res.raw ?? 'No structured result returned.',
            error: null,
          });
        }
      },
      error: (err) => {
        console.error('Error querying users:', err);
        this.isLoading.set(false);

        // on hard error, go back to "no AI" mode
        this.hasLLMQuery.set(false);
        this.llmUsers.set([]);

        this.generatedResponse.set({
          text: 'Something went wrong while querying users.',
          error: 'Error querying users',
        });
      },
    });
  }
}
