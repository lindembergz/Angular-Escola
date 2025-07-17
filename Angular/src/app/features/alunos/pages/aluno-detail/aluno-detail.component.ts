import { Component } from '@angular/core';
import { EmDesenvolvimentoComponent } from '../../../../shared/components/em-desenvolvimento/em-desenvolvimento.component';

@Component({
  selector: 'app-aluno-detail',
  standalone: true,
  imports: [EmDesenvolvimentoComponent],
  template: `
    <app-em-desenvolvimento
      titulo="Detalhes do Aluno"
      descricao="Visualização detalhada das informações do aluno. Em desenvolvimento."
      iconClass="pi pi-eye">
    </app-em-desenvolvimento>
  `
})
export class AlunoDetailComponent {}