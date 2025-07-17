import { Component } from '@angular/core';
import { EmDesenvolvimentoComponent } from '../../../../shared/components/em-desenvolvimento/em-desenvolvimento.component';

@Component({
  selector: 'app-frequencia-form',
  standalone: true,
  imports: [EmDesenvolvimentoComponent],
  template: `
    <app-em-desenvolvimento
      titulo="Lançamento de Frequência"
      descricao="Formulário para registro de presença e faltas. Em desenvolvimento."
      iconClass="pi pi-calendar-plus">
    </app-em-desenvolvimento>
  `
})
export class FrequenciaFormComponent {}