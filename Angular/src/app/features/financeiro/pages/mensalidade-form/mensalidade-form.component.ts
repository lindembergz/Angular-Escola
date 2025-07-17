import { Component } from '@angular/core';
import { EmDesenvolvimentoComponent } from '../../../../shared/components/em-desenvolvimento/em-desenvolvimento.component';

@Component({
  selector: 'app-mensalidade-form',
  standalone: true,
  imports: [EmDesenvolvimentoComponent],
  template: `
    <app-em-desenvolvimento
      titulo="Cadastro de Mensalidade"
      descricao="Formulário para cadastro e edição de mensalidades. Em desenvolvimento."
      iconClass="pi pi-plus">
    </app-em-desenvolvimento>
  `
})
export class MensalidadeFormComponent {}