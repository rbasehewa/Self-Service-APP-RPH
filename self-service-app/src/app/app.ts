import { Component, signal } from '@angular/core';
import { GenerateText } from "./generate-text/generate-text";

@Component({
  selector: 'app-root',
  imports: [GenerateText],
  templateUrl: './app.html',
  styleUrl: './app.scss'
})
export class App {}
