// src/app/data/example-queries.data.ts
import { ExampleQuery } from '../models/example-query.model';

/**
 * Predefined example queries to help users understand
 * how to ask for different kinds of filters over the synthetic staff dataset.
 */
export const EXAMPLE_QUERIES: ExampleQuery[] = [
  // {
  //   label: 'Younger, Non-Immune Staff',
  //   description: 'Filter for all staff members who are non-immune and were born after 1990.',
  //   prompt:
  //     'Give me all non immune users born after 1990 and include yearOfBirth and phoneNumber.',
  // },
  // {
  //   label: 'Immunity Status & Contact',
  //   description: 'List all immune staff, including their vaccination date and contact phone number.',
  //   prompt:
  //     'List all immune users and include their vaccineDate, phoneNumber and yearOfBirth.',
  // },
  {
    label: 'Senior Immune Staff',
    description: 'Find all immune staff members who are 40+ (born before 1985).',
    prompt:
      'Show users who are immune and born before 1985, with yearOfBirth, vaccineDate and phoneNumber.',
  },
  {
    label: 'Staff Over 40, Not Immune',
    description: 'Show all non-immune staff members who are older than 40 years old.',
    prompt:
      'List all non immune users born before 1985 and include yearOfBirth and phoneNumber.',
  },
];