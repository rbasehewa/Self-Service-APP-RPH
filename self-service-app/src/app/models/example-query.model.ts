// src/app/models/example-query.model.ts

/**
 * Represents a reusable LLM example query shown as a suggestion "chip".
 * Each example has:
 *  - label: short title shown in the UI
 *  - description: human-friendly explanation
 *  - prompt: the actual LLM query text
 */
export interface ExampleQuery {
  label: string;
  description: string;
  prompt: string;
}