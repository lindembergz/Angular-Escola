import { Component } from '@angular/core';
import { EmDesenvolvimentoComponent } from '../../../../shared/components/em-desenvolvimento/em-desenvolvimento.component';

@Component({
  selector: 'app-relatorios-financeiros',
  standalone: true,
  imports: [EmDesenvolvimentoComponent],
  template: `
    <app-em-desenvolvimento
      titulo="Relatórios Financeiros"
      descricao="Módulo para geração de relatórios financeiros e análises. Em desenvolvimento."
      iconClass="pi pi-chart-pie">
    </app-em-desenvolvimento>
  `
})
export class RelatoriosFinanceirosComponent {}