import { Component } from '@angular/core';
import { EmDesenvolvimentoComponent } from '../../../../shared/components/em-desenvolvimento/em-desenvolvimento.component';

@Component({
  selector: 'app-mensalidades-list',
  standalone: true,
  imports: [EmDesenvolvimentoComponent],
  template: `
    <app-em-desenvolvimento
      titulo="Gestão de Mensalidades"
      descricao="Módulo para controle de mensalidades e cobranças. Em desenvolvimento."
      iconClass="pi pi-credit-card">
    </app-em-desenvolvimento>
  `
})
export class MensalidadesListComponent {}