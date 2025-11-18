import { CommonModule } from '@angular/common';
import { Component, computed, effect, input, signal } from '@angular/core';
import { SAMPLE_USERS, User } from '../models/user.model';
import { FormsModule } from '@angular/forms';
import { FontAwesomeModule } from '@fortawesome/angular-fontawesome';
import {
  faSort,
  faSortUp,
  faSortDown,
} from '@fortawesome/free-solid-svg-icons';

type SortField = 'id' | 'name' | 'yearOfBirth' | 'immuneStatus';
type SortDirection = 'asc' | 'desc';

@Component({
  selector: 'app-usertable',
  standalone: true,
  imports: [CommonModule,FormsModule, FontAwesomeModule],
  templateUrl: './usertable.html',
  styleUrl: './usertable.scss',
})
export class Usertable {

  /**
   * users:
   *   - Receives data from the parent component using Angular Signals Input API.
   *   - When the parent changes the users (e.g., LLM returns new list), this signal updates.
   */
  users = input<User[]>([]);

    /**
   * loading:
   *   - When true, we show skeleton rows instead of data.
   *   - Parent (GenerateText) binds: [loading]="isQueryLoading()".
   */
  loading = input(false);

  /**
   * filterText:
   *   - Stores user’s free-text search input.
   *   - Used to filter name, phone, immuneStatus, and yearOfBirth.
   */
  filterText = signal('');

  /**
   * filterStatus:
   *   - Controls the “Immune / Non-Immune / Unknown / All” filter chips.
   */
  filterStatus = signal<'All' | 'Immune' | 'Non-Immune' | 'Unknown'>('All');

  /**
   * Sorting State
   * sortField:
   *   - The column currently being sorted (id, name, immuneStatus, yearOfBirth).
   *
   * sortDirection:
   *   - asc = smallest → largest (A→Z, 1980→2020)
   *   - desc = largest → smallest
   */
  sortField = signal<SortField>('id');
  sortDirection = signal<SortDirection>('asc');

  // FontAwesome sorting icons (UI-only)
  readonly faSort = faSort;       // neutral icon
  readonly faSortUp = faSortUp;   // ascending icon
  readonly faSortDown = faSortDown; // descending icon

  constructor() {
    /**
     * effect():
     *   - Reacts when parent updates the user list.
     *   - Useful if you want to auto-reset filters when a new LLM response comes in.
     *   - Currently no reset required, but kept for clarity.
     */
    effect(() => {
      const currentUsers = this.users();
      console.log('[Usertable] received users:', currentUsers.length);
    });
  }

  /**
   * setSort():
   *   - Handles all sorting behaviour.
   *   - Clicking the same column toggles asc <-> desc.
   *   - Clicking a DIFFERENT column resets direction to asc.
   *
   * This method is intentionally simple and reused everywhere.
   */
  setSort(field: SortField) {
    if (this.sortField() === field) {
      // Same column → toggle direction
      this.sortDirection.set(
        this.sortDirection() === 'asc' ? 'desc' : 'asc'
      );
    } else {
      // New column → reset to ascending
      this.sortField.set(field);
      this.sortDirection.set('asc');
    }
  }

  /** Updates text filter */
  setFilterText(value: string) {
    this.filterText.set(value);
  }

  /** Updates immune status filter chip */
  setStatusFilter(value: 'All' | 'Immune' | 'Non-Immune' | 'Unknown') {
    this.filterStatus.set(value);
  }

  /**
   * filteredAndSortedUsers:
   * -----------------------
   * FINAL computed result shown in the HTML table.
   *
   * Applies operations in this order:
   *
   *    (1) immune status filter
   *    (2) free-text filter (name, phone, immuneStatus, yearOfBirth)
   *    (3) sorting (id, name, immuneStatus, yearOfBirth)
   *
   * The parent component NEVER needs to sort or filter —
   * all UI transformation happens here inside the table component.
   */
  readonly filteredAndSortedUsers = computed(() => {
    // Read reactive signal values
    const term = this.filterText().trim().toLowerCase();
    const statusFilter = this.filterStatus();
    const field = this.sortField();
    const dir = this.sortDirection();

    // Start with a COPY of incoming users (important: do not mutate input signals)
    let data = [...this.users()];

    // (1) Immune Status Filter
    if (statusFilter !== 'All') {
      data = data.filter((u) => u.immuneStatus === statusFilter);
    }

    // (2) Free-text filter (search across 4 fields)
    if (term) {
      data = data.filter((u) => {
        const nameMatch = u.name.toLowerCase().includes(term);
        const phoneMatch = u.phoneNumber.toLowerCase().includes(term);
        const statusMatch = u.immuneStatus.toLowerCase().includes(term);
        const yobMatch = String(u.yearOfBirth).includes(term);
        return nameMatch || phoneMatch || statusMatch || yobMatch;
      });
    }

    // (3) Sorting
    data.sort((a, b) => {
      let aVal: string | number;
      let bVal: string | number;

      // Pick comparison fields depending on chosen sort field
      switch (field) {
        case 'name':
          aVal = a.name.toLowerCase();
          bVal = b.name.toLowerCase();
          break;

        case 'yearOfBirth':
          aVal = a.yearOfBirth;
          bVal = b.yearOfBirth;
          break;

        case 'immuneStatus':
          aVal = a.immuneStatus.toLowerCase();
          bVal = b.immuneStatus.toLowerCase();
          break;

        default: // id
          aVal = a.id;
          bVal = b.id;
      }

      // Compare values
      if (aVal < bVal) return dir === 'asc' ? -1 : 1;
      if (aVal > bVal) return dir === 'asc' ? 1 : -1;
      return 0;
    });

    return data;
  });
}