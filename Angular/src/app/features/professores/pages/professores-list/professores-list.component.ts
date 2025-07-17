import { Component } from '@angular/core';
import { EmDesenvolvimentoComponent } from '../../../../shared/components/em-desenvolvimento/em-desenvolvimento.component';

@Component({
  selector: 'app-professores-list',
  standalone: true,
  imports: [EmDesenvolvimentoComponent],
  template: `
    <app-em-desenvolvimento
      titulo="Gestão de Professores"
      descricao="Módulo para cadastro e gerenciamento de professores. Em desenvolvimento."
      iconClass="pi pi-user">
    </app-em-desenvolvimento>
  `
})
export class ProfessoresListComponent {}