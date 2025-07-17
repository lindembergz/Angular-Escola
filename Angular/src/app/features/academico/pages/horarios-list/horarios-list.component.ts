import { Component } from '@angular/core';
import { EmDesenvolvimentoComponent } from '../../../../shared/components/em-desenvolvimento/em-desenvolvimento.component';

@Component({
  selector: 'app-horarios-list',
  standalone: true,
  imports: [EmDesenvolvimentoComponent],
  template: `
    <app-em-desenvolvimento
      titulo="Grade de Horários"
      descricao="Módulo para gerenciamento de horários e cronogramas. Em desenvolvimento."
      iconClass="pi pi-calendar">
    </app-em-desenvolvimento>
  `
})
export class HorariosListComponent {}