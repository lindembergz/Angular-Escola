import { Component } from '@angular/core';
import { EmDesenvolvimentoComponent } from '../../../../shared/components/em-desenvolvimento/em-desenvolvimento.component';

@Component({
  selector: 'app-disciplina-form',
  standalone: true,
  imports: [EmDesenvolvimentoComponent],
  template: `
    <app-em-desenvolvimento
      titulo="Formulário de Disciplina"
      descricao="Formulário para criação e edição de disciplinas. Em desenvolvimento."
      iconClass="pi pi-plus">
    </app-em-desenvolvimento>
  `
})
export class DisciplinaFormComponent {}