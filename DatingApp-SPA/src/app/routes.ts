import {Routes} from '@angular/router';
import { HomeComponent } from './home/home.component';
import { MemberListComponent } from './member-list/member-list.component';
import { MessagesComponent } from './messages/messages.component';
import { ListComponent } from './list/list.component';
import { AuthGuard } from './_guards/auth.guard';

//Ordering is important on Routing because URL checks match in order,
export const appRoutes: Routes = [
    { path: '', component: HomeComponent},
    {
        path: '',
        runGuardsAndResolvers: 'always',
        canActivate: [AuthGuard],
        children: [
            { path: 'Members', component: MemberListComponent},
            { path: 'Messages', component: MessagesComponent},
            { path: 'Lists', component: ListComponent},
        ]
    },
    { path: '**', redirectTo: '', pathMatch: 'full'},
]