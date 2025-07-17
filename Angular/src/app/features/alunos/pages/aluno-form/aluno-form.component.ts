import { Component } from '@angular/core';
import { EmDesenvolvimentoComponent } from '../../../../shared/components/em-desenvolvimento/em-desenvolvimento.component';

@Component({
  selector: 'app-aluno-form',
  standalone: true,
  imports: [EmDesenvolvimentoComponent],
  template: `
    <app-em-desenvolvimento
      titulo="Formulário de Aluno"
      descricao="Formulário para cadastro e edição de alunos. Em desenvolvimento."
      iconClass="pi pi-user-plus">
    </app-em-desenvolvimento>
  `
})
export class AlunoFormComponent {}