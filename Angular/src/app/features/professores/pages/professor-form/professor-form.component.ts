import { Component } from '@angular/core';
import { EmDesenvolvimentoComponent } from '../../../../shared/components/em-desenvolvimento/em-desenvolvimento.component';

@Component({
  selector: 'app-professor-form',
  standalone: true,
  imports: [EmDesenvolvimentoComponent],
  template: `
    <app-em-desenvolvimento
      titulo="Formulário de Professor"
      descricao="Formulário para cadastro e edição de professores. Em desenvolvimento."
      iconClass="pi pi-user-plus">
    </app-em-desenvolvimento>
  `
})
export class ProfessorFormComponent {}