import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { RouterModule } from '@angular/router';

// PrimeNG Components (Mixed standalone and module imports for v19)
import { Button } from 'primeng/button';
import { Card } from 'primeng/card';
import { InputText } from 'primeng/inputtext';
import { DropdownModule } from 'primeng/dropdown';
import { TableModule } from 'primeng/table';
import { DialogModule } from 'primeng/dialog';
import { ConfirmDialogModule } from 'primeng/confirmdialog';
import { ToastModule } from 'primeng/toast';
import { ProgressSpinner } from 'primeng/progressspinner';
import { Toolbar } from 'primeng/toolbar';
import { Tag } from 'primeng/tag';
import { TabViewModule } from 'primeng/tabview';
import { Breadcrumb } from 'primeng/breadcrumb';
import { Menubar } from 'primeng/menubar';
import { Skeleton } from 'primeng/skeleton';
import { DividerModule } from 'primeng/divider';
import { TooltipModule } from 'primeng/tooltip';

// Componentes compartilhados (serão criados)
// import { LoadingSpinnerComponent } from './components/loading-spinner/loading-spinner.component';
// import { ConfirmationDialogComponent } from './components/confirmation-dialog/confirmation-dialog.component';
// import { PageHeaderComponent } from './components/page-header/page-header.component';

// Pipes compartilhados (serão criados)
// import { CpfPipe } from './pipes/cpf.pipe';
// import { PhonePipe } from './pipes/phone.pipe';
// import { CurrencyBrPipe } from './pipes/currency-br.pipe';

// Diretivas compartilhadas (serão criadas)
// import { HighlightDirective } from './directives/highlight.directive';
// import { ClickOutsideDirective } from './directives/click-outside.directive';

const PRIMENG_COMPONENTS = [
  Button,
  Card,
  InputText,
  DropdownModule,
  TableModule,
  DialogModule,
  ConfirmDialogModule,
  ToastModule,
  ProgressSpinner,
  Toolbar,
  Tag,
  TabViewModule,
  Breadcrumb,
  Menubar,
  Skeleton,
  DividerModule,
  TooltipModule
];

const SHARED_COMPONENTS = [
  // LoadingSpinnerComponent,
  // ConfirmationDialogComponent,
  // PageHeaderComponent
];

const SHARED_PIPES = [
  // CpfPipe,
  // PhonePipe,
  // CurrencyBrPipe
];

const SHARED_DIRECTIVES = [
  // HighlightDirective,
  // ClickOutsideDirective
];

@NgModule({
  declarations: [
    ...SHARED_COMPONENTS,
    ...SHARED_PIPES,
    ...SHARED_DIRECTIVES
  ],
  imports: [
    CommonModule,
    FormsModule,
    ReactiveFormsModule,
    RouterModule,
    ...PRIMENG_COMPONENTS
  ],
  exports: [
    // Angular modules
    CommonModule,
    FormsModule,
    ReactiveFormsModule,
    RouterModule,
    
    // PrimeNG standalone components
    ...PRIMENG_COMPONENTS,
    
    // Shared components, pipes, directives
    ...SHARED_COMPONENTS,
    ...SHARED_PIPES,
    ...SHARED_DIRECTIVES
  ]
})
export class SharedModule { }