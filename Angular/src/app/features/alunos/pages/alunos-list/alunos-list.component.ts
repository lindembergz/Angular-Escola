import { Component } from '@angular/core';
import { EmDesenvolvimentoComponent } from '../../../../shared/components/em-desenvolvimento/em-desenvolvimento.component';

@Component({
  selector: 'app-alunos-list',
  standalone: true,
  imports: [EmDesenvolvimentoComponent],
  template: `
    <app-em-desenvolvimento
      titulo="Gestão de Alunos"
      descricao="Módulo para cadastro e gerenciamento de alunos. Em desenvolvimento."
      iconClass="pi pi-users">
    </app-em-desenvolvimento>
  `
})
export class AlunosListComponent {}