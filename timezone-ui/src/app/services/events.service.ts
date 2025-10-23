import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';

export interface EventDto {
  id: string;
  title: string;
  savedUtcIso: string; // ISO-8601 UTC ("...Z")
  savedTz: string;     // e.g., "Australia/Sydney"
}


@Injectable({
  providedIn: 'root'
})
export class EventsService  {
  
  constructor(private http: HttpClient) {}

  list(): Observable<EventDto[]> {
    // proxied to http://localhost:5000/api/events
    return this.http.get<EventDto[]>('/api/events');
  }

  create(title: string): Observable<EventDto> {
    return this.http.post<EventDto>('/api/events', { title });
  }
}
