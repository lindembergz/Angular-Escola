import { Component } from '@angular/core';
import { EmDesenvolvimentoComponent } from '../../../../shared/components/em-desenvolvimento/em-desenvolvimento.component';

@Component({
  selector: 'app-turma-detail',
  standalone: true,
  imports: [EmDesenvolvimentoComponent],
  template: `
    <app-em-desenvolvimento
      titulo="Detalhes da Turma"
      descricao="Visualização detalhada da turma e seus alunos. Em desenvolvimento."
      iconClass="pi pi-eye">
    </app-em-desenvolvimento>
  `
})
export class TurmaDetailComponent {}