import { Component } from '@angular/core';
import { EmDesenvolvimentoComponent } from '../../../../shared/components/em-desenvolvimento/em-desenvolvimento.component';

@Component({
  selector: 'app-relatorios-academicos',
  standalone: true,
  imports: [EmDesenvolvimentoComponent],
  template: `
    <app-em-desenvolvimento
      titulo="Relatórios Acadêmicos"
      descricao="Módulo para geração de relatórios acadêmicos e estatísticas. Em desenvolvimento."
      iconClass="pi pi-chart-line">
    </app-em-desenvolvimento>
  `
})
export class RelatoriosAcademicosComponent {}