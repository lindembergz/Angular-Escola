import { Component } from '@angular/core';
import { EmDesenvolvimentoComponent } from '../../../../shared/components/em-desenvolvimento/em-desenvolvimento.component';

@Component({
  selector: 'app-pagamentos-list',
  standalone: true,
  imports: [EmDesenvolvimentoComponent],
  template: `
    <app-em-desenvolvimento
      titulo="Controle de Pagamentos"
      descricao="Módulo para controle e registro de pagamentos. Em desenvolvimento."
      iconClass="pi pi-money-bill">
    </app-em-desenvolvimento>
  `
})
export class PagamentosListComponent {}