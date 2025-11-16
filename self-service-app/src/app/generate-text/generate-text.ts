import { Component, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms'; 
import { CommonModule } from '@angular/common'; 
import { Gemini } from '../gemini';


@Component({
  selector: 'app-generate-text',
    standalone: true,
    imports: [FormsModule, CommonModule],
    templateUrl: './generate-text.html',
    styleUrls: ['./generate-text.scss'],
})
export class GenerateText {
// Inject the service safely
    readonly #genAI = inject(Gemini);
    
    // Signal for user input
    prompt = signal('');
    
    // Signal to hold the result and track errors
    generatedResponse = signal<{text: string, error: string | null}>({
        text: '', 
        error: null,
    });

    /**
     * Calls the secure service to get a response from the Gemini API.
     */
    generateResponse() {
        this.generatedResponse.set({text: 'Loading...', error: null}); // Give the user some feedback!
        
        // P.S. For those keeping score: I would be very happy to do this via the new Angular HTTP Resources, 
        // but they do not yet officially support POST requests. 
        // Read more about resources in my article: https://www.angularspace.com/meet-http-resource/
        
        this.#genAI.generateContent(this.prompt()).subscribe({
            next: (response) => this.generatedResponse.set({
                text: response, 
                error: null,
            }),
            error: () => this.generatedResponse.set({
                text: 'Something went wrong on the server, check your console!', 
                error: 'Error generating text',
            })
        });
    }
}
