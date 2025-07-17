import { Component } from '@angular/core';
import { EmDesenvolvimentoComponent } from '../../../../shared/components/em-desenvolvimento/em-desenvolvimento.component';

@Component({
  selector: 'app-nota-form',
  standalone: true,
  imports: [EmDesenvolvimentoComponent],
  template: `
    <app-em-desenvolvimento
      titulo="Lançamento de Notas"
      descricao="Formulário para lançamento de notas e avaliações. Em desenvolvimento."
      iconClass="pi pi-pencil">
    </app-em-desenvolvimento>
  `
})
export class NotaFormComponent {}