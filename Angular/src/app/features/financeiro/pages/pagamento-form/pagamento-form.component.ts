import { Component } from '@angular/core';
import { EmDesenvolvimentoComponent } from '../../../../shared/components/em-desenvolvimento/em-desenvolvimento.component';

@Component({
  selector: 'app-pagamento-form',
  standalone: true,
  imports: [EmDesenvolvimentoComponent],
  template: `
    <app-em-desenvolvimento
      titulo="Registro de Pagamento"
      descricao="Formulário para registro de pagamentos recebidos. Em desenvolvimento."
      iconClass="pi pi-check">
    </app-em-desenvolvimento>
  `
})
export class PagamentoFormComponent {}