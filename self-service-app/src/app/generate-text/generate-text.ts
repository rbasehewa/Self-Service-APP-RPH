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
   * isQueryLoading:
   *  - true while we are waiting for /query-users response
   *  - used to show a Tailwind spinner and disable the button
   */
  isQueryLoading = signal(false);

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

  /**
   * Calls /query-users with:
   *  - prompt = user text
   *  - data   = SAMPLE_USERS
   *
   * And stores the resulting user array into llmUsers,
   * which automatically drives tableUsers().
   */
  queryUsers() {
    // Start loading
    this.isQueryLoading.set(true);

    this.generatedResponse.set({ text: 'Loading users......', error: null });

    this.#genAI.queryUsers(this.prompt(), SAMPLE_USERS).subscribe({
      next: (res) => {
        if (res.result) {
          // LLM returned structured JSON array of users
          this.llmUsers.set(res.result);

          this.generatedResponse.set({
            text: JSON.stringify(res.result, null, 2),
            error: null,
          });
        } else {
          // if we only got raw text, clear llmUsers
          this.llmUsers.set([]);

          this.generatedResponse.set({
            text: res.raw ?? 'No structured result returned.',
            error: null,
          });
        }
      },
      error: () => {
        this.llmUsers.set([]);
        this.generatedResponse.set({
          text: 'Something went wrong while querying users.',
          error: 'Error querying users',
        });
      },
      complete: () => {
        // Stop loading regardless of success/failure
        this.isQueryLoading.set(false);
      },
    });
  }
}
