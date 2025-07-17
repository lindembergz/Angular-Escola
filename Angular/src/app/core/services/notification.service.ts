import { Injectable } from '@angular/core';
import { MessageService } from 'primeng/api';

export type NotificationType = 'success' | 'info' | 'warn' | 'error';

export interface NotificationOptions {
  title?: string;
  message: string;
  type: NotificationType;
  duration?: number;
  sticky?: boolean;
  closable?: boolean;
}

@Injectable({
  providedIn: 'root'
})
export class NotificationService {
  constructor(private messageService: MessageService) {}

  show(options: NotificationOptions): void {
    this.messageService.add({
      severity: options.type,
      summary: options.title || this.getDefaultTitle(options.type),
      detail: options.message,
      life: options.sticky ? undefined : (options.duration || 5000),
      closable: options.closable !== false
    });
  }

  success(message: string, title?: string, duration?: number): void {
    this.show({
      type: 'success',
      message,
      title,
      duration
    });
  }

  info(message: string, title?: string, duration?: number): void {
    this.show({
      type: 'info',
      message,
      title,
      duration
    });
  }

  warn(message: string, title?: string, duration?: number): void {
    this.show({
      type: 'warn',
      message,
      title,
      duration
    });
  }

  error(message: string, title?: string, duration?: number): void {
    this.show({
      type: 'error',
      message,
      title,
      duration: duration || 8000 // Erros ficam mais tempo visíveis
    });
  }

  clear(): void {
    this.messageService.clear();
  }

  private getDefaultTitle(type: NotificationType): string {
    const titles = {
      success: 'Sucesso',
      info: 'Informação',
      warn: 'Atenção',
      error: 'Erro'
    };
    
    return titles[type];
  }
}