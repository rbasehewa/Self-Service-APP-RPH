import { CommonModule } from '@angular/common';
import { Component, computed, effect, input, signal } from '@angular/core';
import { User } from '../models/user.model';
import { FormsModule } from '@angular/forms';
import { FontAwesomeModule } from '@fortawesome/angular-fontawesome';
import { faSort, faSortUp, faSortDown, faUsers } from '@fortawesome/free-solid-svg-icons';

type SortField = 'id' | 'name' | 'yearOfBirth' | 'immuneStatus';
type SortDirection = 'asc' | 'desc';

@Component({
  selector: 'app-usertable',
  standalone: true,
  imports: [CommonModule, FormsModule, FontAwesomeModule],
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

  
  /** Last AI query text (used only for empty-state messaging) */
  lastQuery = input<string>('');

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
  readonly faSort = faSort; // neutral icon
  readonly faSortUp = faSortUp; // ascending icon
  readonly faSortDown = faSortDown; // descending icon
  readonly faUsers = faUsers; // descending icon

  // ----------------------------
  // Pagination state (15 per page)
  // ----------------------------

  /**
   * pageSize:
   *   - How many rows per page.
   *   - Fixed at 15 for this demo, but could be made configurable later.
   */
  readonly pageSize = 15;

  /**
   * currentPage:
   *   - 1-based page index (1, 2, 3, ...).
   */
  currentPage = signal(1);

  constructor() {
    // React when parent updates user list (LLM result vs full set).
    effect(() => {
      const _data = this.users();
      // If you want to reset filters on new dataset, you could do it here.
    });

    /**
     * Ensure currentPage is always in a valid range when
     * the filtered list size changes (e.g. new LLM result or filters).
     */
    effect(() => {
      const total = this.totalPages();
      const page = this.currentPage();
      if (page > total) {
        this.currentPage.set(total);
      }
      if (page < 1) {
        this.currentPage.set(1);
      }
    });
  }

  // ----------------------------
  // Sorting + filter handlers
  // ----------------------------

  setSort(field: SortField) {
    if (this.sortField() === field) {
      this.sortDirection.set(this.sortDirection() === 'asc' ? 'desc' : 'asc');
    } else {
      this.sortField.set(field);
      this.sortDirection.set('asc');
    }
  }

  setFilterText(value: string) {
    this.filterText.set(value);
    // When filters change, go back to page 1.
    this.currentPage.set(1);
  }

  setStatusFilter(value: 'All' | 'Immune' | 'Non-Immune' | 'Unknown') {
    this.filterStatus.set(value);
    // Reset to first page when user changes status filter.
    this.currentPage.set(1);
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

  // ----------------------------
  // Pagination derived values
  // ----------------------------

  /** Total number of pages for the current filtered list. */
  readonly totalPages = computed(() => {
    const total = this.filteredAndSortedUsers().length;
    return total === 0 ? 1 : Math.ceil(total / this.pageSize);
  });

  /** Simple [1, 2, 3, ...] array for page buttons. */
  readonly pages = computed(() => Array.from({ length: this.totalPages() }, (_, i) => i + 1));

  /**
   * pagedUsers:
   *   - This is what the template actually renders in @for.
   *   - Takes the filtered + sorted list and slices out 15 rows.
   */
  readonly pagedUsers = computed(() => {
    const page = this.currentPage();
    const start = (page - 1) * this.pageSize;
    const end = start + this.pageSize;
    return this.filteredAndSortedUsers().slice(start, end);
  });

  // ----------------------------
  // Pagination actions
  // ----------------------------

  goToPage(page: number) {
    const clamped = Math.min(Math.max(page, 1), this.totalPages());
    this.currentPage.set(clamped);
  }

  nextPage() {
    this.goToPage(this.currentPage() + 1);
  }

  previousPage() {
    this.goToPage(this.currentPage() - 1);
  }
}
