import { Component, inject, OnInit } from '@angular/core';
import { TableModule } from 'primeng/table';
import { ButtonModule } from 'primeng/button';
import { RippleModule } from 'primeng/ripple';
import { CommonModule } from '@angular/common';
import { UserService } from '../../../../core/services/user.service';
import { UserInEvent } from '../../../../core/model/response/participant.model';

@Component({
  selector: 'app-participant-list',
  standalone: true,
  imports: [TableModule, CommonModule, ButtonModule, RippleModule],
  templateUrl: './participant-list.component.html',
  styleUrl: './participant-list.component.scss'
})
export class ParticipantListComponent implements OnInit {
  private userService = inject(UserService);

  participants: UserInEvent[] = [];
  totalRecords = 0;
  first = 0;
  rows = 10;
  pageIndex = 1;

  ngOnInit(): void {
    this.loadParticipants();
  }

  loadParticipants() {
    this.userService.GetParticipants(this.pageIndex, this.rows).subscribe({
      next: (res) => {
        this.participants = res.items;
        this.totalRecords = res.totalRecords;
      },
      error: (err) => {
        console.error('Error fetching participants', err);
      }
    });
  }

  pageChange(event: any) {
    this.first = event.first;
    this.rows = event.rows;
    this.pageIndex = event.first / event.rows + 1;
    this.loadParticipants();
  }
}
