// src/app/events/events.ts
import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';   // *ngFor, etc.
import { FormsModule } from '@angular/forms';     // [(ngModel)]
import { DateTime } from 'luxon';
import { EventsService, EventDto } from '../services/events.service';

type UiEvent = {
  id: string;
  title: string;
  utc: string;     // guaranteed string for display
  sydney: string;
  local: string;
};

@Component({
  selector: 'app-events',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './events.html',   // external template
  styleUrl: './events.css'        // external styles
})
export class EventsComponent {
  // what we render
  events: UiEvent[] = [];
  titleInput = 'Saved from Angular';
  // viewer’s IANA timezone, e.g. "Australia/Perth"
  userZone = Intl.DateTimeFormat().resolvedOptions().timeZone || 'UTC';

  constructor(private api: EventsService) {
    this.refresh();
  }

  createNow() {
    this.api.create(this.titleInput).subscribe(() => this.refresh());
  }

  refresh() {
    this.api.list().subscribe((items: EventDto[]) => {
      this.events = items.map((e) => {
        // parse the instant as UTC
        const utc = DateTime.fromISO(e.savedUtcIso, { zone: 'utc' });

        // Luxon toISO() can be null; coalesce to a formatted fallback
        const utcStr =
          utc.toUTC().toISO({ suppressMilliseconds: true }) ??
          utc.toUTC().toFormat("yyyy-LL-dd'T'HH:mm:ss'Z'");

        return {
          id: e.id,
          title: e.title,
          utc: utcStr,
          sydney: utc
            .setZone('Australia/Sydney')
            .toLocaleString(DateTime.DATETIME_MED_WITH_SECONDS),
          local: utc
            .setZone(this.userZone)
            .toLocaleString(DateTime.DATETIME_MED_WITH_SECONDS),
        };
      });
    });
  }
}
