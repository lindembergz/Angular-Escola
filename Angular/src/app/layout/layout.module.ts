import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';

// PrimeNG Components (Mixed standalone and module imports for v19)
import { Menubar } from 'primeng/menubar';
import { SidebarModule } from 'primeng/sidebar';
import { Button } from 'primeng/button';
import { Avatar } from 'primeng/avatar';
import { Badge } from 'primeng/badge';
import { MenuModule } from 'primeng/menu';
import { TieredMenuModule } from 'primeng/tieredmenu';
import { PanelMenuModule } from 'primeng/panelmenu';
import { RippleModule } from 'primeng/ripple';
import { TooltipModule } from 'primeng/tooltip';

// Layout Components
import { HeaderComponent } from './components/header/header.component';
import { SidebarComponent } from './components/sidebar/sidebar.component';
import { FooterComponent } from './components/footer/footer.component';

@NgModule({
  declarations: [
    HeaderComponent,
    SidebarComponent,
    FooterComponent
  ],
  imports: [
    CommonModule,
    RouterModule,
    Menubar,
    SidebarModule,
    Button,
    Avatar,
    Badge,
    MenuModule,
    TieredMenuModule,
    PanelMenuModule,
    RippleModule,
    TooltipModule
  ],
  exports: [
    HeaderComponent,
    SidebarComponent,
    FooterComponent
  ]
})
export class LayoutModule { }