import { Component } from '@angular/core';
import { EmDesenvolvimentoComponent } from '../../../../shared/components/em-desenvolvimento/em-desenvolvimento.component';

@Component({
  selector: 'app-notas-list',
  standalone: true,
  imports: [EmDesenvolvimentoComponent],
  template: `
    <app-em-desenvolvimento
      titulo="Gestão de Notas"
      descricao="Módulo para lançamento e consulta de notas. Em desenvolvimento."
      iconClass="pi pi-star">
    </app-em-desenvolvimento>
  `
})
export class NotasListComponent {}