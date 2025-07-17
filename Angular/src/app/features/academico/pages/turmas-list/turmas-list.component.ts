import { Component } from '@angular/core';
import { EmDesenvolvimentoComponent } from '../../../../shared/components/em-desenvolvimento/em-desenvolvimento.component';

@Component({
  selector: 'app-turmas-list',
  standalone: true,
  imports: [EmDesenvolvimentoComponent],
  template: `
    <app-em-desenvolvimento
      titulo="Gestão de Turmas"
      descricao="Módulo para gerenciamento de turmas e classes. Em desenvolvimento."
      iconClass="pi pi-users">
    </app-em-desenvolvimento>
  `
})
export class TurmasListComponent {}