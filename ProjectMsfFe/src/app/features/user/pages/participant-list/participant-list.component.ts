import { Component, inject, OnInit } from '@angular/core';
import { TableModule } from 'primeng/table';
import { ButtonModule } from 'primeng/button';
import { RippleModule } from 'primeng/ripple';
import { CommonModule } from '@angular/common';
import { UserService } from '../../../../core/services/user.service';
import { ParticipantSummary } from '../../../../core/model/response/participant.model';
import { Router } from '@angular/router';
import { VndCurrencyPipe } from '../../../../shared/pipes/vnd-currency.pipe';
import { TagModule } from 'primeng/tag';
import { TooltipModule } from 'primeng/tooltip';
import { ImageUrlPipe } from '../../../../shared/pipes/image-url.pipe';

@Component({
  selector: 'app-participant-list',
  standalone: true,
  imports: [TableModule,
    ImageUrlPipe, CommonModule, ButtonModule, RippleModule, VndCurrencyPipe, TagModule, TooltipModule],
  templateUrl: './participant-list.component.html',
  styleUrl: './participant-list.component.scss',
})
export class ParticipantListComponent implements OnInit {
  private userService = inject(UserService);
  private router = inject(Router);

  participants: ParticipantSummary[] = [];
  totalRecords = 0;
  first = 0;
  rows = 10;
  pageIndex = 1;

  ngOnInit(): void {
    this.loadParticipants();
  }

  loadParticipants() {
    this.userService.GetParticipants(this.pageIndex, this.rows).subscribe({
      next: (res: any) => {
        console.log(res.items);
        this.participants = res.items

        this.totalRecords = res.totalRecords;
      },
      error: (err) => {
        console.error('Error fetching participants', err);
      },
    });
  }

  pageChange(event: any) {
    this.first = event.first;
    this.rows = event.rows;
    this.pageIndex = event.first / event.rows + 1;
    this.loadParticipants();
  }

  viewDetail(userId: string) {
    this.router.navigate(['/admin/participants', userId]);
  }
}
