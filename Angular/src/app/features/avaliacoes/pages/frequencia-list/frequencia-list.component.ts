import { Component } from '@angular/core';
import { EmDesenvolvimentoComponent } from '../../../../shared/components/em-desenvolvimento/em-desenvolvimento.component';

@Component({
  selector: 'app-frequencia-list',
  standalone: true,
  imports: [EmDesenvolvimentoComponent],
  template: `
    <app-em-desenvolvimento
      titulo="Controle de Frequência"
      descricao="Módulo para controle de presença e faltas. Em desenvolvimento."
      iconClass="pi pi-check-circle">
    </app-em-desenvolvimento>
  `
})
export class FrequenciaListComponent {}