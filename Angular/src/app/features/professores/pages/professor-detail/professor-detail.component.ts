import { Component } from '@angular/core';
import { EmDesenvolvimentoComponent } from '../../../../shared/components/em-desenvolvimento/em-desenvolvimento.component';

@Component({
  selector: 'app-professor-detail',
  standalone: true,
  imports: [EmDesenvolvimentoComponent],
  template: `
    <app-em-desenvolvimento
      titulo="Detalhes do Professor"
      descricao="Visualização detalhada das informações do professor. Em desenvolvimento."
      iconClass="pi pi-eye">
    </app-em-desenvolvimento>
  `
})
export class ProfessorDetailComponent {}