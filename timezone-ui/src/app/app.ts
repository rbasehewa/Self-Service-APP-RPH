import { Component, signal } from '@angular/core';
import { EventsComponent } from './events/events';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [EventsComponent],
  templateUrl: './app.html',
  styleUrl: './app.css'
})
export class App {
  protected readonly title = signal('timezone-ui');
}
