// src/app/data/example-queries.data.ts
import { ExampleQuery } from '../models/example-query.model';

/**
 * Predefined example queries to help users understand
 * how to ask for different kinds of filters over the dataset.
 */
export const EXAMPLE_QUERIES: ExampleQuery[] = [
  {
    label: 'Non-immune after 1990',
    description: 'Give me all non immune users born after 1990.',
    prompt:
      'Give me all non immune users born after 1990 and include yearOfBirth and phoneNumber.',
  },
  {
    label: 'Immune with vaccine details',
    description: 'List immune users with their vaccineDate and phoneNumber.',
    prompt:
      'List all immune users and include their vaccineDate, phoneNumber and yearOfBirth.',
  },
  {
    label: 'Immune before 1985',
    description: 'Show users born before 1985 who are immune.',
    prompt:
      'Show users who are immune and born before 1985, with yearOfBirth, vaccineDate and phoneNumber.',
  },
  {
    label: 'Newest vaccinated',
    description: 'Who are the most recently vaccinated immune users?',
    prompt:
      'Give me immune users sorted by vaccineDate (newest first), including name, vaccineDate and phoneNumber.',
  },
  {
    label: 'Older non-immune',
    description: 'Non-immune users older than 40.',
    prompt:
      'List all non immune users born before 1985 and include yearOfBirth and phoneNumber.',
  },
];
