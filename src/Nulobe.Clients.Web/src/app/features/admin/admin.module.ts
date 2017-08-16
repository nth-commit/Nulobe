import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MdButtonModule, MdDialogModule } from '@angular/material';

import { AuthModule } from '../../features/auth';
import { CoreModule } from '../../core';
import { AuthHttp, authHttpProvider } from '../../auth-http.service';

import { AdminRoutingModule } from './admin-routing.module';
import { AdminStateModule } from './state/admin-state.module';
import { AdminComponent } from './admin.component';
import { CreateFactComponent } from './pages/create-fact/create-fact.component';
import { FactPreviewDialogComponent } from './pages/fact-preview-dialog/fact-preview-dialog.component';
import { EditFactComponent } from './pages/edit-fact/edit-fact.component';
import { EditFactResolve } from './pages/edit-fact/edit-fact.resolve';

@NgModule({
  imports: [
    CommonModule,
    MdButtonModule,
    MdDialogModule,

    CoreModule,
    AuthModule,

    AdminRoutingModule,
    AdminStateModule,
  ],
  declarations: [
    AdminComponent,
    CreateFactComponent,
    FactPreviewDialogComponent,
    EditFactComponent
  ],
  entryComponents: [
    FactPreviewDialogComponent
  ],
  providers: [
    AuthHttp,
    authHttpProvider,
    EditFactResolve
  ]
})
export class AdminModule { }
