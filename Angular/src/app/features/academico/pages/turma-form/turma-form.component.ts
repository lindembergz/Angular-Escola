import { Component } from '@angular/core';
import { EmDesenvolvimentoComponent } from '../../../../shared/components/em-desenvolvimento/em-desenvolvimento.component';

@Component({
  selector: 'app-turma-form',
  standalone: true,
  imports: [EmDesenvolvimentoComponent],
  template: `
    <app-em-desenvolvimento
      titulo="Formulário de Turma"
      descricao="Formulário para criação e edição de turmas. Em desenvolvimento."
      iconClass="pi pi-plus">
    </app-em-desenvolvimento>
  `
})
export class TurmaFormComponent {}