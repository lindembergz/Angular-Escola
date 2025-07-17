import { Component } from '@angular/core';
import { EmDesenvolvimentoComponent } from '../../../../shared/components/em-desenvolvimento/em-desenvolvimento.component';

@Component({
  selector: 'app-disciplinas-list',
  standalone: true,
  imports: [EmDesenvolvimentoComponent],
  template: `
    <app-em-desenvolvimento
      titulo="Gestão de Disciplinas"
      descricao="Módulo para gerenciamento de disciplinas e matérias. Em desenvolvimento."
      iconClass="pi pi-bookmark">
    </app-em-desenvolvimento>
  `
})
export class DisciplinasListComponent {}